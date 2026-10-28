using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.PostInvoice;

public static class PostInvoiceEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapPost(
        "/invoices/{id:guid}/post",
        async(Guid id,ISender s,CancellationToken ct) => 
        
        Results.Ok(await s.Send(new PostInvoiceCommand(id),ct)))
        
        .RequirePermission("Billing.Invoice",Capability.Post)
        .RequirePermission("Billing.Post",Capability.Post);
}

