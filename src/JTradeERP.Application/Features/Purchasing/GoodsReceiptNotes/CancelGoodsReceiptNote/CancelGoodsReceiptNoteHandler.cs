using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Purchasing.Common;
using JTradeERP.Domain.Inventory;
using JTradeERP.Domain.Purchasing;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.CancelGoodsReceiptNote;

public sealed class CancelGoodsReceiptNoteHandler(IApplicationDbContext db, IStockPostingService stock,
    ICurrentUserService actor, IClock clock)
    : IRequestHandler<CancelGoodsReceiptNoteCommand, CancelGoodsReceiptNoteResponse>
{
    public async Task<CancelGoodsReceiptNoteResponse> Handle(CancelGoodsReceiptNoteCommand request, CancellationToken cancellationToken)
    {
        var note = db.GoodsReceiptNotesWithLines.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Goods receipt note was not found.");
        if (note.Status == GoodsReceiptNoteStatus.Cancelled)
            return new(PurchasingResponseMapper.Map(note));
        if (note.Status != GoodsReceiptNoteStatus.Posted)
            throw new ConflictException("Only posted goods receipt notes can be cancelled.");

        var actorId = actor.UserId ?? throw new AuthenticationFailureException();
        foreach (var line in note.Lines)
        {
            var original = db.StockLedgerEntries.SingleOrDefault(x => x.SourceDocumentType == "GoodsReceiptNote"
                && x.SourceDocumentId == note.Id && x.SourceLineId == line.Id
                && x.MovementType == StockMovementType.Receipt && x.Direction == StockMovementDirection.In)
                ?? throw new ConflictException("The original inventory receipt entry was not found.");
            stock.Reverse(original, note.Id, "GoodsReceiptNoteCancellation", actorId,
                InventoryPostingMetadata.CorrelationId);
        }

        PurchaseOrder? order = null;
        if (note.PurchaseOrderId.HasValue)
        {
            order = db.PurchaseOrdersWithLines.SingleOrDefault(x => x.Id == note.PurchaseOrderId.Value)
                ?? throw new NotFoundException("Purchase order was not found.");
            var quantities = note.Lines.GroupBy(x => x.PurchaseOrderLineId!.Value)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.ReceivedQuantity));
            order.ReverseReceipt(quantities, clock.UtcNow);
        }

        note.Cancel(clock.UtcNow, actorId);
        SecurityAuditWriter.Add(db, actorId, "GoodsReceiptNoteCancelled", "GoodsReceiptNote", note.Id.ToString(),
            GoodsReceiptNoteStatus.Posted.ToString(), GoodsReceiptNoteStatus.Cancelled.ToString());
        if (order is not null)
            SecurityAuditWriter.Add(db, actorId, "PurchaseOrderReceiptProgressUpdated", "PurchaseOrder", order.Id.ToString(),
                null, string.Join(';', order.Lines.Select(x => $"{x.Id}:{x.ReceivedQuantity}")));
        await db.SaveChangesAsync(cancellationToken);
        return new(PurchasingResponseMapper.Map(note));
    }
}
