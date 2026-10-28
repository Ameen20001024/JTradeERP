using MediatR;

namespace JTradeERP.Application.Features.Billing.Payments.RecordPayment;

public sealed record PaymentAllocationInput(Guid InvoiceId,decimal Amount);

public sealed record RecordPaymentCommand(
    DateOnly PaymentDate,
    Guid PaymentMethodId,
    decimal Amount,
    string? ReferenceNumber,
    IReadOnlyCollection<PaymentAllocationInput> Allocations):IRequest<RecordPaymentResponse>;
