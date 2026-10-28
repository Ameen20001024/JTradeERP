using JTradeERP.Application.Common.Inventory;
namespace JTradeERP.Application.Features.Inventory.Adjustments.GetStockAdjustments;
public sealed record GetStockAdjustmentsResponse(IReadOnlyCollection<StockAdjustmentItemResponse> Items,int Page,int PageSize,int TotalCount);
