using JTradeERP.Application.Common.Security;

using JTradeERP.Domain.Security;

using MediatR;

namespace JTradeERP.Application.Features.Billing.Payments.ReversePayment;

public static class ReversePaymentEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        e.MapPost(
            "/payments/{id:guid}/reverse",
            async(Guid id,ISender s,CancellationToken ct) => 
                Results.Ok(await s.Send(new ReversePaymentCommand(id), ct))
    ).RequirePermission("Billing.Payment",Capability.Post);
}
