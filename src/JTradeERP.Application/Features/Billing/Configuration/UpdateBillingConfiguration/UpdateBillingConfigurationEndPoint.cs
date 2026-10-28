using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Configuration.UpdateBillingConfiguration;

public static class UpdateBillingConfigurationEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        e.MapPut(
            "/configuration",
            (UpdateBillingConfigurationCommand c,ISender s,CancellationToken ct) => 
            
            s.Send(c,ct))
        .RequirePermission("Admin.Configuration.Manage",Capability.CreateEdit);
}
