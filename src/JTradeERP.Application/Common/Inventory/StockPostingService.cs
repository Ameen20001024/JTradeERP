using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Domain.Inventory;
using JTradeERP.Domain.MasterData;

namespace JTradeERP.Application.Common.Inventory;

public sealed class StockPostingService(IApplicationDbContext db, IClock clock) : IStockPostingService
{
    private readonly Dictionary<(Guid ProductId, Guid WarehouseId), StockBalance> balances = [];

    public StockLedgerEntry Receive(
        Guid productId, Guid warehouseId, decimal quantity, decimal unitCost,
        string sourceDocumentType, Guid sourceDocumentId, Guid? sourceLineId,
        StockMovementType movementType, Guid actorUserId, string correlationId)
    {
        var existing = FindExisting(sourceDocumentType, sourceDocumentId, sourceLineId, movementType, StockMovementDirection.In);
        if (existing is not null)
        {
            EnsureSamePosting(existing, productId, warehouseId, quantity, unitCost);
            return existing;
        }

        var (product, warehouse) = GetActiveReferences(productId, warehouseId);
        _ = product;
        var balance = GetOrCreateBalance(product.Id, warehouse.Id);
        balance.Receive(quantity, unitCost);
        var entry = new StockLedgerEntry(productId, warehouseId, movementType, StockMovementDirection.In,
            quantity, unitCost, sourceDocumentType, sourceDocumentId, sourceLineId, null,
            clock.UtcNow, actorUserId, correlationId);
        db.Add(entry);
        return entry;
    }

    public StockLedgerEntry Issue(
        Guid productId, Guid warehouseId, decimal quantity,
        string sourceDocumentType, Guid sourceDocumentId, Guid? sourceLineId,
        StockMovementType movementType, Guid actorUserId, string correlationId)
    {
        var existing = FindExisting(sourceDocumentType, sourceDocumentId, sourceLineId, movementType, StockMovementDirection.Out);
        if (existing is not null)
        {
            EnsureSamePosting(existing, productId, warehouseId, quantity, existing.UnitCost);
            return existing;
        }

        var (product, warehouse) = GetActiveReferences(productId, warehouseId);
        var balance = GetOrCreateBalance(product.Id, warehouse.Id);
        if (!warehouse.AllowNegativeStock && !balance.CanIssue(quantity))
            throw new ConflictException("Insufficient available stock for this issue.");

        var unitCost = balance.WeightedAverageCost;
        balance.Issue(quantity);
        var entry = new StockLedgerEntry(productId, warehouseId, movementType, StockMovementDirection.Out,
            quantity, unitCost, sourceDocumentType, sourceDocumentId, sourceLineId, null,
            clock.UtcNow, actorUserId, correlationId);
        db.Add(entry);
        return entry;
    }

    public void Reserve(Guid productId, Guid warehouseId, decimal quantity)
    {
        var (product, warehouse) = GetActiveReferences(productId, warehouseId);
        var balance = GetOrCreateBalance(product.Id, warehouse.Id);
        if (!balance.TryReserve(quantity, warehouse.AllowNegativeStock))
            throw new ConflictException("Insufficient available stock for this reservation.");
    }

    public void ReleaseReservation(Guid productId, Guid warehouseId, decimal quantity)
    {
        var balance = GetOrCreateBalance(productId, warehouseId);
        if (!balance.TryReleaseReservation(quantity))
            throw new ConflictException("The requested reservation is no longer available to release.");
    }

    public StockLedgerEntry ConvertReservationToIssue(Guid productId, Guid warehouseId, decimal quantity,
        string sourceDocumentType, Guid sourceDocumentId, Guid sourceLineId, Guid actorUserId, string correlationId)
    {
        var existing = FindExisting(sourceDocumentType, sourceDocumentId, sourceLineId, StockMovementType.Issue, StockMovementDirection.Out);
        if (existing is not null)
        {
            EnsureSamePosting(existing, productId, warehouseId, quantity, existing.UnitCost);
            return existing;
        }
        var (product, warehouse) = GetActiveReferences(productId, warehouseId);
        var balance = GetOrCreateBalance(product.Id, warehouse.Id);
        if (!balance.TryConvertReservationToIssue(quantity, warehouse.AllowNegativeStock))
            throw new ConflictException("Invoice quantity exceeds its reservation or permitted stock.");
        var entry = new StockLedgerEntry(productId, warehouseId, StockMovementType.Issue, StockMovementDirection.Out,
            quantity, balance.WeightedAverageCost, sourceDocumentType, sourceDocumentId, sourceLineId, null,
            clock.UtcNow, actorUserId, correlationId);
        db.Add(entry);
        return entry;
    }

