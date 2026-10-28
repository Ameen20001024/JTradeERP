using MediatR;
using JTradeERP.Application.Features.Billing.Common;
namespace JTradeERP.Application.Features.Billing.Quotations.UpdateQuotation;

public sealed record UpdateQuotationCommand(
    Guid Id,
    Guid CustomerId,
    DateOnly QuotationDate,
    DateOnly ValidityDate,
    IReadOnlyCollection<BillingLineInput> Lines):IRequest<UpdateQuotationResponse>;
