using JTradeERP.Application.Common.Exceptions;

using JTradeERP.Application.Common.Persistence;

using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.DeletePaymentMethod;

public sealed class DeletePaymentMethodHandler(IApplicationDbContext db):IRequestHandler<DeletePaymentMethodCommand,DeletePaymentMethodResponse>

{
    public async Task<DeletePaymentMethodResponse> Handle(DeletePaymentMethodCommand r,CancellationToken ct)
    
    {
        
        var x = db.PaymentMethods.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Payment method not found.");
        
        if(db.Payments.Any(p=>p.PaymentMethodId==r.Id)) throw new ConflictException("Payment method is referenced by payments and cannot be deleted.");
        
        db.Remove(x);
        
        await db.SaveChangesAsync(ct);
        
        return new DeletePaymentMethodResponse(x.Id,true);}}
