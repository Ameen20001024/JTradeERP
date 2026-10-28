using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.SalesOrders.UpdateSalesOrder;

public static class UpdateSalesOrderEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        e.MapPut(
            "/sales-orders/{id:guid}",
            async(Guid id,UpdateSalesOrderCommand c,ISender s,CancellationToken ct) =>
            
                Results.Ok(await s.Send(c with{Id=id},ct))
        ).RequirePermission("Billing.SalesOrder",Capability.CreateEdit);
}
