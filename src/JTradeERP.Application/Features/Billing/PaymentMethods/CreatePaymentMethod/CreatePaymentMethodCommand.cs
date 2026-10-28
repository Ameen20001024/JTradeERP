using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.CreatePaymentMethod;

public sealed record CreatePaymentMethodCommand(string Code,string Name):IRequest<CreatePaymentMethodResponse>;
