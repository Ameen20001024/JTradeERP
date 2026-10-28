using JTradeERP.Application.Common.Security;

using JTradeERP.Domain.Security;

using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.GetPaymentMethods;

public static class GetPaymentMethodsEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        
        e.MapGet("/payment-methods",
            (bool includeInactive,ISender s,CancellationToken ct) => 
            
                s.Send(new GetPaymentMethodsQuery(includeInactive),ct)
            
        ).RequirePermission("Admin.Configuration.Manage",Capability.View);
}
