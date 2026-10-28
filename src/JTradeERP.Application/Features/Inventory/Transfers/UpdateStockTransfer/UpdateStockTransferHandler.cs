using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Inventory;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.UpdateStockTransfer;
public sealed class UpdateStockTransferHandler(IApplicationDbContext db, ICurrentUserService actor)
    : IRequestHandler<UpdateStockTransferCommand, UpdateStockTransferResponse>
{
    public async Task<UpdateStockTransferResponse> Handle(UpdateStockTransferCommand request, CancellationToken cancellationToken)
    {
        var transfer = db.StockTransfers.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Stock transfer was not found.");
        if (transfer.Status != InventoryDocumentStatus.Draft)
            throw new ConflictException("Only draft transfers can be edited.");
        var priorLines = db.StockTransferLines.Where(x => x.StockTransferId == transfer.Id).ToArray();
        foreach (var prior in priorLines) db.Remove(prior);
        transfer.Update(request.SourceWarehouseId, request.DestinationWarehouseId,
            request.Lines.Select(x => (x.ProductId, x.Quantity)).ToArray());
        // These have client-generated keys and are new rows; explicitly mark them Added because
        // relationship fix-up may otherwise classify a replacement as an update to a deleted row.
        foreach (var line in transfer.Lines) db.Add(line);
        SecurityAuditWriter.Add(db, actor.UserId, "StockTransferUpdated", "StockTransfer", transfer.Id.ToString(),
            $"{transfer.SourceWarehouseId}|{transfer.DestinationWarehouseId}|{priorLines.Length}",
            $"{request.SourceWarehouseId}|{request.DestinationWarehouseId}|{request.Lines.Count}");
        await db.SaveChangesAsync(cancellationToken);
        return new(StockTransferResponseMapper.Map(db, transfer,
            db.StockTransferLines.Where(x => x.StockTransferId == transfer.Id).ToArray()));
    }
}
