using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.SalesOrders.PostSalesOrder;

public static class PostSalesOrderEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        e.MapPost(
            "/sales-orders/{id:guid}/post",
            async(Guid id,ISender s,CancellationToken ct) => 
            
            Results.Ok(await s.Send(new PostSalesOrderCommand(id),ct))
        ).RequirePermission("Billing.SalesOrder",Capability.Post).RequirePermission("Billing.Post",Capability.Post);
}
