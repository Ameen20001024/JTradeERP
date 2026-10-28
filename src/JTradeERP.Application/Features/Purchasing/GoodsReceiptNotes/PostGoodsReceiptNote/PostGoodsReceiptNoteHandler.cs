using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Purchasing.Common;
using JTradeERP.Domain.Inventory;
using JTradeERP.Domain.Purchasing;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.PostGoodsReceiptNote;

public sealed class PostGoodsReceiptNoteHandler(IApplicationDbContext db, IStockPostingService stock,
    ICurrentUserService actor, IClock clock)
    : IRequestHandler<PostGoodsReceiptNoteCommand, PostGoodsReceiptNoteResponse>
{
    public async Task<PostGoodsReceiptNoteResponse> Handle(PostGoodsReceiptNoteCommand request, CancellationToken cancellationToken)
    {
        var note = db.GoodsReceiptNotesWithLines.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Goods receipt note was not found.");
        if (note.Status == GoodsReceiptNoteStatus.Posted)
            return new(PurchasingResponseMapper.Map(note));
        if (note.Status != GoodsReceiptNoteStatus.Draft)
            throw new ConflictException("Only draft goods receipt notes can be posted.");

        PurchasingReferenceChecks.EnsureSupplierAndWarehouse(db, note.SupplierId, note.WarehouseId);
        var order = note.PurchaseOrderId.HasValue
            ? db.PurchaseOrdersWithLines.SingleOrDefault(x => x.Id == note.PurchaseOrderId.Value)
                ?? throw new NotFoundException("Purchase order was not found.")
            : null;
        PurchasingReferenceChecks.ValidateOrderRelationship(order, note.SupplierId, note.WarehouseId);
        var allowOverReceipt = db.PurchasingConfigurations.SingleOrDefault(x => x.Id == PurchasingConfiguration.SingletonId)?.AllowOverReceipt
            ?? throw new ConflictException("Purchasing configuration is missing.");
        var actorId = actor.UserId ?? throw new AuthenticationFailureException();

        if (order is not null)
        {
            var quantities = note.Lines.GroupBy(x => x.PurchaseOrderLineId!.Value)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.ReceivedQuantity));
            foreach (var (lineId, quantity) in quantities)
            {
                var poLine = order.Lines.SingleOrDefault(x => x.Id == lineId)
                    ?? throw new ConflictException("A GRN line does not belong to the referenced purchase order.");
                if (!allowOverReceipt && poLine.ReceivedQuantity + quantity > poLine.OrderedQuantity)
                    throw new ConflictException("The GRN exceeds the remaining purchase order quantity.");
            }

            foreach (var line in note.Lines)
                stock.EnsureActiveReferences(line.ProductId, note.WarehouseId);
            foreach (var line in note.Lines)
                stock.Receive(line.ProductId, note.WarehouseId, line.ReceivedQuantity, line.UnitCost,
                    "GoodsReceiptNote", note.Id, line.Id, StockMovementType.Receipt, actorId,
                    InventoryPostingMetadata.CorrelationId);
            order.ApplyReceipt(quantities, clock.UtcNow);
        }
        else
        {
            foreach (var line in note.Lines)
                stock.Receive(line.ProductId, note.WarehouseId, line.ReceivedQuantity, line.UnitCost,
                    "GoodsReceiptNote", note.Id, line.Id, StockMovementType.Receipt, actorId,
                    InventoryPostingMetadata.CorrelationId);
        }

        note.Post(clock.UtcNow, actorId);
        SecurityAuditWriter.Add(db, actorId, "GoodsReceiptNotePosted", "GoodsReceiptNote", note.Id.ToString(),
            GoodsReceiptNoteStatus.Draft.ToString(), GoodsReceiptNoteStatus.Posted.ToString());
        if (order is not null)
            SecurityAuditWriter.Add(db, actorId, "PurchaseOrderReceiptProgressUpdated", "PurchaseOrder", order.Id.ToString(),
                null, string.Join(';', order.Lines.Select(x => $"{x.Id}:{x.ReceivedQuantity}")));
        await db.SaveChangesAsync(cancellationToken);
        return new(PurchasingResponseMapper.Map(note));
    }
}
