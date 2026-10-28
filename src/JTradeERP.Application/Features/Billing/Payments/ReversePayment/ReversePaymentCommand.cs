using MediatR;

namespace JTradeERP.Application.Features.Billing.Payments.ReversePayment;

public sealed record ReversePaymentCommand(Guid Id):IRequest<ReversePaymentResponse>;