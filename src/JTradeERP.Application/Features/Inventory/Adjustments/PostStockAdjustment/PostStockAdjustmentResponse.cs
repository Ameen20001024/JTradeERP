

using JTradeERP.Application.Common.Inventory;

namespace JTradeERP.Application.Features.Inventory.Adjustments.PostStockAdjustment;
public sealed record PostStockAdjustmentResponse(StockAdjustmentItemResponse Adjustment, Guid LedgerEntryId);
