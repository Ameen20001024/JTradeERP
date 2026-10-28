using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.GetStockTransfer;
public static class GetStockTransferEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints.MapGet("/transfers/{id:guid}",
        async (Guid id, ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetStockTransferQuery(id), ct)))
        .RequirePermission("Stock.View", Capability.View);
}
