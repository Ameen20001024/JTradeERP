using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Purchasing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.UpdateGoodsReceiptNote;

public sealed class UpdateGoodsReceiptNoteHandler(IApplicationDbContext db, ICurrentUserService actor, IClock clock)
    : IRequestHandler<UpdateGoodsReceiptNoteCommand, UpdateGoodsReceiptNoteResponse>
{
    public async Task<UpdateGoodsReceiptNoteResponse> Handle(UpdateGoodsReceiptNoteCommand request, CancellationToken cancellationToken)
    {
        var note = db.GoodsReceiptNotesWithLines.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Goods receipt note was not found.");
        PurchasingReferenceChecks.EnsureSupplierAndWarehouse(db, request.SupplierId, request.WarehouseId);
        var order = request.PurchaseOrderId.HasValue
            ? db.PurchaseOrdersWithLines.SingleOrDefault(x => x.Id == request.PurchaseOrderId.Value)
                ?? throw new NotFoundException("Purchase order was not found.")
            : null;
        PurchasingReferenceChecks.ValidateOrderRelationship(order, request.SupplierId, request.WarehouseId);
        var userId = actor.UserId ?? throw new AuthenticationFailureException();
        var oldValue = $"{note.PurchaseOrderId}|{note.SupplierId}|{note.WarehouseId}|{note.Lines.Count}";
        var previousLines = note.Lines.ToArray();
        try
        {
            var lines = request.Lines.Select(line => PurchasingReferenceChecks.CreateGoodsReceiptNoteLine(db, line, order));
            note.UpdateDraft(request.PurchaseOrderId, request.SupplierId, request.WarehouseId,
                request.ReceiptDate, lines, clock.UtcNow);
            foreach (var line in previousLines) db.Remove(line);
            foreach (var line in note.Lines) db.Add(line);
        }
        catch (InvalidOperationException exception)
        {
            throw new ConflictException(exception.Message);
        }
        catch (ArgumentException exception)
        {
            throw new ValidationFailureException(new Dictionary<string, string[]>
            {
                ["goodsReceiptNote"] = [exception.Message]
            });
        }

        SecurityAuditWriter.Add(db, userId, "GoodsReceiptNoteUpdated", "GoodsReceiptNote", note.Id.ToString(), oldValue,
            $"{note.PurchaseOrderId}|{note.SupplierId}|{note.WarehouseId}|{note.Lines.Count}");
        await db.SaveChangesAsync(cancellationToken);
        return new(PurchasingResponseMapper.Map(note));
    }
}
