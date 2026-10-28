using JTradeERP.Application.Common.Security;

using JTradeERP.Domain.Security;

using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.CreatePaymentMethod;

public static class CreatePaymentMethodEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        
        e.MapPost(
            "/payment-methods",
            async(CreatePaymentMethodCommand c,ISender s,CancellationToken ct) => 
            
                Results.Ok(
                    await s.Send(c,ct))
        ).RequirePermission("Admin.Configuration.Manage",Capability.CreateEdit);
}
