using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.CopyQuotation;

public static class CopyQuotationEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        e.MapPost(
            "/quotations/{id:guid}/copy",
            async(Guid id,
                DateOnly quotationDate,
                DateOnly validityDate,
                ISender s,
                CancellationToken ct) =>
            
            Results.Ok(await s.Send(new CopyQuotationCommand(id,quotationDate,validityDate),ct))
        ).RequirePermission("Billing.Quotation",Capability.CreateEdit);
}
