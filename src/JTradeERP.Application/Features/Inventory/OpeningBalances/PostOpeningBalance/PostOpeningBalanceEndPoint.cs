using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Inventory.OpeningBalances.PostOpeningBalance;

public static class PostOpeningBalanceEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapPost("/opening-balances", async (PostOpeningBalanceCommand command, ISender sender, CancellationToken cancellationToken) =>
            Results.Ok(await sender.Send(command, cancellationToken)))
        .RequirePermission("Stock.OpeningBalance", Capability.Full);
}
