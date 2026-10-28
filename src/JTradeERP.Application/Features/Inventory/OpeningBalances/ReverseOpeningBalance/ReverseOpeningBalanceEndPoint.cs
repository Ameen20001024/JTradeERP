using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.OpeningBalances.ReverseOpeningBalance;
public static class ReverseOpeningBalanceEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints.MapPost("/opening-balances/{id:guid}/reverse",
        async (Guid id, ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new ReverseOpeningBalanceCommand(id), ct)))
        .RequirePermission("Stock.Post", Capability.Post)
        .RequirePermission("Stock.OpeningBalance", Capability.Full);
}
