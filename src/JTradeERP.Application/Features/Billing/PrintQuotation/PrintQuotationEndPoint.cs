using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using JTradeERP.Application.Features.Billing.Quotations.GetQuotationById;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.PrintQuotation;

public static class PrintQuotationEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => 
        
        endpoints.MapGet(
            "/quotations/{id:guid}/print",
            async (Guid id, ISender sender, 
            CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetQuotationByIdQuery(id), cancellationToken);

            return Results.Content(BillingPrintDocument.Quotation(response.Quotation), "text/html; charset=utf-8");
        }).RequirePermission("Billing.Quotation", Capability.View);
}
