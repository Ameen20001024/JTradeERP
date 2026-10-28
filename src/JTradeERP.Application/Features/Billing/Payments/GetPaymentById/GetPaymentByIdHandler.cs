using JTradeERP.Application.Common.Exceptions;

using JTradeERP.Application.Common.Persistence;

using MediatR;

namespace JTradeERP.Application.Features.Billing.Payments.GetPaymentById;

public sealed class GetPaymentByIdHandler(IApplicationDbContext db):IRequestHandler<GetPaymentByIdQuery,GetPaymentByIdResponse>
{
    public Task<GetPaymentByIdResponse> Handle(GetPaymentByIdQuery r,CancellationToken ct)
    {
        var x = db.PaymentsWithAllocations.SingleOrDefault(x=>x.Id==r.Id)??throw new NotFoundException("Payment not found.");
        
        return Task.FromResult(
            new GetPaymentByIdResponse(
                x.Id,
                x.PaymentDate,
                x.PaymentMethodId,
                x.Amount,
                x.ReferenceNumber,
                x.Status.ToString(),
                x.ReversedAt,
                x.Allocations.Select(a=>new PaymentAllocationDto(a.InvoiceId, a.Amount)).ToArray())
            );
    }
}
