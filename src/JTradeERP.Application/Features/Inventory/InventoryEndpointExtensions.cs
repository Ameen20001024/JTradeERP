using JTradeERP.Application.Features.Inventory.Adjustments.CreateStockAdjustment;
using JTradeERP.Application.Features.Inventory.Adjustments.GetStockAdjustment;
using JTradeERP.Application.Features.Inventory.Adjustments.GetStockAdjustments;
using JTradeERP.Application.Features.Inventory.Adjustments.PostStockAdjustment;
using JTradeERP.Application.Features.Inventory.Adjustments.ReverseStockAdjustment;
using JTradeERP.Application.Features.Inventory.Adjustments.UpdateStockAdjustment;
using JTradeERP.Application.Features.Inventory.OpeningBalances.PostOpeningBalance;
using JTradeERP.Application.Features.Inventory.OpeningBalances.ReverseOpeningBalance;
using JTradeERP.Application.Features.Inventory.ReorderAlerts.GetReorderAlerts;
using JTradeERP.Application.Features.Inventory.StockBalances.GetStockBalance;
using JTradeERP.Application.Features.Inventory.StockBalances.GetStockBalances;
using JTradeERP.Application.Features.Inventory.StockLedger.GetStockLedger;
using JTradeERP.Application.Features.Inventory.WarehouseNegativeStock.UpdateWarehouseNegativeStockSetting;
using JTradeERP.Application.Features.Inventory.Transfers.CreateStockTransfer;
using JTradeERP.Application.Features.Inventory.Transfers.GetStockTransfer;
using JTradeERP.Application.Features.Inventory.Transfers.GetStockTransfers;
using JTradeERP.Application.Features.Inventory.Transfers.PostStockTransfer;
using JTradeERP.Application.Features.Inventory.Transfers.UpdateStockTransfer;

namespace JTradeERP.Application.Features.Inventory;

public static class InventoryEndpointExtensions
{
    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/inventory").RequireAuthorization();
        PostOpeningBalanceEndPoint.Map(group);
        ReverseOpeningBalanceEndPoint.Map(group);
        GetStockBalanceEndPoint.Map(group);
        GetStockBalancesEndPoint.Map(group);
        GetStockLedgerEndPoint.Map(group);
        GetReorderAlertsEndPoint.Map(group);
        CreateStockAdjustmentEndPoint.Map(group);
        UpdateStockAdjustmentEndPoint.Map(group);
        PostStockAdjustmentEndPoint.Map(group);
        ReverseStockAdjustmentEndPoint.Map(group);
        GetStockAdjustmentEndPoint.Map(group);
        GetStockAdjustmentsEndPoint.Map(group);
        CreateStockTransferEndPoint.Map(group);
        UpdateStockTransferEndPoint.Map(group);
        PostStockTransferEndPoint.Map(group);
        GetStockTransferEndPoint.Map(group);
        GetStockTransfersEndPoint.Map(group);
        UpdateWarehouseNegativeStockSettingEndPoint.Map(group);
        return endpoints;
    }
}
