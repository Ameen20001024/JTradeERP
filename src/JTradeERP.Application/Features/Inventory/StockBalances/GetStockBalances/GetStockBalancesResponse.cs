using JTradeERP.Application.Common.Inventory;
namespace JTradeERP.Application.Features.Inventory.StockBalances.GetStockBalances;
public sealed record GetStockBalancesResponse(IReadOnlyCollection<StockBalanceItemResponse> Items, int Page, int PageSize, int TotalCount);
