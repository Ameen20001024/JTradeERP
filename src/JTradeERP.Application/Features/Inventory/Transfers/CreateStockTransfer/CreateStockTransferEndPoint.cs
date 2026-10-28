using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.Transfers.CreateStockTransfer;
public static class CreateStockTransferEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints.MapPost("/transfers",
        async (CreateStockTransferCommand command, ISender sender, CancellationToken ct) =>
            Results.Created("/api/inventory/transfers", await sender.Send(command, ct)))
        .RequirePermission("Stock.Transfer", Capability.CreateEdit);
}
