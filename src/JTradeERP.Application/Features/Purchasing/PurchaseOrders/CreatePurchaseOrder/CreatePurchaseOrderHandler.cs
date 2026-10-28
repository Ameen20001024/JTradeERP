using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Purchasing.Common;
using JTradeERP.Domain.Purchasing;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.CreatePurchaseOrder;

public sealed class CreatePurchaseOrderHandler(IApplicationDbContext db, ICurrentUserService actor, IClock clock)
    : IRequestHandler<CreatePurchaseOrderCommand, CreatePurchaseOrderResponse>
{
    public async Task<CreatePurchaseOrderResponse> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        PurchasingReferenceChecks.EnsureSupplierAndWarehouse(db, request.SupplierId, request.WarehouseId);
        var userId = actor.UserId ?? throw new AuthenticationFailureException();
        PurchaseOrder order;
        try
        {
            var lines = request.Lines.Select(line => PurchasingReferenceChecks.CreatePurchaseOrderLine(db, line));
            order = new PurchaseOrder(request.SupplierId, request.WarehouseId, request.OrderDate,
                request.ExpectedDate, userId, lines, clock.UtcNow);
        }
        catch (ArgumentException exception)
        {
            throw new ValidationFailureException(new Dictionary<string, string[]>
            {
                ["purchaseOrder"] = [exception.Message]
            });
        }

        db.Add(order);
        SecurityAuditWriter.Add(db, userId, "PurchaseOrderCreated", "PurchaseOrder", order.Id.ToString(), null,
            $"{order.SupplierId}|{order.WarehouseId}|{order.Lines.Count}|{order.GrandTotal}");
        await db.SaveChangesAsync(cancellationToken);
        return new(PurchasingResponseMapper.Map(order));
    }
}
