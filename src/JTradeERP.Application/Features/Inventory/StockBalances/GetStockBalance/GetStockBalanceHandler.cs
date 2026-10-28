using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Inventory;
using JTradeERP.Application.Common.Persistence;
using MediatR;
namespace JTradeERP.Application.Features.Inventory.StockBalances.GetStockBalance;
public sealed class GetStockBalanceHandler(IApplicationDbContext db) : IRequestHandler<GetStockBalanceQuery, GetStockBalanceResponse>
{
    public Task<GetStockBalanceResponse> Handle(GetStockBalanceQuery request, CancellationToken cancellationToken)
    {
        var product = db.Products.SingleOrDefault(x => x.Id == request.ProductId) ?? throw new NotFoundException("Product was not found.");
        var warehouse = db.Warehouses.SingleOrDefault(x => x.Id == request.WarehouseId) ?? throw new NotFoundException("Warehouse was not found.");
        var balance = db.StockBalances.SingleOrDefault(x => x.ProductId == request.ProductId && x.WarehouseId == request.WarehouseId);
        var dto = new StockBalanceItemResponse(product.Id, product.ProductCode, product.Name, warehouse.Id, warehouse.WarehouseCode, warehouse.Name,
            balance?.QtyOnHand ?? 0, balance?.QtyReserved ?? 0, balance?.AvailableQuantity ?? 0, balance?.WeightedAverageCost ?? 0,
            balance is null ? string.Empty : Convert.ToBase64String(balance.RowVersion));
        return Task.FromResult(new GetStockBalanceResponse(dto));
    }
}
