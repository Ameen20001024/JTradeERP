using System.Net;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using JTradeERP.Domain.Inventory;
using JTradeERP.Domain.Security;
using JTradeERP.Infrastructure.Persistence;
using JTradeERP.Application.Common.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JTradeERP.Api.IntegrationTests;

[Collection(nameof(SqlServerIntegrationCollection))]
public sealed class InventoryEndpointTests(ApiFactory factory)
{
    [Fact]
    public async Task OpeningBalances_AreIdempotent_AndReceiptsUseWeightedAverageCost()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, sourceWarehouseId, destinationWarehouseId) = await SetupAsync(client);
        var source = Guid.NewGuid();

        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/inventory/opening-balances",
            Opening(source, productId, sourceWarehouseId, 10, 4))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/inventory/opening-balances",
            Opening(source, productId, sourceWarehouseId, 10, 4))).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/inventory/opening-balances",
            Opening(source, productId, sourceWarehouseId, 11, 4))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/inventory/opening-balances",
            Opening(Guid.NewGuid(), productId, sourceWarehouseId, 10, 6))).StatusCode);

        using var balance = JsonDocument.Parse(await (await client.GetAsync(BalanceRoute(productId, sourceWarehouseId))).Content.ReadAsStringAsync());
        Assert.Equal(20m, balance.RootElement.GetProperty("balance").GetProperty("qtyOnHand").GetDecimal());
        Assert.Equal(5m, balance.RootElement.GetProperty("balance").GetProperty("weightedAverageCost").GetDecimal());
        using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Equal(2, await db.StockLedgerEntries.CountAsync(x => x.SourceDocumentType == "OpeningBalance"));
        using var ledger = JsonDocument.Parse(await (await client.GetAsync($"/api/inventory/stock-ledger?productId={productId}&warehouseId={sourceWarehouseId}")).Content.ReadAsStringAsync());
        Assert.Equal(2, ledger.RootElement.GetProperty("totalCount").GetInt32());
        using var balances = JsonDocument.Parse(await (await client.GetAsync($"/api/inventory/stock-balances?productId={productId}")).Content.ReadAsStringAsync());
        Assert.Single(balances.RootElement.GetProperty("items").EnumerateArray());
    }

    [Fact]
    public async Task Adjustment_PostAndReverse_AppendLedgerAndRestoreBalanceIdempotently()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, sourceWarehouseId, _) = await SetupAsync(client);
        var create = await client.PostAsJsonAsync("/api/inventory/adjustments", new
        { productId, warehouseId = sourceWarehouseId, quantityDelta = 7m, reasonCode = "COUNT" });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        using var created = JsonDocument.Parse(await create.Content.ReadAsStringAsync());
        var adjustmentId = created.RootElement.GetProperty("adjustment").GetProperty("id").GetGuid();

        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync($"/api/inventory/adjustments/{adjustmentId}/post",
            new { unitCost = 3m })).StatusCode);
        var firstReverse = await client.PostAsync($"/api/inventory/adjustments/{adjustmentId}/reverse", null);
        var secondReverse = await client.PostAsync($"/api/inventory/adjustments/{adjustmentId}/reverse", null);
        Assert.Equal(HttpStatusCode.OK, firstReverse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondReverse.StatusCode);
        using var first = JsonDocument.Parse(await firstReverse.Content.ReadAsStringAsync());
        using var second = JsonDocument.Parse(await secondReverse.Content.ReadAsStringAsync());
        Assert.Equal(first.RootElement.GetProperty("reversalLedgerEntryId").GetGuid(),
            second.RootElement.GetProperty("reversalLedgerEntryId").GetGuid());

        using var balance = JsonDocument.Parse(await (await client.GetAsync(BalanceRoute(productId, sourceWarehouseId))).Content.ReadAsStringAsync());
        Assert.Equal(0m, balance.RootElement.GetProperty("balance").GetProperty("qtyOnHand").GetDecimal());
        using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Equal(2, await db.StockLedgerEntries.CountAsync(x => x.SourceDocumentId == adjustmentId));
    }

    [Fact]
    public async Task Transfer_PostCreatesPairedEntriesAtomically_AndCarriesWeightedAverageCost()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, sourceWarehouseId, destinationWarehouseId) = await SetupAsync(client);
        await client.PostAsJsonAsync("/api/inventory/opening-balances", Opening(Guid.NewGuid(), productId, sourceWarehouseId, 8, 12));
        var draft = await client.PostAsJsonAsync("/api/inventory/transfers", new
        {
            sourceWarehouseId, destinationWarehouseId,
            lines = new[] { new { productId, quantity = 3m } }
        });
        Assert.Equal(HttpStatusCode.Created, draft.StatusCode);
        using var created = JsonDocument.Parse(await draft.Content.ReadAsStringAsync());
        var transferId = created.RootElement.GetProperty("transfer").GetProperty("id").GetGuid();
        var update = await client.PutAsJsonAsync($"/api/inventory/transfers/{transferId}", new
        {
            sourceWarehouseId, destinationWarehouseId,
            lines = new[] { new { productId, quantity = 2m } }
        });
        Assert.True(update.StatusCode == HttpStatusCode.OK,
            $"{(int)update.StatusCode}: {await update.Content.ReadAsStringAsync()}");
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/inventory/transfers/{transferId}")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/inventory/transfers?page=1&pageSize=10")).StatusCode);

        var posted = await client.PostAsync($"/api/inventory/transfers/{transferId}/post", null);
        Assert.Equal(HttpStatusCode.OK, posted.StatusCode);
        using var postResult = JsonDocument.Parse(await posted.Content.ReadAsStringAsync());
        var movement = postResult.RootElement.GetProperty("movements")[0];
        Assert.Equal(12m, movement.GetProperty("unitCost").GetDecimal());
        Assert.NotEqual(movement.GetProperty("outLedgerEntryId").GetGuid(), movement.GetProperty("inLedgerEntryId").GetGuid());

        using var source = JsonDocument.Parse(await (await client.GetAsync(BalanceRoute(productId, sourceWarehouseId))).Content.ReadAsStringAsync());
        using var destination = JsonDocument.Parse(await (await client.GetAsync(BalanceRoute(productId, destinationWarehouseId))).Content.ReadAsStringAsync());
        Assert.Equal(6m, source.RootElement.GetProperty("balance").GetProperty("qtyOnHand").GetDecimal());
        Assert.Equal(2m, destination.RootElement.GetProperty("balance").GetProperty("qtyOnHand").GetDecimal());
        using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Equal(2, await db.StockLedgerEntries.CountAsync(x => x.SourceDocumentType == "StockTransfer" && x.SourceDocumentId == transferId));
    }

    [Fact]
    public async Task Transfer_ForcedSqlFailureRollsBackStatusBalancesLedgerAndAudit()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, sourceWarehouseId, destinationWarehouseId) = await SetupAsync(client);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/inventory/opening-balances",
            Opening(Guid.NewGuid(), productId, sourceWarehouseId, 8, 12))).StatusCode);
        var draft = await client.PostAsJsonAsync("/api/inventory/transfers", new
        {
            sourceWarehouseId, destinationWarehouseId,
            lines = new[] { new { productId, quantity = 3m } }
        });
        Assert.Equal(HttpStatusCode.Created, draft.StatusCode);
        using var transferJson = JsonDocument.Parse(await draft.Content.ReadAsStringAsync());
        var transferId = transferJson.RootElement.GetProperty("transfer").GetProperty("id").GetGuid();

        const string triggerName = "TR_Test_FailStockTransferLedger";
        await using (var setupScope = factory.Services.CreateAsyncScope())
        {
            var db = setupScope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            await db.Database.ExecuteSqlRawAsync($"""
                CREATE TRIGGER dbo.{triggerName} ON dbo.StockLedgerEntries AFTER INSERT AS
                BEGIN
                    IF EXISTS (SELECT 1 FROM inserted WHERE SourceDocumentType = 'StockTransfer')
                    BEGIN
                        THROW 51001, 'Forced stock transfer transaction failure', 1;
                    END;
                END;
                """);
        }

        HttpResponseMessage? failedPost = null;
        try
        {
            failedPost = await client.PostAsync($"/api/inventory/transfers/{transferId}/post", null);
            Assert.Equal(HttpStatusCode.InternalServerError, failedPost.StatusCode);
        }
        finally
        {
            failedPost?.Dispose();
            await using var cleanupScope = factory.Services.CreateAsyncScope();
            var db = cleanupScope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            await db.Database.ExecuteSqlRawAsync($"DROP TRIGGER IF EXISTS dbo.{triggerName};");
        }

        await using var verifyScope = factory.Services.CreateAsyncScope();
        var verify = verifyScope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Equal(InventoryDocumentStatus.Draft, (await verify.StockTransfers.SingleAsync(x => x.Id == transferId)).Status);
        Assert.Equal(8m, (await verify.StockBalances.SingleAsync(x => x.ProductId == productId && x.WarehouseId == sourceWarehouseId)).QtyOnHand);
        Assert.False(await verify.StockBalances.AnyAsync(x => x.ProductId == productId && x.WarehouseId == destinationWarehouseId));
        Assert.Empty(await verify.StockLedgerEntries.Where(x => x.SourceDocumentType == "StockTransfer" && x.SourceDocumentId == transferId).ToListAsync());
        Assert.False(await verify.SecurityAuditEntries.AnyAsync(x => x.Entity == "StockTransfer" && x.EntityId == transferId.ToString() && x.Action == "StockTransferPosted"));
    }

    [Fact]
    public async Task TransferWithInsufficientAvailableStock_RemainsDraftWithoutLedgerEffects()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, sourceWarehouseId, destinationWarehouseId) = await SetupAsync(client);
        await client.PostAsJsonAsync("/api/inventory/opening-balances", Opening(Guid.NewGuid(), productId, sourceWarehouseId, 2, 1));
        var draft = await client.PostAsJsonAsync("/api/inventory/transfers", new
        { sourceWarehouseId, destinationWarehouseId, lines = new[] { new { productId, quantity = 3m } } });
        using var json = JsonDocument.Parse(await draft.Content.ReadAsStringAsync());
        var id = json.RootElement.GetProperty("transfer").GetProperty("id").GetGuid();

        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsync($"/api/inventory/transfers/{id}/post", null)).StatusCode);
        using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Empty(await db.StockLedgerEntries.Where(x => x.SourceDocumentType == "StockTransfer" && x.SourceDocumentId == id).ToListAsync());
        Assert.Equal("Draft", (await db.StockTransfers.SingleAsync(x => x.Id == id)).Status.ToString());
    }

    [Fact]
    public async Task RowVersion_RejectsSecondWriterUsingStaleStockBalance()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, warehouseId, _) = await SetupAsync(client);
        await client.PostAsJsonAsync("/api/inventory/opening-balances", Opening(Guid.NewGuid(), productId, warehouseId, 10, 2));

        await using var firstScope = factory.Services.CreateAsyncScope();
        await using var secondScope = factory.Services.CreateAsyncScope();
        var firstDb = firstScope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var secondDb = secondScope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var first = await firstDb.StockBalances.SingleAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId);
        var second = await secondDb.StockBalances.SingleAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId);
        first.Issue(1);
        second.Issue(1);
        await firstDb.SaveChangesAsync();
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => secondDb.SaveChangesAsync());
    }

    [Fact]
    public async Task StockLedger_CannotBeModifiedOrDeletedThroughEfContext()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, warehouseId, _) = await SetupAsync(client);
        await client.PostAsJsonAsync("/api/inventory/opening-balances", Opening(Guid.NewGuid(), productId, warehouseId, 1, 1));
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var entry = await db.StockLedgerEntries.SingleAsync();
        db.Entry(entry).Property(x => x.UnitCost).CurrentValue = 99;
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task LedgerSourceEffect_IsUniquelyConstrainedForDocumentsWithoutLineIds()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, warehouseId, _) = await SetupAsync(client);
        await client.PostAsJsonAsync("/api/inventory/opening-balances", Opening(Guid.NewGuid(), productId, warehouseId, 2, 1));
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var original = await db.StockLedgerEntries.SingleAsync();
        db.StockLedgerEntries.Add(new StockLedgerEntry(original.ProductId, original.WarehouseId, original.MovementType,
            original.Direction, original.Quantity, original.UnitCost, original.SourceDocumentType,
            original.SourceDocumentId, null, null, DateTimeOffset.UtcNow, original.PostedByUserId, "duplicate-test"));
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task FailedSave_RollsBackBalanceLedgerAndAuditTogether()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, warehouseId, _) = await SetupAsync(client);
        var userId = Guid.Parse(new JwtSecurityTokenHandler().ReadJwtToken(
            client.DefaultRequestHeaders.Authorization!.Parameter).Subject);
        var sourceId = Guid.NewGuid();
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            var posting = scope.ServiceProvider.GetRequiredService<IStockPostingService>();
            posting.Receive(productId, warehouseId, 4, 2, "AtomicTest", sourceId, null,
                StockMovementType.Receipt, userId, "atomic-test");
            db.SecurityAuditEntries.Add(new SecurityAuditEntry(userId, new string('X', 101),
                "Inventory", sourceId.ToString(), null, null, "atomic-test"));
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
        await using var verifyScope = factory.Services.CreateAsyncScope();
        var verify = verifyScope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.False(await verify.StockBalances.AnyAsync(x => x.ProductId == productId && x.WarehouseId == warehouseId));
        Assert.False(await verify.StockLedgerEntries.AnyAsync(x => x.SourceDocumentType == "AtomicTest" && x.SourceDocumentId == sourceId));
        Assert.False(await verify.SecurityAuditEntries.AnyAsync(x => x.EntityId == sourceId.ToString()));
    }

    [Fact]
    public async Task ReorderAlerts_UseAvailableStockAndFallAwayAtThreshold()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, warehouseId, _) = await SetupAsync(client);
        await client.PostAsJsonAsync("/api/inventory/opening-balances", Opening(Guid.NewGuid(), productId, warehouseId, 1, 3));
        using var below = JsonDocument.Parse(await (await client.GetAsync($"/api/inventory/reorder-alerts?warehouseId={warehouseId}")).Content.ReadAsStringAsync());
        Assert.Contains(below.RootElement.GetProperty("items").EnumerateArray(), x => x.GetProperty("productId").GetGuid() == productId);
        await client.PostAsJsonAsync("/api/inventory/opening-balances", Opening(Guid.NewGuid(), productId, warehouseId, 1, 3));
        using var atThreshold = JsonDocument.Parse(await (await client.GetAsync($"/api/inventory/reorder-alerts?warehouseId={warehouseId}")).Content.ReadAsStringAsync());
        Assert.DoesNotContain(atThreshold.RootElement.GetProperty("items").EnumerateArray(), x => x.GetProperty("productId").GetGuid() == productId);
    }

    [Fact]
    public async Task InventoryReadAndPostPolicies_RespectWarehouseUserPermissions()
    {
        await factory.ResetDatabaseAsync();
        using var admin = await factory.CreateAuthenticatedClientAsync();
        var (productId, warehouseId, _) = await SetupAsync(admin);
        using var warehouseUser = await factory.CreateAuthenticatedClientAsync("Warehouse User");
        Assert.Equal(HttpStatusCode.OK, (await warehouseUser.GetAsync(BalanceRoute(productId, warehouseId))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await warehouseUser.PostAsJsonAsync("/api/inventory/opening-balances",
            Opening(Guid.NewGuid(), productId, warehouseId, 1, 1))).StatusCode);
    }

    [Fact]
    public async Task OpeningBalanceReversal_IsIdempotent_AndCannotOverdrawAvailableStock()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, warehouseId, _) = await SetupAsync(client);
        var sourceDocumentId = Guid.NewGuid();
        var posted = await client.PostAsJsonAsync("/api/inventory/opening-balances",
            Opening(sourceDocumentId, productId, warehouseId, 5, 2));
        using var opening = JsonDocument.Parse(await posted.Content.ReadAsStringAsync());
        var openingBalanceId = opening.RootElement.GetProperty("id").GetGuid();
        var first = await client.PostAsync($"/api/inventory/opening-balances/{openingBalanceId}/reverse", null);
        var retry = await client.PostAsync($"/api/inventory/opening-balances/{openingBalanceId}/reverse", null);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        using var firstJson = JsonDocument.Parse(await first.Content.ReadAsStringAsync());
        using var retryJson = JsonDocument.Parse(await retry.Content.ReadAsStringAsync());
        Assert.Equal(firstJson.RootElement.GetProperty("reversalLedgerEntryId").GetGuid(),
            retryJson.RootElement.GetProperty("reversalLedgerEntryId").GetGuid());
        Assert.Equal("Reversed", firstJson.RootElement.GetProperty("status").GetString());
        Assert.Equal(0m, await ReadBalanceAsync(client, productId, warehouseId));
    }

    [Fact]
    public async Task PostingRejectsInactiveProductAndTransferBetweenSameWarehouse()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, warehouseId, _) = await SetupAsync(client);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/master-data/products/{productId}/deactivate", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/inventory/opening-balances",
            Opening(Guid.NewGuid(), productId, warehouseId, 1, 1))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/inventory/transfers", new
        { sourceWarehouseId = warehouseId, destinationWarehouseId = warehouseId,
            lines = new[] { new { productId, quantity = 1m } } })).StatusCode);
    }

    private static async Task<decimal> ReadBalanceAsync(HttpClient client, Guid productId, Guid warehouseId)
    {
        using var response = JsonDocument.Parse(await (await client.GetAsync(BalanceRoute(productId, warehouseId))).Content.ReadAsStringAsync());
        return response.RootElement.GetProperty("balance").GetProperty("qtyOnHand").GetDecimal();
    }

    [Fact]
    public async Task NegativeStock_IsBlockedByDefault_AndWarehouseSettingAppliesToFuturePostings()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var (productId, warehouseId, _) = await SetupAsync(client);
        var adjustment = await client.PostAsJsonAsync("/api/inventory/adjustments", new
        { productId, warehouseId, quantityDelta = -1m, reasonCode = "COUNT" });
        using var draft = JsonDocument.Parse(await adjustment.Content.ReadAsStringAsync());
        var adjustmentId = draft.RootElement.GetProperty("adjustment").GetProperty("id").GetGuid();
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync($"/api/inventory/adjustments/{adjustmentId}/post", new { })).StatusCode);

        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync($"/api/inventory/warehouses/{warehouseId}/negative-stock",
            new { allowNegativeStock = true })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync($"/api/inventory/adjustments/{adjustmentId}/post", new { })).StatusCode);
        using var balance = JsonDocument.Parse(await (await client.GetAsync(BalanceRoute(productId, warehouseId))).Content.ReadAsStringAsync());
        Assert.Equal(-1m, balance.RootElement.GetProperty("balance").GetProperty("qtyOnHand").GetDecimal());
    }

    private static object Opening(Guid sourceDocumentId, Guid productId, Guid warehouseId, decimal quantity, decimal unitCost) =>
        new { sourceDocumentId, productId, warehouseId, quantity, unitCost };

    private static string BalanceRoute(Guid productId, Guid warehouseId) =>
        $"/api/inventory/stock-balances/{productId}/{warehouseId}";

    private static async Task<(Guid ProductId, Guid SourceWarehouseId, Guid DestinationWarehouseId)> SetupAsync(HttpClient client)
    {
        var categoryId = await CreateIdAsync(client, "/api/master-data/categories", new { name = $"Inventory {Guid.NewGuid():N}" });
        var uomId = await CreateIdAsync(client, "/api/master-data/units-of-measure", new { code = $"U{Guid.NewGuid():N}"[..12], name = $"Unit {Guid.NewGuid():N}" });
        var taxCodeId = await CreateIdAsync(client, "/api/master-data/tax-codes", new { code = $"T{Guid.NewGuid():N}"[..12], name = $"Tax {Guid.NewGuid():N}", rate = 5m, taxType = "VAT", isActive = true });
        var productId = await CreateIdAsync(client, "/api/master-data/products", new
        {
            productCode = $"P{Guid.NewGuid():N}"[..12], name = "Inventory test product", categoryId,
            stockUomId = uomId, taxCodeId, minimumStockLevel = 2m, reorderQuantity = 4m, salesPrice = 10m, purchasePrice = 5m
        });
        var source = await CreateIdAsync(client, "/api/master-data/warehouses", new { warehouseCode = $"W{Guid.NewGuid():N}"[..12], name = "Source" });
        var destination = await CreateIdAsync(client, "/api/master-data/warehouses", new { warehouseCode = $"W{Guid.NewGuid():N}"[..12], name = "Destination" });
        return (productId, source, destination);
    }

    private static async Task<Guid> CreateIdAsync(HttpClient client, string route, object request)
    {
        var response = await client.PostAsJsonAsync(route, request);
        Assert.True(response.IsSuccessStatusCode, $"{route}: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("id").GetGuid();
    }
}
