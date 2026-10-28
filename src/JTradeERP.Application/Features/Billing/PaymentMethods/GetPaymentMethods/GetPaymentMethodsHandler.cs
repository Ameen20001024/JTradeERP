using JTradeERP.Application.Common.Persistence;

using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.GetPaymentMethods;

public sealed class GetPaymentMethodsHandler(IApplicationDbContext db):IRequestHandler<GetPaymentMethodsQuery,GetPaymentMethodsResponse>

{
    public Task<GetPaymentMethodsResponse> Handle(GetPaymentMethodsQuery r,CancellationToken ct) => 
        
        Task.FromResult(new GetPaymentMethodsResponse(
            
            db.PaymentMethods
            .Where(x=>r.IncludeInactive||x.IsActive)
            .OrderBy(x=>x.Name)
            .Select(x=>new PaymentMethodItemResponse(x.Id,x.Code,x.Name,x.IsActive))
            .ToList()
        ));
}
