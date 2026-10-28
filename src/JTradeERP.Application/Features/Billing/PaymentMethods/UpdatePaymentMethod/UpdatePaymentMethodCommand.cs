using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.UpdatePaymentMethod;

public sealed record UpdatePaymentMethodCommand(Guid Id,string Code,string Name):IRequest<UpdatePaymentMethodResponse>;
