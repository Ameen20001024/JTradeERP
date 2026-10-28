using JTradeERP.Application.Common.Reports;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JTradeERP.Application.Features.Reports.SalesSummary;

public static class GetSalesSummaryEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/sales-summary", ( [AsParameters] GetSalesSummaryQuery query, ISender sender, CancellationToken ct) => sender.Send(query, ct))
            .RequirePermission("Reports.View", Capability.View);
        endpoints.MapGet("/sales-summary/export.csv", async ([AsParameters] GetSalesSummaryQuery query, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(query, ct);
            var bytes = CsvReportFormatter.Create(["Date", "CustomerId", "CustomerName", "ProductId", "ProductCode", "ProductName", "SubTotal", "DiscountAmount", "NetAmount", "TaxAmount", "GrandTotal"], result.Items,
                x => [x.Date, x.CustomerId, x.CustomerName, x.ProductId, x.ProductCode, x.ProductName, x.SubTotal, x.DiscountAmount, x.NetAmount, x.TaxAmount, x.GrandTotal]);
            return Results.File(bytes, "text/csv; charset=utf-8", "sales-summary.csv");
        }).RequirePermission("Reports.View", Capability.View);
    }
}
