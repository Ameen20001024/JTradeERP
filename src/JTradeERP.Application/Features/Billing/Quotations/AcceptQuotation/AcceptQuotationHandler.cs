using JTradeERP.Application.Common.Exceptions;

using JTradeERP.Application.Common.Persistence;

using JTradeERP.Application.Common.Security;

using JTradeERP.Application.Features.Billing.Common;

using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.AcceptQuotation;

public sealed class AcceptQuotationHandler(IApplicationDbContext db,IClock clock):IRequestHandler<AcceptQuotationCommand,AcceptQuotationResponse>
{
    public async Task<AcceptQuotationResponse> Handle(AcceptQuotationCommand r,CancellationToken ct)
    {
        var q=db.QuotationsWithLines.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Quotation not found.");
        q.Accept(clock.UtcNow);
        await db.SaveChangesAsync(ct);
        return new AcceptQuotationResponse(BillingResponseMapper.Map(q));
    }
}
