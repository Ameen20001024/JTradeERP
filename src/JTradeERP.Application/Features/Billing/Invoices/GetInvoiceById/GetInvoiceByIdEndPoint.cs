using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.GetInvoiceById;

public static class GetInvoiceByIdEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapGet(
        "/invoices/{id:guid}",
        (Guid id,ISender s,CancellationToken ct) => 
    
        s.Send(new GetInvoiceByIdQuery(id),ct))
        
        .RequirePermission("Billing.Invoice",Capability.View);
}
