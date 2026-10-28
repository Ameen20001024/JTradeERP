using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.ExpireQuotation;

public static class ExpireQuotationEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => 
        e.MapPost(
            "/quotations/{id:guid}/expire",
            async(Guid id,ISender s,CancellationToken ct) => 
                Results.Ok(await s.Send(new ExpireQuotationCommand(id),ct))
        ).RequirePermission("Billing.Quotation",Capability.Post);
}
