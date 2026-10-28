using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using JTradeERP.Domain.Billing;
using MediatR;

namespace JTradeERP.Application.Features.Billing.SalesOrders.CancelSalesOrder;

public sealed class CancelSalesOrderHandler(
    IApplicationDbContext db,
    IStockPostingService stock,
    ICurrentUserService actor,
    IClock clock) : IRequestHandler<CancelSalesOrderCommand, CancelSalesOrderResponse>
{
    public async Task<CancelSalesOrderResponse> Handle(CancelSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var order = db.SalesOrdersWithLines.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Sales order not found.");
        if (order.Status == SalesOrderStatus.Cancelled)
            return new(BillingResponseMapper.Map(order));

        var userId = actor.UserId ?? throw new AuthenticationFailureException();
        var oldStatus = order.Status.ToString();
        await using var transaction = await db.BeginTransactionAsync(cancellationToken);
        if (order.Status is SalesOrderStatus.Posted or SalesOrderStatus.PartiallyFulfilled)
        {
            foreach (var line in order.Lines)
                if (line.ReservedQuantity > 0)
                    stock.ReleaseReservation(line.ProductId, order.WarehouseId, line.ReservedQuantity);
        }

        order.Cancel(clock.UtcNow, userId);
        SecurityAuditWriter.Add(db, userId, "SalesOrderCancelled", "SalesOrder", order.Id.ToString(),
            oldStatus, SalesOrderStatus.Cancelled.ToString());
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new CancelSalesOrderResponse(BillingResponseMapper.Map(order));
    }
}
