using JTradeERP.Application.Features.Billing.Common;
namespace JTradeERP.Application.Features.Billing.SalesOrders.GetSalesOrders;

public sealed record GetSalesOrdersResponse(
    IReadOnlyList<SalesOrderDocumentResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
