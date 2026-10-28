using JTradeERP.Application.Common.Exceptions;

using JTradeERP.Application.Common.Persistence;

using JTradeERP.Application.Features.Billing.PaymentMethods.Common;

using JTradeERP.Domain.Billing;

using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.CreatePaymentMethod;

public sealed class CreatePaymentMethodHandler(IApplicationDbContext db):IRequestHandler<CreatePaymentMethodCommand,CreatePaymentMethodResponse>
{
    public async Task<CreatePaymentMethodResponse> Handle(CreatePaymentMethodCommand r,CancellationToken ct)
    {
        var code = r.Code.Trim().ToUpperInvariant();
        
        if(db.PaymentMethods.Any(x=>x.Code==code))throw new ConflictException("Payment method code already exists.");
        
        var x = new PaymentMethod(code,r.Name);
        
        db.Add(x);
        
        await db.SaveChangesAsync(ct);
        
        return new CreatePaymentMethodResponse(new PaymentMethodResponse(x.Id,x.Code,x.Name,x.IsActive));
    }
}
