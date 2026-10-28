using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.PostStockTransfer;
public static class PostStockTransferEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints.MapPost("/transfers/{id:guid}/post",
        async (Guid id, ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new PostStockTransferCommand(id), ct)))
        .RequirePermission("Stock.Post", Capability.Post)
        .RequirePermission("Stock.Transfer", Capability.CreateEdit);
}
