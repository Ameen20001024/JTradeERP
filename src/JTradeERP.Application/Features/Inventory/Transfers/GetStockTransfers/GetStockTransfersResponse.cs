using JTradeERP.Application.Features.Inventory.Transfers;
namespace JTradeERP.Application.Features.Inventory.Transfers.GetStockTransfers;
public sealed record GetStockTransfersResponse(IReadOnlyCollection<StockTransferResponse> Items, int Page, int PageSize, int TotalCount);
