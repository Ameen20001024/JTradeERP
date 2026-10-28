using JTradeERP.Application.Features.Purchasing.Common;

namespace JTradeERP.Application.Features.Purchasing.PurchaseOrders.GetPurchaseOrders;

public sealed record GetPurchaseOrdersResponse(IReadOnlyCollection<PurchaseOrderSummaryResponse> Items,
    int PageNumber, int PageSize, int TotalCount);
