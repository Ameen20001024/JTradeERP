using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.Billing.PaymentMethods.Common;
using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.ActivatePaymentMethod;

public sealed class ActivatePaymentMethodHandler(IApplicationDbContext db):IRequestHandler<ActivatePaymentMethodCommand, ActivatePaymentMethodResponse>
{
    public async Task<ActivatePaymentMethodResponse> Handle(ActivatePaymentMethodCommand r,CancellationToken ct)
    {
        var x = db.PaymentMethods.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Payment method not found.");
        
        x.Activate();
        
        await db.SaveChangesAsync(ct);
        
        return new ActivatePaymentMethodResponse( new PaymentMethodResponse(x.Id,x.Code,x.Name,x.IsActive));
    }
}
