using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.GetPurchaseOrderById;

public static class GetPurchaseOrderByIdEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapGet("/purchase-orders/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new GetPurchaseOrderByIdQuery(id), ct)))
        .RequirePermission("Purchasing.PurchaseOrder", Capability.View);
}
