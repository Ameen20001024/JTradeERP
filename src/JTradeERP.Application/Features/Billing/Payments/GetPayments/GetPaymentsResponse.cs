namespace JTradeERP.Application.Features.Billing.Payments.GetPayments;

public sealed record PaymentAllocationItemResponse(Guid InvoiceId,decimal Amount);

public sealed record GetPaymentItemResponse(
    Guid Id,
    DateOnly PaymentDate,
    Guid PaymentMethodId,
    decimal Amount,
    string? ReferenceNumber,
    string Status,
    IReadOnlyList<PaymentAllocationItemResponse> Allocations);

public sealed record GetPaymentsResponse(
    IReadOnlyList<GetPaymentItemResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
