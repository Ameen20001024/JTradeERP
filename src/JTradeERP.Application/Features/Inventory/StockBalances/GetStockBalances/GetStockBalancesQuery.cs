using MediatR;
namespace JTradeERP.Application.Features.Inventory.StockBalances.GetStockBalances;
public sealed record GetStockBalancesQuery(Guid? ProductId = null, Guid? WarehouseId = null, int Page = 1, int PageSize = 50) : IRequest<GetStockBalancesResponse>;
