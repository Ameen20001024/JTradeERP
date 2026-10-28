using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.AcceptQuotation;
public static class AcceptQuotationEndPoint
{
    public static void Map(IEndpointRouteBuilder e)=>
        e.MapPost(
            "/quotations/{id:guid}/accept",
            async(Guid id,ISender s,CancellationToken ct) =>
                Results.Ok(await s.Send(new AcceptQuotationCommand(id),ct))
        ).RequirePermission("Billing.Quotation",Capability.CreateEdit);
}
