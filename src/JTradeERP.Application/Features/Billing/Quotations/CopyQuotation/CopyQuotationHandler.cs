using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using JTradeERP.Application.Features.Billing.Common;
using JTradeERP.Domain.Billing;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.CopyQuotation;

public sealed class CopyQuotationHandler(IApplicationDbContext db,IClock clock):IRequestHandler<CopyQuotationCommand,CopyQuotationResponse>
{
    public async Task<CopyQuotationResponse> Handle(CopyQuotationCommand r,CancellationToken ct)
    {
        var source=db.QuotationsWithLines.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Quotation not found.");
        var copy = new Quotation(
            source.CustomerId,
            r.QuotationDate,
            r.ValidityDate,
            source.Lines.Select(x=>new QuotationLine(
                x.ProductId,
                x.Quantity,
                x.UnitPrice,
                x.DiscountAmount,
                x.TaxCodeId,
                x.TaxRate)),
            clock.UtcNow);
        
        db.Add(copy);
        await db.SaveChangesAsync(ct);
        return new CopyQuotationResponse(BillingResponseMapper.Map(copy));
    }
}
