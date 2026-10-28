using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.SalesOrders.CancelSalesOrder;

public static class CancelSalesOrderEndPoint
{
    public static void Map(IEndpointRouteBuilder e) =>
        e.MapPost(
            "/sales-orders/{id:guid}/cancel",
            async(Guid id,ISender s,CancellationToken ct) =>
            
            Results.Ok(await s.Send(new CancelSalesOrderCommand(id),ct))
        ).RequirePermission("Billing.SalesOrder",Capability.Post).RequirePermission("Billing.Cancel",Capability.Post);
}
