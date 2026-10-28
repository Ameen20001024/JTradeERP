using JTradeERP.Domain.Inventory;

namespace JTradeERP.Application.Common.Inventory;

public static class StockAdjustmentMapper
{
    public static StockAdjustmentItemResponse ToResponse(StockAdjustment adjustment) => new(
        adjustment.Id,
        adjustment.ProductId,
        adjustment.WarehouseId,
        adjustment.QuantityDelta,
        adjustment.UnitCost,
        adjustment.ReasonCode,
        adjustment.Status.ToString(),
        adjustment.PostedAt,
        adjustment.ReversedAt);
}
