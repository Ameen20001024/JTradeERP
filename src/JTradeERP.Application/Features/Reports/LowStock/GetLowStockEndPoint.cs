using JTradeERP.Application.Common.Reports;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JTradeERP.Application.Features.Reports.LowStock;

public static class GetLowStockEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/low-stock", ([AsParameters] GetLowStockQuery query, ISender sender, CancellationToken ct) => sender.Send(query, ct))
            .RequirePermission("Reports.View", Capability.View);
        endpoints.MapGet("/low-stock/export.csv", async ([AsParameters] GetLowStockQuery query, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(query, ct);
            var bytes = CsvReportFormatter.Create(["WarehouseId", "WarehouseCode", "WarehouseName", "ProductId", "ProductCode", "ProductName", "CategoryId", "CategoryName", "QtyOnHand", "QtyReserved", "AvailableQuantity", "MinimumStockLevel", "ReorderQuantity"], result.Items,
                x => [x.WarehouseId, x.WarehouseCode, x.WarehouseName, x.ProductId, x.ProductCode, x.ProductName, x.CategoryId, x.CategoryName, x.QtyOnHand, x.QtyReserved, x.AvailableQuantity, x.MinimumStockLevel, x.ReorderQuantity]);
            return Results.File(bytes, "text/csv; charset=utf-8", "low-stock.csv");
        }).RequirePermission("Reports.View", Capability.View);
    }
}
