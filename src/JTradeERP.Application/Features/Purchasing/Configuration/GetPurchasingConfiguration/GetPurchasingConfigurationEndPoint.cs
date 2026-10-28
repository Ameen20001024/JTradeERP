using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.Configuration.GetPurchasingConfiguration;

public static class GetPurchasingConfigurationEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapGet("/configuration/over-receipt", async (ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new GetPurchasingConfigurationQuery(), ct)))
        .RequirePermission("Admin.Configuration.Manage", Capability.View);
}
