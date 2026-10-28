using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Billing;
using JTradeERP.Domain.Security;
using MediatR;namespace JTradeERP.Application.Features.Billing.Quotations.GetQuotations;
public static class GetQuotationsEndPoint
{
    public static void Map(IEndpointRouteBuilder e)=>
        e.MapGet(
            "/quotations",
            (Guid? customerId,
                QuotationStatus? status,
                int page,
                int pageSize,
                ISender s,
                CancellationToken ct)=>
            
            s.Send(new GetQuotationsQuery(customerId,
                status,
                page==0?1:page,
                pageSize==0?50:pageSize),ct)
        ).RequirePermission("Billing.Quotation",Capability.View);
}
