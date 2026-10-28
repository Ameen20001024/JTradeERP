using JTradeERP.Application.Common.Persistence;

using MediatR;

namespace JTradeERP.Application.Features.Billing.Payments.GetPayments;

public sealed class GetPaymentsHandler(IApplicationDbContext db):IRequestHandler<GetPaymentsQuery,GetPaymentsResponse>
{
    public Task<GetPaymentsResponse> Handle(GetPaymentsQuery r,CancellationToken ct)
    {
        var q=db.PaymentsWithAllocations.AsQueryable();

        if(r.Status.HasValue)
            q = q.Where(x=>x.Status==r.Status);
        
        if(r.From.HasValue)
            q = q.Where(x=>x.PaymentDate>=r.From);
        
        if(r.To.HasValue)
            q=q.Where(x=>x.PaymentDate<=r.To);
        
        var count= q.Count();
        var rows=q.OrderByDescending(x=>x.PaymentDate)
            .Skip((r.Page-1)*r.PageSize)
            .Take(r.PageSize)
            .ToList()
            .Select(x=>new GetPaymentItemResponse(
                x.Id,
                x.PaymentDate,
                x.PaymentMethodId,
                x.Amount,
                x.ReferenceNumber,
                x.Status.ToString(),
                x.Allocations.Select(a=>new PaymentAllocationItemResponse(a.InvoiceId,a.Amount)).ToArray()))
            .ToArray();
        
        return Task.FromResult(new GetPaymentsResponse(rows,count,r.Page,r.PageSize));
    }
}
