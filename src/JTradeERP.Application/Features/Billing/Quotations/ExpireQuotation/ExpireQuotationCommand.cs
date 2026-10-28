using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.ExpireQuotation;

public sealed record ExpireQuotationCommand(Guid Id):IRequest<ExpireQuotationResponse>;
