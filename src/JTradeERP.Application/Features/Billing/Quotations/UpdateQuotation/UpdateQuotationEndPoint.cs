using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.UpdateQuotation;

public static class UpdateQuotationEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapPut(
        "/quotations/{id:guid}",
        async(
            Guid id,
            UpdateQuotationCommand c,
            ISender s,
            CancellationToken ct) => 
        
        Results.Ok(await s.Send(c with{Id=id},ct))
    ).RequirePermission("Billing.Quotation",Capability.CreateEdit);
}
