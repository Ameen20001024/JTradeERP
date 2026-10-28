using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using JTradeERP.Application.Features.Billing.Invoices.GetInvoiceById;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.PrintInvoice;

public static class PrintInvoiceEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints) => 
        endpoints.MapGet("/invoices/{id:guid}/print", 
            async (Guid id,
                ISender sender, 
                CancellationToken cancellationToken) =>
        {
            var response = await sender.Send(new GetInvoiceByIdQuery(id), cancellationToken);
            return Results.Content(BillingPrintDocument.Invoice(response.Invoice), "text/html; charset=utf-8");
        }).RequirePermission("Billing.Invoice", Capability.View);
}
