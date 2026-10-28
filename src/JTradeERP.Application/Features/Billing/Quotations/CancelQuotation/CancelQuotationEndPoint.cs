using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.CancelQuotation;
public static class CancelQuotationEndPoint
{
    public static void Map(IEndpointRouteBuilder e) =>
        e.MapPost(
            "/quotations/{id:guid}/cancel",
            async(Guid id,ISender s,CancellationToken ct) => 
                Results.Ok(await s.Send(new CancelQuotationCommand(id),ct))
        ).RequirePermission("Billing.Quotation",Capability.Post)
        .RequirePermission("Billing.Cancel",Capability.Post);
}
