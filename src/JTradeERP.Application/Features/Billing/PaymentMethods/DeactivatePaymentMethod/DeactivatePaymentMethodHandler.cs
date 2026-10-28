using JTradeERP.Application.Common.Exceptions;

using JTradeERP.Application.Common.Persistence;

using JTradeERP.Application.Features.Billing.PaymentMethods.Common;

using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.DeactivatePaymentMethod;

public sealed class DeactivatePaymentMethodHandler(IApplicationDbContext db):IRequestHandler<DeactivatePaymentMethodCommand,DeactivatePaymentMethodResponse>
{
    public async Task<DeactivatePaymentMethodResponse> Handle(DeactivatePaymentMethodCommand r,CancellationToken ct)
    {
        var x = db.PaymentMethods.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Payment method not found.");
        
        x.Deactivate();
        
        await db.SaveChangesAsync(ct);
        
        return new DeactivatePaymentMethodResponse(new PaymentMethodResponse(x.Id,x.Code,x.Name,x.IsActive));
    }
}
