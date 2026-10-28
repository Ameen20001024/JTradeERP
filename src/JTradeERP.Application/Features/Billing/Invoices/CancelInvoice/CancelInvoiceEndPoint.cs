using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.CancelInvoice;

public static class CancelInvoiceEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapPost(
        "/invoices/{id:guid}/cancel",
        async(Guid id,ISender s,CancellationToken ct) => 
        
        Results.Ok(await s.Send(new CancelInvoiceCommand(id),ct)))
        
        .RequirePermission("Billing.Invoice",Capability.Post)
        .RequirePermission("Billing.Cancel",Capability.Post);
}
