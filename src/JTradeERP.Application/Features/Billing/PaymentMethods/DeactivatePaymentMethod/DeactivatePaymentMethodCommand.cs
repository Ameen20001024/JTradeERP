using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.DeactivatePaymentMethod;

public sealed record DeactivatePaymentMethodCommand(Guid Id):IRequest<DeactivatePaymentMethodResponse>;
