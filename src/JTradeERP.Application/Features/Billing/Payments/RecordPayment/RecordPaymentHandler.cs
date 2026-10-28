using JTradeERP.Application.Common.Exceptions;

using JTradeERP.Application.Common.Persistence;

using JTradeERP.Application.Common.Security;

using JTradeERP.Domain.Billing;

using JTradeERP.Domain.Security;

using MediatR;

namespace JTradeERP.Application.Features.Billing.Payments.RecordPayment;
public sealed class RecordPaymentHandler(IApplicationDbContext db,
    ICurrentUserService actor,
    IClock clock):IRequestHandler<RecordPaymentCommand,RecordPaymentResponse>
{
    public async Task<RecordPaymentResponse> Handle(RecordPaymentCommand r,CancellationToken ct)
    {
        var method=db.PaymentMethods.SingleOrDefault(x=>x.Id==r.PaymentMethodId)??throw new NotFoundException("Payment method not found.");
        
        if(!method.IsActive)throw new ConflictException("Inactive payment method cannot be used.");
        
        var invoices = r.Allocations.Select(x=>db.Invoices.SingleOrDefault(i=>i.Id==x.InvoiceId)??throw new NotFoundException("Invoice not found.")).ToArray();
        
        for(var i=0; i<r.Allocations.Count; i++)
        {
            var invoice=invoices[i];
            if(invoice.Status is not(InvoiceStatus.Posted or InvoiceStatus.PartiallyPaid)||r.Allocations.ElementAt(i).Amount>invoice.OutstandingAmount)
                throw new ConflictException("Invoice is not payable or allocation exceeds outstanding amount.");
        }
        
        var user=actor.UserId??throw new AuthenticationFailureException();
        
        await using var tx=await db.BeginTransactionAsync(ct);
        
        var entities=r.Allocations.Select(x=>new PaymentAllocation(x.InvoiceId,x.Amount)).ToArray();
        
        var payment=new Payment(r.PaymentDate,
            r.PaymentMethodId,
            r.Amount,
            r.ReferenceNumber,
            entities,clock.UtcNow);
        
        for(var i=0;i<invoices.Length;i++)
            invoices[i].ApplyPayment(r.Allocations.ElementAt(i).Amount,clock.UtcNow);db.Add(payment);
        
        SecurityAuditWriter.Add(db,
            user,
            "PaymentRecorded",
            "Payment",
            payment.Id.ToString(),
            null,
            $"{payment.Amount}|{payment.Allocations.Sum(x=>x.Amount)}");
        
        await db.SaveChangesAsync(ct);
        
        await tx.CommitAsync(ct);
        
        return new RecordPaymentResponse(payment.Id,
            payment.PaymentDate,
            payment.PaymentMethodId,
            payment.Amount,
            payment.ReferenceNumber,
            payment.Status.ToString(),
            payment.Allocations.Select(x=>new PaymentAllocationResponse(x.InvoiceId,x.Amount)).ToArray());
    }
}
