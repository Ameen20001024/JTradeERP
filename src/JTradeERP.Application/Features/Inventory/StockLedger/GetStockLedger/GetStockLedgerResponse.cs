using JTradeERP.Application.Common.Inventory;
namespace JTradeERP.Application.Features.Inventory.StockLedger.GetStockLedger;
public sealed record GetStockLedgerResponse(IReadOnlyCollection<StockLedgerItemResponse> Items,int Page,int PageSize,int TotalCount);
