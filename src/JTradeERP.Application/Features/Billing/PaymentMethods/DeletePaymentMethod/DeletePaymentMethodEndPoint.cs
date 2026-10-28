using JTradeERP.Application.Common.Security;

using JTradeERP.Domain.Security;

using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.DeletePaymentMethod;

public static class DeletePaymentMethodEndPoint

{
    public static void Map(IEndpointRouteBuilder e) => 
        
        e.MapDelete(
            "/payment-methods/{id:guid}",
            async(Guid id,ISender s,CancellationToken ct) => 
                
                Results.Ok(await s.Send(new DeletePaymentMethodCommand(id),ct))

        ).RequirePermission("Admin.Configuration.Manage",Capability.CreateEdit);
}
