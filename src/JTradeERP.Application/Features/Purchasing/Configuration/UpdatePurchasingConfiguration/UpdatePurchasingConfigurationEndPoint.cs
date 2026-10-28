using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Purchasing.Configuration.UpdatePurchasingConfiguration;

public static class UpdatePurchasingConfigurationEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => endpoints
        .MapPut("/configuration/over-receipt", (UpdatePurchasingConfigurationCommand command, ISender sender, CancellationToken ct) =>
            sender.Send(command, ct))
        .RequirePermission("Admin.Configuration.Manage", Capability.Full);
}
