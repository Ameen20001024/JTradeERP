using JTradeERP.Application.Common.Security;

using JTradeERP.Domain.Security;

using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.UpdatePaymentMethod;

public static class UpdatePaymentMethodEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        
        e.MapPut(
            "/payment-methods/{id:guid}",
            async(Guid id,UpdatePaymentMethodCommand c,ISender s,CancellationToken ct) => 
                
                Results.Ok(await s.Send(c with{Id=id},ct))
        ).RequirePermission("Admin.Configuration.Manage",Capability.CreateEdit);
}
