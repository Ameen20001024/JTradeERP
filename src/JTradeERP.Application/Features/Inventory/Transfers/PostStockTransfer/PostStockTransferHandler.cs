using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Inventory;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.PostStockTransfer;
public sealed class PostStockTransferHandler(IApplicationDbContext db, IStockPostingService stock,
    ICurrentUserService actor, IClock clock) : IRequestHandler<PostStockTransferCommand, PostStockTransferResponse>
{
    public async Task<PostStockTransferResponse> Handle(PostStockTransferCommand request, CancellationToken cancellationToken)
    {
        var transfer = db.StockTransfers.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Stock transfer was not found.");
        var lines = db.StockTransferLines.Where(x => x.StockTransferId == transfer.Id).OrderBy(x => x.ProductId).ToArray();
        if (transfer.Status != InventoryDocumentStatus.Draft)
        {
            if (transfer.Status != InventoryDocumentStatus.Posted)
                throw new ConflictException("Only draft transfers can be posted.");
            var prior = lines.Select(line => new StockTransferMovementResponse(line.Id, line.ProductId, line.Quantity,
                db.StockLedgerEntries.Single(x => x.SourceDocumentType == "StockTransfer" && x.SourceDocumentId == transfer.Id
                    && x.SourceLineId == line.Id && x.Direction == StockMovementDirection.Out).UnitCost,
                db.StockLedgerEntries.Single(x => x.SourceDocumentType == "StockTransfer" && x.SourceDocumentId == transfer.Id
                    && x.SourceLineId == line.Id && x.Direction == StockMovementDirection.Out).Id,
                db.StockLedgerEntries.Single(x => x.SourceDocumentType == "StockTransfer" && x.SourceDocumentId == transfer.Id
                    && x.SourceLineId == line.Id && x.Direction == StockMovementDirection.In).Id)).ToArray();
            return new(StockTransferResponseMapper.Map(db, transfer, lines), prior);
        }
        if (lines.Length == 0) throw new ConflictException("A transfer requires at least one line.");
        stock.EnsureActiveReferences(lines[0].ProductId, transfer.SourceWarehouseId);
        var sourceWarehouse = db.Warehouses.Single(x => x.Id == transfer.SourceWarehouseId);
        var destinationWarehouse = db.Warehouses.SingleOrDefault(x => x.Id == transfer.DestinationWarehouseId)
            ?? throw new NotFoundException("Destination warehouse was not found.");
        if (!destinationWarehouse.IsActive) throw new ConflictException("Inactive warehouses cannot receive stock.");

        // Preload balance rows in stable product/warehouse key order, then validate aggregate demand before mutating.
        foreach (var productId in lines.Select(x => x.ProductId).Distinct().Order())
        foreach (var warehouseId in new[] { transfer.SourceWarehouseId, transfer.DestinationWarehouseId }.Order())
            stock.EnsureActiveReferences(productId, warehouseId);
        foreach (var demand in lines.GroupBy(x => x.ProductId).Select(x => new { ProductId = x.Key, Quantity = x.Sum(y => y.Quantity) }))
        {
            var sourceBalance = stock.GetOrCreateBalance(demand.ProductId, transfer.SourceWarehouseId);
            if (!sourceWarehouse.AllowNegativeStock && !sourceBalance.CanIssue(demand.Quantity))
                throw new ConflictException("Insufficient available stock for transfer.");
        }

        var actorId = actor.UserId ?? throw new AuthenticationFailureException();
        var movements = new List<StockTransferMovementResponse>(lines.Length);
        foreach (var line in lines)
        {
            var sourceBalance = stock.GetOrCreateBalance(line.ProductId, transfer.SourceWarehouseId);
            var cost = sourceBalance.WeightedAverageCost;
            var outEntry = stock.Issue(line.ProductId, transfer.SourceWarehouseId, line.Quantity,
                "StockTransfer", transfer.Id, line.Id, StockMovementType.Transfer, actorId, InventoryPostingMetadata.CorrelationId);
            var inEntry = stock.Receive(line.ProductId, transfer.DestinationWarehouseId, line.Quantity, cost,
                "StockTransfer", transfer.Id, line.Id, StockMovementType.Transfer, actorId, InventoryPostingMetadata.CorrelationId);
            movements.Add(new(line.Id, line.ProductId, line.Quantity, cost, outEntry.Id, inEntry.Id));
        }
        transfer.Post(clock.UtcNow, actorId, lines.Length);
        SecurityAuditWriter.Add(db, actorId, "StockTransferPosted", "StockTransfer", transfer.Id.ToString(),
            "Draft", $"{transfer.SourceWarehouseId}|{transfer.DestinationWarehouseId}|{lines.Length}");
        await db.SaveChangesAsync(cancellationToken);
        return new(StockTransferResponseMapper.Map(db, transfer, lines), movements);
    }
}
