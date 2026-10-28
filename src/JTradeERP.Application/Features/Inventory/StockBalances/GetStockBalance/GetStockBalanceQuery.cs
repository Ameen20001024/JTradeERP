using MediatR;
namespace JTradeERP.Application.Features.Inventory.StockBalances.GetStockBalance;
public sealed record GetStockBalanceQuery(Guid ProductId, Guid WarehouseId) : IRequest<GetStockBalanceResponse>;
