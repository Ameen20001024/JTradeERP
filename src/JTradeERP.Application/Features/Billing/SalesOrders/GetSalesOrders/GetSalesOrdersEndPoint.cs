using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Billing;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.SalesOrders.GetSalesOrders;

public static class GetSalesOrdersEndPoint
{
    public static void Map(IEndpointRouteBuilder e) =>
        e.MapGet(
            "/sales-orders",
            (Guid? customerId,SalesOrderStatus? status,int page,int pageSize,ISender s,CancellationToken ct)=>
            
            s.Send(new GetSalesOrdersQuery(
                customerId,
                status,page==0?1:page,
                pageSize==0?50:pageSize), ct)
        ).RequirePermission("Billing.SalesOrder",Capability.View);
}
