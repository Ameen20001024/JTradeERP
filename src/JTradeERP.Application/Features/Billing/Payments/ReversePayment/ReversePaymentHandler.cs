using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Common.Security;
using MediatR;

namespace JTradeERP.Application.Features.Billing.Payments.ReversePayment;

public sealed class ReversePaymentHandler(
    IApplicationDbContext db,
    ICurrentUserService actor,
    IClock clock) : IRequestHandler<ReversePaymentCommand, ReversePaymentResponse>
{
    public async Task<ReversePaymentResponse> Handle(ReversePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = db.PaymentsWithAllocations.SingleOrDefault(x => x.Id == request.Id)
            ?? throw new NotFoundException("Payment not found.");

        if (payment.Status == JTradeERP.Domain.Billing.PaymentStatus.Reversed)
            return new(payment.Id, payment.Status.ToString(), payment.ReversedAt);

        var userId = actor.UserId ?? throw new AuthenticationFailureException();

        var invoices = payment.Allocations.Select(allocation =>
            db.Invoices.SingleOrDefault(x => x.Id == allocation.InvoiceId)
                ?? throw new NotFoundException("Invoice not found.")).ToArray();

        await using var transaction = await db.BeginTransactionAsync(cancellationToken);

        foreach (var allocation in payment.Allocations)
        {
            var invoice = invoices.Single(x => x.Id == allocation.InvoiceId);
            invoice.ReversePayment(allocation.Amount, clock.UtcNow);
        }

        payment.Reverse(clock.UtcNow, userId);
        SecurityAuditWriter.Add(db, 
            userId, 
            "PaymentReversed", 
            "Payment", 
            payment.Id.ToString(),
            payment.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture), 
            "Reversed");
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new ReversePaymentResponse(payment.Id, payment.Status.ToString(), payment.ReversedAt);
    }
}
