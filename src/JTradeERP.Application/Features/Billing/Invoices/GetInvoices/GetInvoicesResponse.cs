using JTradeERP.Application.Features.Billing.Common;

namespace JTradeERP.Application.Features.Billing.Invoices.GetInvoices;

public sealed record GetInvoicesResponse(
    IReadOnlyList<InvoiceDocumentResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
