using JTradeERP.Application.Features.Billing.Common;
using MediatR;
namespace JTradeERP.Application.Features.Billing.Quotations.CreateQuotation;
public sealed record CreateQuotationCommand(
    Guid CustomerId,
    DateOnly QuotationDate,
    DateOnly ValidityDate,
    IReadOnlyCollection<BillingLineInput> Lines) : IRequest<CreateQuotationResponse>;
