using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.UpdateInvoice;

public static class UpdateInvoiceEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapPut(
        "/invoices/{id:guid}",
        async(Guid id,UpdateInvoiceCommand c,ISender s,CancellationToken ct) => 
        
        Results.Ok(await s.Send(c with{Id=id},ct)))
        
        .RequirePermission("Billing.Invoice",Capability.CreateEdit);
}
