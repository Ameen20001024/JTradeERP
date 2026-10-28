using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.UpdateQuotation;

public sealed class UpdateQuotationHandler(IApplicationDbContext db,IClock clock):IRequestHandler<UpdateQuotationCommand,UpdateQuotationResponse>
{
    public async Task<UpdateQuotationResponse> Handle(UpdateQuotationCommand r,CancellationToken ct)
    {
        var q = db.QuotationsWithLines.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Quotation not found.");
        
        BillingReferences.Customer(db, r.CustomerId);
        
        q.UpdateDraft(
            r.CustomerId,
            r.QuotationDate,
            r.ValidityDate,
            r.Lines.Select(x=> BillingReferences.QuotationLine(db,x)),clock.UtcNow);
        
        await db.SaveChangesAsync(ct);
        
        return new UpdateQuotationResponse(BillingResponseMapper.Map(q));
    }
}
