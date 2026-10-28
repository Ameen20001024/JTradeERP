using JTradeERP.Application.Common.Security;using JTradeERP.Domain.Security;using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.CreateQuotation;
public static class CreateQuotationEndPoint
{
    public static void Map(IEndpointRouteBuilder e) =>
        e.MapPost(
            "/quotations",
            async(
                CreateQuotationCommand c,
                ISender s,
                CancellationToken ct)=>
            {
                var r = await s.Send(c,ct);
                return Results.Created($"/api/billing/quotations/{r.Quotation.Id}",r);
            }
        ).RequirePermission("Billing.Quotation",Capability.CreateEdit);
}
