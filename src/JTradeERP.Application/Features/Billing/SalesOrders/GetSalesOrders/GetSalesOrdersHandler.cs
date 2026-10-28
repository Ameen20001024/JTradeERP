using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Billing.Common;
using MediatR;
namespace JTradeERP.Application.Features.Billing.SalesOrders.GetSalesOrders;
public sealed class GetSalesOrdersHandler(IApplicationDbContext db):IRequestHandler<GetSalesOrdersQuery,GetSalesOrdersResponse>
{
    public Task<GetSalesOrdersResponse> Handle(GetSalesOrdersQuery r,CancellationToken ct)
    {
        var q = db.SalesOrdersWithLines.AsQueryable();
        
        if(r.CustomerId.HasValue)
            q = q.Where(x=>x.CustomerId==r.CustomerId);
        
        if(r.Status.HasValue) 
            q = q.Where(x=>x.Status==r.Status);
        
        var count = q.Count();
        
        var rows = q.OrderByDescending(x=>x.OrderDate)
            .Skip((r.Page-1)*r.PageSize)
            .Take(r.PageSize)
            .ToList()
            .Select(BillingResponseMapper.Map)
            .ToArray();
        return Task.FromResult(new GetSalesOrdersResponse(rows,count,r.Page,r.PageSize));
    }
}