    public StockBalance GetOrCreateBalance(Guid productId, Guid warehouseId)
    {
        var key = (productId, warehouseId);
        if (balances.TryGetValue(key, out var trackedBalance))
            return trackedBalance;
        var balance = db.StockBalances.SingleOrDefault(x => x.ProductId == productId && x.WarehouseId == warehouseId);
        if (balance is not null)
        {
            balances[key] = balance;
            return balance;
        }
        balance = new StockBalance(productId, warehouseId);
        db.Add(balance);
        balances[key] = balance;
        return balance;
    }

    public void EnsureActiveReferences(Guid productId, Guid warehouseId) => _ = GetActiveReferences(productId, warehouseId);

    public StockLedgerEntry Reverse(StockLedgerEntry original, Guid reversalDocumentId, string reversalDocumentType, Guid actorUserId, string correlationId)
    {
        var existing = db.StockLedgerEntries.SingleOrDefault(x => x.ReversalOfLedgerEntryId == original.Id);
        if (existing is not null)
            return existing;

        var (_, warehouse) = GetActiveReferences(original.ProductId, original.WarehouseId);
        var balance = GetOrCreateBalance(original.ProductId, original.WarehouseId);
        var reverseDirection = original.Direction == StockMovementDirection.In
            ? StockMovementDirection.Out
            : StockMovementDirection.In;

        if (reverseDirection == StockMovementDirection.Out)
        {
            if (!warehouse.AllowNegativeStock && !balance.CanIssue(original.Quantity))
                throw new ConflictException("This reversal would make available stock negative.");
            try
            {
                balance.ReverseReceipt(original.Quantity, original.UnitCost, warehouse.AllowNegativeStock);
            }
            catch (InvalidOperationException exception)
            {
                throw new ConflictException(exception.Message);
            }
        }
        else
        {
            balance.Receive(original.Quantity, original.UnitCost);
        }

        var reversal = new StockLedgerEntry(original.ProductId, original.WarehouseId, StockMovementType.Reversal,
            reverseDirection, original.Quantity, original.UnitCost, reversalDocumentType, reversalDocumentId,
            original.Id, original.Id, clock.UtcNow, actorUserId, correlationId);
        db.Add(reversal);
        return reversal;
    }

    private (Product Product, Warehouse Warehouse) GetActiveReferences(Guid productId, Guid warehouseId)
    {
        var product = db.Products.SingleOrDefault(x => x.Id == productId)
            ?? throw new NotFoundException("Product was not found.");
        var warehouse = db.Warehouses.SingleOrDefault(x => x.Id == warehouseId)
            ?? throw new NotFoundException("Warehouse was not found.");
        if (!product.IsActive)
            throw new ConflictException("Inactive products cannot be used in new stock postings.");
        if (!warehouse.IsActive)
            throw new ConflictException("Inactive warehouses cannot be used in new stock postings.");
        return (product, warehouse);
    }

    private StockLedgerEntry? FindExisting(string sourceType, Guid sourceId, Guid? sourceLineId, StockMovementType type, StockMovementDirection direction) =>
        db.StockLedgerEntries.SingleOrDefault(x => x.SourceDocumentType == sourceType
            && x.SourceDocumentId == sourceId && x.SourceLineId == sourceLineId
            && x.MovementType == type && x.Direction == direction);

    private static void EnsureSamePosting(StockLedgerEntry existing, Guid productId, Guid warehouseId, decimal quantity, decimal unitCost)
    {
        if (existing.ProductId != productId || existing.WarehouseId != warehouseId || existing.Quantity != quantity || existing.UnitCost != unitCost)
            throw new ConflictException("The source-document reference was already posted with different stock values.");
    }
}
