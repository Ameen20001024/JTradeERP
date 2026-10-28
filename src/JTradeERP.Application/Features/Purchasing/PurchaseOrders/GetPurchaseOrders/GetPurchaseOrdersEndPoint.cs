using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.GetPurchaseOrders;

public static class GetPurchaseOrdersEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapGet("/purchase-orders", async ([AsParameters] GetPurchaseOrdersQuery query, ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(query, ct)))
        .RequirePermission("Purchasing.PurchaseOrder", Capability.View);
}
