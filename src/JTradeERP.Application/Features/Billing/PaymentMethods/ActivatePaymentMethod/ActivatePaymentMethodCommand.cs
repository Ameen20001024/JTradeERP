using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.ActivatePaymentMethod;

public sealed record ActivatePaymentMethodCommand(Guid Id):IRequest<ActivatePaymentMethodResponse>;
