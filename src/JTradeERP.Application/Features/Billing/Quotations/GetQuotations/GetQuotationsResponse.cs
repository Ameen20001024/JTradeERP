using JTradeERP.Application.Features.Billing.Common;
namespace JTradeERP.Application.Features.Billing.Quotations.GetQuotations;

public sealed record GetQuotationsResponse(
    IReadOnlyList<QuotationDocumentResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
