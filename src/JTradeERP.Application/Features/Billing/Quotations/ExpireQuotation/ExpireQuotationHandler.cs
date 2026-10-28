using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.ExpireQuotation;

public sealed class ExpireQuotationHandler(
    IApplicationDbContext db,
    IClock clock):IRequestHandler<ExpireQuotationCommand,ExpireQuotationResponse>

{
    public async Task<ExpireQuotationResponse> Handle(ExpireQuotationCommand r,CancellationToken ct)
    {
        var q=db.QuotationsWithLines.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Quotation not found.");
        q.Expire(DateOnly.FromDateTime(clock.UtcNow.UtcDateTime),clock.UtcNow);
        await db.SaveChangesAsync(ct);
        return new ExpireQuotationResponse(BillingResponseMapper.Map(q));
    }
}
