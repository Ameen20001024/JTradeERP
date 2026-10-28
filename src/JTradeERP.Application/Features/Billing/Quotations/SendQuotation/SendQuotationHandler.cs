using JTradeERP.Application.Common.Exceptions;using JTradeERP.Application.Common.Persistence;using JTradeERP.Application.Common.Security;using JTradeERP.Application.Features.Billing.Common;using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.SendQuotation;

public sealed class SendQuotationHandler(IApplicationDbContext db,IClock clock):IRequestHandler<SendQuotationCommand,SendQuotationResponse>
{
    public async Task<SendQuotationResponse> Handle(SendQuotationCommand r,CancellationToken ct)
    {
        var q=db.QuotationsWithLines.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Quotation not found.");
        
        q.Send(clock.UtcNow);
        
        await db.SaveChangesAsync(ct);
        
        return new SendQuotationResponse(BillingResponseMapper.Map(q));
    }
}
