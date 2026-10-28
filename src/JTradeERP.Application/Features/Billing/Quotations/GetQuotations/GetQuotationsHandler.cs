using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Billing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.GetQuotations;

public sealed class GetQuotationsHandler(IApplicationDbContext db):IRequestHandler<GetQuotationsQuery,GetQuotationsResponse>
{
    public Task<GetQuotationsResponse> Handle(GetQuotationsQuery r,CancellationToken ct)
    {
        var q = db.QuotationsWithLines.AsQueryable();
        
        if(r.CustomerId.HasValue)
            q = q.Where(x=>x.CustomerId==r.CustomerId);
        
        if(r.Status.HasValue)
            q = q.Where(x=>x.Status==r.Status);
        
        var count = q.Count();
        
        var rows = q.OrderByDescending(x=>x.QuotationDate)
            .Skip((r.Page-1)*r.PageSize)
            .Take(r.PageSize)
            .ToList()
            .Select(BillingResponseMapper.Map)
            .ToArray();
        
        return Task.FromResult(new GetQuotationsResponse(rows,count,r.Page,r.PageSize));
    }
}
