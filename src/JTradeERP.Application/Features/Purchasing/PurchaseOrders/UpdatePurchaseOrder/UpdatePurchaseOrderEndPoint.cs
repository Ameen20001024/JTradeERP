using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.UpdatePurchaseOrder;

public static class UpdatePurchaseOrderEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapPut("/purchase-orders/{id:guid}", (Guid id, UpdatePurchaseOrderCommand command, ISender sender, CancellationToken ct) =>
            sender.Send(command with { Id = id }, ct))
        .RequirePermission("Purchasing.PurchaseOrder", Capability.CreateEdit);
}
