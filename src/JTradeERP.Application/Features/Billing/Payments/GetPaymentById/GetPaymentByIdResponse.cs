namespace JTradeERP.Application.Features.Billing.Payments.GetPaymentById;

public sealed record PaymentAllocationDto(Guid InvoiceId, decimal Amount);

public sealed record GetPaymentByIdResponse(
    Guid Id,
    DateOnly PaymentDate,
    Guid PaymentMethodId,
    decimal Amount,
    string? ReferenceNumber,
    string Status,
    DateTimeOffset? ReversedAt,
    IReadOnlyList<PaymentAllocationDto> Allocations);
