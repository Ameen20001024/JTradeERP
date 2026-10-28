using JTradeERP.Application.Common.Security;

using JTradeERP.Domain.Security;

using MediatR;

namespace JTradeERP.Application.Features.Billing.Payments.GetPaymentById;

public static class GetPaymentByIdEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        e.MapGet(
            "/payments/{id:guid}",
            (Guid id,ISender s,CancellationToken ct) => 
            s.Send(new GetPaymentByIdQuery(id),ct))
        .RequirePermission("Billing.Payment",Capability.View);
}
