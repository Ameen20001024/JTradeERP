using JTradeERP.Application.Common.Reports;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JTradeERP.Application.Features.Reports.StockValuation;

public static class GetStockValuationEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/stock-valuation", ([AsParameters] GetStockValuationQuery query, ISender sender, CancellationToken ct) => sender.Send(query, ct))
            .RequirePermission("Reports.View", Capability.View);
        endpoints.MapGet("/stock-valuation/export.csv", async ([AsParameters] GetStockValuationQuery query, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(query, ct);
            var bytes = CsvReportFormatter.Create(["WarehouseId", "WarehouseCode", "WarehouseName", "ProductId", "ProductCode", "ProductName", "CategoryId", "CategoryName", "QtyOnHand", "WeightedAverageCost", "InventoryValue"], result.Items,
                x => [x.WarehouseId, x.WarehouseCode, x.WarehouseName, x.ProductId, x.ProductCode, x.ProductName, x.CategoryId, x.CategoryName, x.QtyOnHand, x.WeightedAverageCost, x.InventoryValue]);
            return Results.File(bytes, "text/csv; charset=utf-8", "stock-valuation.csv");
        }).RequirePermission("Reports.View", Capability.View);
    }
}
