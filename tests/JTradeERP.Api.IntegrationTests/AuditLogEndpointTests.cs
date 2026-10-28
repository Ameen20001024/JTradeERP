using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using JTradeERP.Domain.Security;
using JTradeERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JTradeERP.Api.IntegrationTests;

[Collection(nameof(SqlServerIntegrationCollection))]
public sealed class AuditLogEndpointTests(ApiFactory factory)
{
    [Fact]
    public async Task AuditLogs_AuditorCanReadAndFilterButCannotMutate()
    {
        await factory.ResetDatabaseAsync();
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            db.SecurityAuditEntries.AddRange(
                new SecurityAuditEntry(null, "InvoicePosted", "Invoice", Guid.NewGuid().ToString(), null, "posted", "test-correlation-1"),
                new SecurityAuditEntry(null, "UserCreated", "User", Guid.NewGuid().ToString(), null, "created", "test-correlation-2"));
            await db.SaveChangesAsync();
        }

        using var auditor = await factory.CreateAuthenticatedClientAsync("Auditor");
        using var all = await auditor.GetAsync("/api/admin/audit-logs?page=1&pageSize=1");
        Assert.Equal(HttpStatusCode.OK, all.StatusCode);
        using var page = JsonDocument.Parse(await all.Content.ReadAsStringAsync());
        Assert.Equal(1, page.RootElement.GetProperty("items").GetArrayLength());
        Assert.Equal(2, page.RootElement.GetProperty("totalCount").GetInt32());

        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await auditor.PostAsJsonAsync("/api/admin/audit-logs", new { })).StatusCode);
    }

    [Fact]
    public async Task AuditLogs_RequireAuditLogViewPermission()
    {
        await factory.ResetDatabaseAsync();
        using var anonymous = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.GetAsync("/api/admin/audit-logs")).StatusCode);

        using var salesUser = await factory.CreateAuthenticatedClientAsync("Sales User");
        Assert.Equal(HttpStatusCode.Forbidden, (await salesUser.GetAsync("/api/admin/audit-logs")).StatusCode);

        using var auditor = await factory.CreateAuthenticatedClientAsync("Auditor");
        var invalidPage = await auditor.GetAsync("/api/admin/audit-logs?page=0");
        Assert.Equal(HttpStatusCode.BadRequest, invalidPage.StatusCode);
    }
}
