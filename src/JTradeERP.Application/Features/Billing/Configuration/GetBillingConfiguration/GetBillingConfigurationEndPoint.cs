using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Configuration.GetBillingConfiguration;

public static class GetBillingConfigurationEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        e.MapGet(
            "/configuration",
            (ISender s,CancellationToken ct) => 
            
            s.Send(new GetBillingConfigurationQuery(),ct)
        ).RequirePermission("Admin.Configuration.Manage",Capability.View);
}
