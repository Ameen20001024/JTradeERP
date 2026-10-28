using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using JTradeERP.Domain.Billing;
using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.CreateQuotation;
public sealed class CreateQuotationHandler(IApplicationDbContext db,IClock clock):IRequestHandler<CreateQuotationCommand,CreateQuotationResponse>
{
    public async Task<CreateQuotationResponse> Handle(CreateQuotationCommand r,CancellationToken ct)
    {
        BillingReferences.Customer(db,r.CustomerId);
        var entity = new Quotation(
            r.CustomerId,
            r.QuotationDate,
            r.ValidityDate,
            r.Lines.Select(x=>BillingReferences.QuotationLine(db,x)),
            clock.UtcNow);
        
        db.Add(entity);
        await db.SaveChangesAsync(ct);
        return new CreateQuotationResponse(BillingResponseMapper.Map(entity));
    }
}
