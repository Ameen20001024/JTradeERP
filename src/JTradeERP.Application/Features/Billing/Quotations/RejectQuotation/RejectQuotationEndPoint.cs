using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.RejectQuotation;

public static class RejectQuotationEndPoint
{
    public static void Map(IEndpointRouteBuilder e) =>
        e.MapPost(
            "/quotations/{id:guid}/reject",
            async(Guid id,ISender s,CancellationToken ct) =>
            
            Results.Ok(await s.Send(new RejectQuotationCommand(id),ct))
        ).RequirePermission("Billing.Quotation",Capability.CreateEdit);
}
