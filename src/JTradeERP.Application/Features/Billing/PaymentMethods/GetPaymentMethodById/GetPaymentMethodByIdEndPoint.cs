using JTradeERP.Application.Common.Security;

using JTradeERP.Domain.Security;

using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.GetPaymentMethodById;

public static class GetPaymentMethodByIdEndPoint

{
    public static void Map(IEndpointRouteBuilder e) => 
        
        e.MapGet(
            "/payment-methods/{id:guid}",
            (Guid id,ISender s,CancellationToken ct) => s.Send(new GetPaymentMethodByIdQuery(id),ct)
        ).RequirePermission("Admin.Configuration.Manage",Capability.View);
}
