using JTradeERP.Application.Common.Security;

using JTradeERP.Domain.Security;

using MediatR;

namespace JTradeERP.Application.Features.Billing.Payments.RecordPayment;

public static class RecordPaymentEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        e.MapPost(
            "/payments",
            async(RecordPaymentCommand c,ISender s,CancellationToken ct) =>
                Results.Ok(await s.Send(c,ct))
        ).RequirePermission("Billing.Payment",Capability.CreateEdit);
}
