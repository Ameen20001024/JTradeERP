using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Purchasing.Common;
using JTradeERP.Domain.Purchasing;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.UpdatePurchaseOrder;

public sealed class UpdatePurchaseOrderHandler(IApplicationDbContext db, ICurrentUserService actor, IClock clock)
    : IRequestHandler<UpdatePurchaseOrderCommand, UpdatePurchaseOrderResponse>
{
    public async Task<UpdatePurchaseOrderResponse> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = db.PurchaseOrdersWithLines.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Purchase order was not found.");
        PurchasingReferenceChecks.EnsureSupplierAndWarehouse(db, request.SupplierId, request.WarehouseId);
        var userId = actor.UserId ?? throw new AuthenticationFailureException();
        var oldValue = $"{order.SupplierId}|{order.WarehouseId}|{order.Lines.Count}|{order.GrandTotal}";
        var previousLines = order.Lines.ToArray();
        try
        {
            var lines = request.Lines.Select(line => PurchasingReferenceChecks.CreatePurchaseOrderLine(db, line));
            order.UpdateDraft(request.SupplierId, request.WarehouseId, request.OrderDate, request.ExpectedDate,
                userId, lines, clock.UtcNow);
            foreach (var line in previousLines) db.Remove(line);
            foreach (var line in order.Lines) db.Add(line);
        }
        catch (InvalidOperationException exception)
        {
            throw new ConflictException(exception.Message);
        }
        catch (ArgumentException exception)
        {
            throw new ValidationFailureException(new Dictionary<string, string[]>
            {
                ["purchaseOrder"] = [exception.Message]
            });
        }

        SecurityAuditWriter.Add(db, userId, "PurchaseOrderUpdated", "PurchaseOrder", order.Id.ToString(), oldValue,
            $"{order.SupplierId}|{order.WarehouseId}|{order.Lines.Count}|{order.GrandTotal}");
        await db.SaveChangesAsync(cancellationToken);
        return new(PurchasingResponseMapper.Map(order));
    }
}
