using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.RejectQuotation;

public sealed class RejectQuotationHandler(IApplicationDbContext db,IClock clock):IRequestHandler<RejectQuotationCommand,RejectQuotationResponse>
{
    public async Task<RejectQuotationResponse> Handle(RejectQuotationCommand r,CancellationToken ct)
    {
        var q = db.QuotationsWithLines.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Quotation not found.");

        q.Reject(clock.UtcNow);

        await db.SaveChangesAsync(ct);
        
        return new RejectQuotationResponse(BillingResponseMapper.Map(q));
    }
}
