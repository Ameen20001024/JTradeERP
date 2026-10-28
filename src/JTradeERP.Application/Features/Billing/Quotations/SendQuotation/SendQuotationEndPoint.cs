using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.SendQuotation;

public static class SendQuotationEndPoint
{
    public static void Map(IEndpointRouteBuilder e) =>
        e.MapPost(
            "/quotations/{id:guid}/send",
            async(Guid id,ISender s,CancellationToken ct)=>
            
            Results.Ok(await s.Send(new SendQuotationCommand(id),ct))
        ).RequirePermission("Billing.Quotation",Capability.CreateEdit);
}
