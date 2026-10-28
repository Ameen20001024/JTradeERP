using MediatR;

namespace JTradeERP.Application.Features.Billing.Quotations.AcceptQuotation;

public sealed record AcceptQuotationCommand(Guid Id):IRequest<AcceptQuotationResponse>;
