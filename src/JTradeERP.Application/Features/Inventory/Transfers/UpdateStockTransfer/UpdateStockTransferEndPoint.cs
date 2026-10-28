using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.UpdateStockTransfer;
public static class UpdateStockTransferEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints.MapPut("/transfers/{id:guid}",
        async (Guid id, UpdateStockTransferCommand command, ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(command with { Id = id }, ct)))
        .RequirePermission("Stock.Transfer", Capability.CreateEdit);
}
