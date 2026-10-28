using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Inventory;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.CreateStockTransfer;
public sealed class CreateStockTransferHandler(IApplicationDbContext db, ICurrentUserService actor)
    : IRequestHandler<CreateStockTransferCommand, CreateStockTransferResponse>
{
    public async Task<CreateStockTransferResponse> Handle(CreateStockTransferCommand request, CancellationToken cancellationToken)
    {
        var transfer = new StockTransfer(request.SourceWarehouseId, request.DestinationWarehouseId);
        transfer.Update(request.SourceWarehouseId, request.DestinationWarehouseId,
            request.Lines.Select(x => (x.ProductId, x.Quantity)).ToArray());
        db.Add(transfer);
        SecurityAuditWriter.Add(db, actor.UserId, "StockTransferCreated", "StockTransfer", transfer.Id.ToString(), null,
            $"{transfer.SourceWarehouseId}|{transfer.DestinationWarehouseId}|{transfer.Lines.Count}");
        await db.SaveChangesAsync(cancellationToken);
        var warehouses = db.Warehouses.Where(x => x.Id == transfer.SourceWarehouseId || x.Id == transfer.DestinationWarehouseId)
            .ToDictionary(x => x.Id);
        return new(new(transfer.Id, transfer.SourceWarehouseId, warehouses[transfer.SourceWarehouseId].WarehouseCode,
            transfer.DestinationWarehouseId, warehouses[transfer.DestinationWarehouseId].WarehouseCode,
            transfer.Status.ToString(), transfer.PostedAt, transfer.Lines.Select(x => new StockTransferLineResponse(
                x.Id, x.ProductId, db.Products.Single(p => p.Id == x.ProductId).ProductCode,
                db.Products.Single(p => p.Id == x.ProductId).Name, x.Quantity)).ToArray()));
    }
}
