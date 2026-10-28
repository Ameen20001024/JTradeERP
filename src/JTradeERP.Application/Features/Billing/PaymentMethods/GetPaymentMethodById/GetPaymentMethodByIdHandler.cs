using JTradeERP.Application.Common.Exceptions;

using JTradeERP.Application.Common.Persistence;

using JTradeERP.Application.Features.Billing.PaymentMethods.Common;

using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.GetPaymentMethodById;

public sealed class GetPaymentMethodByIdHandler(IApplicationDbContext db):IRequestHandler<GetPaymentMethodByIdQuery,GetPaymentMethodByIdResponse>
{
    public Task<GetPaymentMethodByIdResponse> Handle(GetPaymentMethodByIdQuery r,CancellationToken ct)
    
    {
        var x = db.PaymentMethods.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Payment method not found.");
        
        return Task.FromResult(new GetPaymentMethodByIdResponse(new PaymentMethodResponse(x.Id,x.Code,x.Name,x.IsActive)));
    }
}
