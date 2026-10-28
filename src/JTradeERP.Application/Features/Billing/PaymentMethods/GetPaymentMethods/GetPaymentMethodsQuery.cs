using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.GetPaymentMethods;

public sealed record GetPaymentMethodsQuery(bool IncludeInactive=false):IRequest<GetPaymentMethodsResponse>;
