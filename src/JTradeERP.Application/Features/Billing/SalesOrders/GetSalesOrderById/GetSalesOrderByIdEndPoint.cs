using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.SalesOrders.GetSalesOrderById;
public static class GetSalesOrderByIdEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        e.MapGet(
            "/sales-orders/{id:guid}",
            (Guid id,ISender s,CancellationToken ct) => 

            s.Send(new GetSalesOrderByIdQuery(id),ct)
        ).RequirePermission("Billing.SalesOrder",Capability.View);
}
