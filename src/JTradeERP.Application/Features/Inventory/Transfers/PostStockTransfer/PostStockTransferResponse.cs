using JTradeERP.Application.Features.Inventory.Transfers;
namespace JTradeERP.Application.Features.Inventory.Transfers.PostStockTransfer;
public sealed record StockTransferMovementResponse(Guid LineId, Guid ProductId, decimal Quantity,
    decimal UnitCost, Guid OutLedgerEntryId, Guid InLedgerEntryId);
public sealed record PostStockTransferResponse(StockTransferResponse Transfer,
    IReadOnlyCollection<StockTransferMovementResponse> Movements);
