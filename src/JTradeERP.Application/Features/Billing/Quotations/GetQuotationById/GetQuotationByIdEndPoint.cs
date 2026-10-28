using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.GetQuotationById;

public static class GetQuotationByIdEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        e.MapGet(
            "/quotations/{id:guid}",
            (Guid id,ISender s,CancellationToken ct) => 
                s.Send(new GetQuotationByIdQuery(id),ct)
        ).RequirePermission("Billing.Quotation",Capability.View);
}
