using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.CreateInvoice;

public static class CreateInvoiceEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapPost(
        "/invoices",
        async(CreateInvoiceCommand c,ISender s,CancellationToken ct) => 
        {
            var r=await s.Send(c,ct);
            return Results.Created($"/api/billing/invoices/{r.Invoice.Id}",r);
        })
        
        .RequirePermission("Billing.Invoice",Capability.CreateEdit);}
