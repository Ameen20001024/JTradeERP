using JTradeERP.Application.Common.Exceptions;

using JTradeERP.Application.Common.Persistence;

using JTradeERP.Application.Features.Billing.PaymentMethods.Common;

using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.UpdatePaymentMethod;

public sealed class UpdatePaymentMethodHandler(IApplicationDbContext db):IRequestHandler<UpdatePaymentMethodCommand,UpdatePaymentMethodResponse>
{
    public async Task<UpdatePaymentMethodResponse> Handle(UpdatePaymentMethodCommand r,CancellationToken ct)
    {
        var x = db.PaymentMethods.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Payment method not found.");
        
        var code=r.Code.Trim().ToUpperInvariant();
        
        if(db.PaymentMethods.Any(x=>x.Id!=r.Id&&(x.Code==code||x.Name==r.Name.Trim())))
            
            throw new ConflictException("Payment method code or name already exists.");
        
        x.Update(code,r.Name);
        
        await db.SaveChangesAsync(ct);
        
        return new UpdatePaymentMethodResponse(new PaymentMethodResponse(x.Id,x.Code,x.Name,x.IsActive));
    }
}
