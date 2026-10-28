namespace JTradeERP.Application.Features.Billing.Payments.ReversePayment;

public sealed record ReversePaymentResponse(Guid Id,string Status,DateTimeOffset? ReversedAt);
