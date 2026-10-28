using MediatR;

using JTradeERP.Domain.Billing;

namespace JTradeERP.Application.Features.Billing.Payments.GetPayments;

public sealed record GetPaymentsQuery(
    PaymentStatus? Status=null,
    DateOnly? From=null,
    DateOnly? To=null,
    int Page=1,
    int PageSize=50):IRequest<GetPaymentsResponse>;
