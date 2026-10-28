using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.GetInvoices;

public static class GetInvoicesEndPoint
{
    public static void Map(IEndpointRouteBuilder e) => e.MapGet(
        "/invoices", (
            Guid? customerId,
            JTradeERP.Domain.Billing.InvoiceStatus? status,
            DateOnly? from,
            DateOnly? to,
            int page,
            int pageSize,
            ISender s,
            CancellationToken ct) =>
        
        s.Send(new GetInvoicesQuery(customerId,status,from,to,page==0?1:page,pageSize==0?50:pageSize),ct))
        
        .RequirePermission("Billing.Invoice",Capability.View);
}
