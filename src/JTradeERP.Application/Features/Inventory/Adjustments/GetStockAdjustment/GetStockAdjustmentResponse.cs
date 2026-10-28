using JTradeERP.Application.Common.Inventory;
namespace JTradeERP.Application.Features.Inventory.Adjustments.GetStockAdjustment;
public sealed record GetStockAdjustmentResponse(StockAdjustmentItemResponse Adjustment,Guid? LedgerEntryId);
