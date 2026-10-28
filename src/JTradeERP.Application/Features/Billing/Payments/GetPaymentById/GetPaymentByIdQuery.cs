using MediatR;

namespace JTradeERP.Application.Features.Billing.Payments.GetPaymentById;

public sealed record GetPaymentByIdQuery(Guid Id):IRequest<GetPaymentByIdResponse>;
