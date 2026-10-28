using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using JTradeERP.Domain.Billing;
using MediatR;

namespace JTradeERP.Application.Features.Billing.SalesOrders.PostSalesOrder;

public sealed class PostSalesOrderHandler(
    IApplicationDbContext db,
    IStockPostingService stock,
    ICurrentUserService actor,
    IClock clock) : IRequestHandler<PostSalesOrderCommand, PostSalesOrderResponse>
{
    public async Task<PostSalesOrderResponse> Handle(PostSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var order = db.SalesOrdersWithLines.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Sales order not found.");
        if (order.Status is SalesOrderStatus.Posted or SalesOrderStatus.PartiallyFulfilled or SalesOrderStatus.Fulfilled)
            return new(BillingResponseMapper.Map(order));
        if (order.Status != SalesOrderStatus.Draft)
            throw new ConflictException("Only a draft sales order can be posted.");

        var userId = actor.UserId ?? throw new AuthenticationFailureException();
        await using var transaction = await db.BeginTransactionAsync(cancellationToken);
        foreach (var line in order.Lines)
        {
            stock.EnsureActiveReferences(line.ProductId, order.WarehouseId);
            stock.Reserve(line.ProductId, order.WarehouseId, line.OrderedQuantity);
        }

        order.Post(clock.UtcNow, userId);
        SecurityAuditWriter.Add(db, userId, "SalesOrderPosted", "SalesOrder", order.Id.ToString(),
            SalesOrderStatus.Draft.ToString(), SalesOrderStatus.Posted.ToString());
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new PostSalesOrderResponse(BillingResponseMapper.Map(order));
    }
}
