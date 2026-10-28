using System.Net;
using System.Text.Json;

namespace JTradeERP.Api.IntegrationTests;

[Collection(nameof(SqlServerIntegrationCollection))]
public sealed class ReportsEndpointTests(ApiFactory factory)
{
    [Fact]
    public async Task Reports_UseSqlBackedReadServiceAndProvideCsvExports()
    {
        await factory.ResetDatabaseAsync();
        using var auditor = await factory.CreateAuthenticatedClientAsync("Auditor");
        var routes = new[]
        {
            "/api/reports/sales-summary",
            "/api/reports/stock-valuation",
            "/api/reports/low-stock",
            "/api/reports/stock-movement",
            "/api/reports/customer-aging?asOfDate=2026-06-30"
        };
        foreach (var route in routes)
        {
            using var response = await auditor.GetAsync(route);
            Assert.True(response.StatusCode == HttpStatusCode.OK, $"{route}: {await response.Content.ReadAsStringAsync()}");
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.True(json.RootElement.TryGetProperty("items", out _));
        }

        using var csv = await auditor.GetAsync("/api/reports/stock-movement/export.csv?page=1&pageSize=5");
        Assert.Equal(HttpStatusCode.OK, csv.StatusCode);
        Assert.Equal("text/csv", csv.Content.Headers.ContentType?.MediaType);
        Assert.Contains("LedgerEntryId", await csv.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await auditor.PostAsync("/api/reports/stock-movement", null)).StatusCode);
        using var agingCsv = await auditor.GetAsync("/api/reports/customer-aging/export.csv?asOfDate=2026-06-30");
        Assert.Equal(HttpStatusCode.OK, agingCsv.StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await auditor.PostAsync("/api/reports/customer-aging", null)).StatusCode);
    }

    [Fact]
    public async Task Reports_RequireReportsView_AndRejectUnapprovedSortExpressions()
    {
        await factory.ResetDatabaseAsync();
        using var anonymous = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.GetAsync("/api/reports/sales-summary")).StatusCode);

        using var readOnlyUser = await factory.CreateAuthenticatedClientAsync("Read-Only User");
        Assert.Equal(HttpStatusCode.Forbidden, (await readOnlyUser.GetAsync("/api/reports/sales-summary")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden,
            (await readOnlyUser.GetAsync("/api/reports/customer-aging?asOfDate=2026-06-30")).StatusCode);

        using var salesUser = await factory.CreateAuthenticatedClientAsync("Sales User");
        var invalidSort = await salesUser.GetAsync("/api/reports/sales-summary?sortBy=GrandTotal%3Bdrop%20table%20Invoices");
        Assert.Equal(HttpStatusCode.BadRequest, invalidSort.StatusCode);
    }
}
