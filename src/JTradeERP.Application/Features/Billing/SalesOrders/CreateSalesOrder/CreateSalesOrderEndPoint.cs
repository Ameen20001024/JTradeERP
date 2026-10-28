using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.SalesOrders.CreateSalesOrder;

public static class CreateSalesOrderEndPoint
{
    public static void Map(IEndpointRouteBuilder e) =>
        e.MapPost(
            "/sales-orders",
            async(CreateSalesOrderCommand c,ISender s,CancellationToken ct) =>
            {
                var r=await s.Send(c,ct);
                
                return Results.Created($"/api/billing/sales-orders/{r.SalesOrder.Id}",r);
            }
        ).RequirePermission("Billing.SalesOrder",Capability.CreateEdit);
}
