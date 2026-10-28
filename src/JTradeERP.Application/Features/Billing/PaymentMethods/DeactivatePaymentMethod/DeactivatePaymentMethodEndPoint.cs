using JTradeERP.Application.Common.Security;

using JTradeERP.Domain.Security;

using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.DeactivatePaymentMethod;

public static class DeactivatePaymentMethodEndPoint
{
    public static void Map(IEndpointRouteBuilder e) =>
        
        e.MapPost(
            "/payment-methods/{id:guid}/deactivate",
            async(Guid id,ISender s,CancellationToken ct) => 
            
                Results.Ok(await s.Send( new DeactivatePaymentMethodCommand(id),ct))

        ).RequirePermission("Admin.Configuration.Manage",Capability.CreateEdit);

}
