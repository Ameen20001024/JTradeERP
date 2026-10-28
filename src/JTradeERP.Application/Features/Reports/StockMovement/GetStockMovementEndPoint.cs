using JTradeERP.Application.Common.Reports;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JTradeERP.Application.Features.Reports.StockMovement;

public static class GetStockMovementEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/stock-movement", ([AsParameters] GetStockMovementQuery query, ISender sender, CancellationToken ct) => sender.Send(query, ct))
            .RequirePermission("Reports.View", Capability.View);
        endpoints.MapGet("/stock-movement/export.csv", async ([AsParameters] GetStockMovementQuery query, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(query, ct);
            var bytes = CsvReportFormatter.Create(["LedgerEntryId", "PostedAt", "WarehouseId", "WarehouseCode", "WarehouseName", "ProductId", "ProductCode", "ProductName", "MovementType", "Direction", "Quantity", "UnitCost", "SourceDocumentType", "SourceDocumentId", "SourceLineId", "ReversalOfLedgerEntryId", "CorrelationId"], result.Items,
                x => [x.LedgerEntryId, x.PostedAt, x.WarehouseId, x.WarehouseCode, x.WarehouseName, x.ProductId, x.ProductCode, x.ProductName, x.MovementType, x.Direction, x.Quantity, x.UnitCost, x.SourceDocumentType, x.SourceDocumentId, x.SourceLineId, x.ReversalOfLedgerEntryId, x.CorrelationId]);
            return Results.File(bytes, "text/csv; charset=utf-8", "stock-movement.csv");
        }).RequirePermission("Reports.View", Capability.View);
    }
}
