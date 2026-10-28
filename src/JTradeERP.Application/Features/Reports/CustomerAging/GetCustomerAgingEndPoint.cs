using JTradeERP.Application.Common.Reports;
using JTradeERP.Application.Common.Security;
using JTradeERP.Domain.Security;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace JTradeERP.Application.Features.Reports.CustomerAging;

public static class GetCustomerAgingEndPoint
{
    public static void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/customer-aging", ([AsParameters] GetCustomerAgingQuery query, ISender sender, CancellationToken ct) =>
                sender.Send(query, ct))
            .RequirePermission("Reports.View", Capability.View);

        endpoints.MapGet("/customer-aging/export.csv", async ([AsParameters] GetCustomerAgingQuery query, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(query, ct);
            var bytes = CsvReportFormatter.Create(
                ["CustomerId", "CustomerCode", "CustomerName", "AsOfDate", "Current", "Days1To30", "Days31To60", "Days61To90", "Days91Plus", "TotalOutstanding"],
                result.Items,
                x => [x.CustomerId, x.CustomerCode, x.CustomerName, x.AsOfDate, x.Current, x.Days1To30,
                    x.Days31To60, x.Days61To90, x.Days91Plus, x.TotalOutstanding]);
            return Results.File(bytes, "text/csv; charset=utf-8", "customer-aging.csv");
        }).RequirePermission("Reports.View", Capability.View);
    }
}
