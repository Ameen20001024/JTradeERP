namespace JTradeERP.Application.Features.Billing.PaymentMethods.GetPaymentMethods;

public sealed record PaymentMethodItemResponse(Guid Id,string Code,string Name,bool IsActive);

public sealed record GetPaymentMethodsResponse(IReadOnlyList<PaymentMethodItemResponse> Items);
