using JTradeERP.Application.Common.Persistence;
using JTradeERP.Domain.Inventory;
namespace JTradeERP.Application.Features.Inventory.Transfers;
internal static class StockTransferResponseMapper
{
    public static StockTransferResponse Map(IApplicationDbContext db, StockTransfer transfer, IReadOnlyCollection<StockTransferLine> lines)
    {
        var warehouses = db.Warehouses.Where(x => x.Id == transfer.SourceWarehouseId || x.Id == transfer.DestinationWarehouseId)
            .ToDictionary(x => x.Id);
        var products = db.Products.Where(x => lines.Select(line => line.ProductId).Contains(x.Id)).ToDictionary(x => x.Id);
        return new(transfer.Id, transfer.SourceWarehouseId, warehouses[transfer.SourceWarehouseId].WarehouseCode,
            transfer.DestinationWarehouseId, warehouses[transfer.DestinationWarehouseId].WarehouseCode,
            transfer.Status.ToString(), transfer.PostedAt,
            lines.Select(x => new StockTransferLineResponse(x.Id, x.ProductId, products[x.ProductId].ProductCode,
                products[x.ProductId].Name, x.Quantity)).ToArray());
    }
}
