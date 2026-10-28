using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Purchasing.Common;
using JTradeERP.Domain.Purchasing;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.GoodsReceiptNotes.CreateGoodsReceiptNote;

public sealed class CreateGoodsReceiptNoteHandler(IApplicationDbContext db, ICurrentUserService actor, IClock clock)
    : IRequestHandler<CreateGoodsReceiptNoteCommand, CreateGoodsReceiptNoteResponse>
{
    public async Task<CreateGoodsReceiptNoteResponse> Handle(CreateGoodsReceiptNoteCommand request, CancellationToken cancellationToken)
    {
        PurchasingReferenceChecks.EnsureSupplierAndWarehouse(db, request.SupplierId, request.WarehouseId);
        var order = request.PurchaseOrderId.HasValue
            ? db.PurchaseOrdersWithLines.SingleOrDefault(x => x.Id == request.PurchaseOrderId.Value)
                ?? throw new NotFoundException("Purchase order was not found.")
            : null;
        PurchasingReferenceChecks.ValidateOrderRelationship(order, request.SupplierId, request.WarehouseId);
        var userId = actor.UserId ?? throw new AuthenticationFailureException();
        GoodsReceiptNote note;
        try
        {
            var lines = request.Lines.Select(line => PurchasingReferenceChecks.CreateGoodsReceiptNoteLine(db, line, order));
            note = new GoodsReceiptNote(request.PurchaseOrderId, request.SupplierId,
                request.WarehouseId, request.ReceiptDate, lines, clock.UtcNow);
        }
        catch (ArgumentException exception)
        {
            throw new ValidationFailureException(new Dictionary<string, string[]>
            {
                ["goodsReceiptNote"] = [exception.Message]
            });
        }

        db.Add(note);
        SecurityAuditWriter.Add(db, userId, "GoodsReceiptNoteCreated", "GoodsReceiptNote", note.Id.ToString(), null,
            $"{note.PurchaseOrderId}|{note.SupplierId}|{note.WarehouseId}|{note.Lines.Count}");
        await db.SaveChangesAsync(cancellationToken);
        return new(PurchasingResponseMapper.Map(note));
    }

}
