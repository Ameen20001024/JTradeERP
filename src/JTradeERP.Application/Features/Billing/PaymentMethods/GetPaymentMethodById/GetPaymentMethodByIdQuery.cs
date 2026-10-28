using MediatR;

namespace JTradeERP.Application.Features.Billing.PaymentMethods.GetPaymentMethodById;

public sealed record GetPaymentMethodByIdQuery(Guid Id):IRequest<GetPaymentMethodByIdResponse>;
