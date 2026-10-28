using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.CreatePurchaseOrder;

public static class CreatePurchaseOrderEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapPost("/purchase-orders", async (CreatePurchaseOrderCommand command, ISender sender, CancellationToken ct) =>
        {
            var response = await sender.Send(command, ct);
            return Results.Created($"/api/purchasing/purchase-orders/{response.PurchaseOrder.Id}", response);
        })
        .RequirePermission("Purchasing.PurchaseOrder", Capability.CreateEdit);
}
