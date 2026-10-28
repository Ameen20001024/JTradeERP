using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.DeletePaymentMethod;

public sealed record DeletePaymentMethodCommand(Guid Id):IRequest<DeletePaymentMethodResponse>;
