using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Billing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Invoices.GetInvoices;

public sealed class GetInvoicesHandler(IApplicationDbContext db):IRequestHandler<GetInvoicesQuery,GetInvoicesResponse>
{
    public Task<GetInvoicesResponse> Handle(GetInvoicesQuery r,CancellationToken ct)
    {
        var q = db.InvoicesWithLines.AsQueryable();
        
        if(r.CustomerId.HasValue)
            q = q.Where(x=>x.CustomerId==r.CustomerId);
        
        if(r.Status.HasValue)
            q = q.Where(x=>x.Status==r.Status);
        
        if(r.From.HasValue)
            q = q.Where(x=>x.InvoiceDate>=r.From);
        
        if(r.To.HasValue)
            q = q.Where(x=>x.InvoiceDate<=r.To);
        
        var count = q.Count();
        
        var rows = q.OrderByDescending(x=>x.InvoiceDate)
            .ThenBy(x=>x.InvoiceNumber)
            .Skip((r.Page-1)*r.PageSize)
            .Take(r.PageSize)
            .ToList()
            .Select(BillingResponseMapper.Map)
            .ToArray();
        
        return Task.FromResult(new GetInvoicesResponse(rows,count,r.Page,r.PageSize));
    }
}
