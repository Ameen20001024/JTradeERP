using JTradeERP.Application.Common.Security;

using JTradeERP.Domain.Billing;

using JTradeERP.Domain.Security;

using MediatR;

namespace JTradeERP.Application.Features.Billing.Payments.GetPayments;

public static class GetPaymentsEndPoint
{
    public static void Map(IEndpointRouteBuilder e) =>
        
        e.MapGet(
            "/payments",
            (PaymentStatus? status,
                DateOnly? from,
                DateOnly? to,
                int page,
                int pageSize,
                ISender s,
                CancellationToken ct) =>
            
            s.Send(new GetPaymentsQuery(
                status,
                from,
                to,
                page==0?1:page,
                pageSize==0?50:pageSize),
                ct)
        ).RequirePermission("Billing.Payment",Capability.View);
}
