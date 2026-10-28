using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.CopyQuotation;

public sealed record CopyQuotationCommand(
    Guid Id,
    DateOnly QuotationDate,
    DateOnly ValidityDate):IRequest<CopyQuotationResponse>;
