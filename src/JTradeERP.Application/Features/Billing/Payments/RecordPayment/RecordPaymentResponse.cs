namespace JTradeERP.Application.Features.Billing.Payments.RecordPayment;

public sealed record PaymentAllocationResponse(Guid InvoiceId,decimal Amount);

public sealed record RecordPaymentResponse(Guid Id,
    DateOnly PaymentDate,
    Guid PaymentMethodId,
    decimal Amount,
    string? ReferenceNumber,
    string Status,
    IReadOnlyList<PaymentAllocationResponse> Allocations);
