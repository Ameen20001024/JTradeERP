using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.CancelQuotation;

public sealed class CancelQuotationHandler(IApplicationDbContext db,IClock clock):IRequestHandler<CancelQuotationCommand,CancelQuotationResponse>
{
    public async Task<CancelQuotationResponse> Handle(CancelQuotationCommand r,CancellationToken ct)
    {
        var q = db.QuotationsWithLines.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Quotation not found.");
        q.Cancel(clock.UtcNow);
        await db.SaveChangesAsync(ct);
        return new CancelQuotationResponse(BillingResponseMapper.Map(q));
    }
}
