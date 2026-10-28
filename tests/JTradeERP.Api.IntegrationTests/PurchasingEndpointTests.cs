using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using JTradeERP.Domain.Inventory;
using JTradeERP.Domain.Purchasing;
using JTradeERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JTradeERP.Api.IntegrationTests;

[Collection(nameof(SqlServerIntegrationCollection))]
public sealed class PurchasingEndpointTests(ApiFactory factory)
{
    [Fact]
    public async Task PurchaseOrderDraft_CalculatesTotalsAndCanBeEditedOnlyBeforeReceipt()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var setup = await SetupAsync(client);
        var orderId = await CreateOrderAsync(client, setup, 100m, 10m);

        using var before = JsonDocument.Parse(await (await client.GetAsync($"/api/purchasing/purchase-orders/{orderId}")).Content.ReadAsStringAsync());
        var order = before.RootElement.GetProperty("purchaseOrder");
        Assert.Equal("Draft", order.GetProperty("status").GetString());
        Assert.Equal(1000m, order.GetProperty("subTotal").GetDecimal());
        Assert.Equal(50m, order.GetProperty("taxAmount").GetDecimal());
        Assert.Equal(1050m, order.GetProperty("grandTotal").GetDecimal());

        var update = await client.PutAsJsonAsync($"/api/purchasing/purchase-orders/{orderId}", OrderBody(setup, 120m, 10m));
        Assert.True(update.StatusCode == HttpStatusCode.OK,
            $"Update PO: {(int)update.StatusCode} {await update.Content.ReadAsStringAsync()}");
        using var after = JsonDocument.Parse(await (await client.GetAsync($"/api/purchasing/purchase-orders/{orderId}")).Content.ReadAsStringAsync());
        Assert.Equal(120m, after.RootElement.GetProperty("purchaseOrder").GetProperty("lines")[0].GetProperty("orderedQuantity").GetDecimal());
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/purchasing/purchase-orders?pageNumber=1&pageSize=20")).StatusCode);
    }

    [Fact]
    public async Task PartialGrn_PostIsIdempotent_UpdatesWeightedAverage_AndCancelReversesStockAndPoProgress()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var setup = await SetupAsync(client);
        var orderId = await CreateOrderAsync(client, setup, 100m, 10m);
        var lineId = await ReadOrderLineIdAsync(client, orderId);
        await client.PostAsJsonAsync("/api/inventory/opening-balances", new
        { sourceDocumentId = Guid.NewGuid(), setup.ProductId, setup.WarehouseId, quantity = 10m, unitCost = 4m });
        var receiptId = await CreateGrnAsync(client, setup, orderId, lineId, 40m);

        var posted = await client.PostAsync($"/api/purchasing/goods-receipt-notes/{receiptId}/post", null);
        Assert.Equal(HttpStatusCode.OK, posted.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/purchasing/goods-receipt-notes/{receiptId}/post", null)).StatusCode);
        using (var orderJson = JsonDocument.Parse(await (await client.GetAsync($"/api/purchasing/purchase-orders/{orderId}")).Content.ReadAsStringAsync()))
        {
            var result = orderJson.RootElement.GetProperty("purchaseOrder");
            Assert.Equal("PartiallyReceived", result.GetProperty("status").GetString());
            Assert.Equal(40m, result.GetProperty("lines")[0].GetProperty("receivedQuantity").GetDecimal());
        }
        using (var balance = JsonDocument.Parse(await (await client.GetAsync(BalanceRoute(setup))).Content.ReadAsStringAsync()))
        {
            Assert.Equal(50m, balance.RootElement.GetProperty("balance").GetProperty("qtyOnHand").GetDecimal());
            Assert.Equal(8.8m, balance.RootElement.GetProperty("balance").GetProperty("weightedAverageCost").GetDecimal());
        }
        Assert.Equal(HttpStatusCode.Conflict, (await client.PutAsJsonAsync($"/api/purchasing/goods-receipt-notes/{receiptId}",
            GrnBody(setup, orderId, lineId, 40m))).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PutAsJsonAsync($"/api/purchasing/purchase-orders/{orderId}",
            OrderBody(setup, 100m, 10m))).StatusCode);

        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/purchasing/goods-receipt-notes/{receiptId}/cancel", null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/purchasing/goods-receipt-notes/{receiptId}/cancel", null)).StatusCode);
        using (var orderJson = JsonDocument.Parse(await (await client.GetAsync($"/api/purchasing/purchase-orders/{orderId}")).Content.ReadAsStringAsync()))
        {
            var result = orderJson.RootElement.GetProperty("purchaseOrder");
            Assert.Equal("Open", result.GetProperty("status").GetString());
            Assert.Equal(0m, result.GetProperty("lines")[0].GetProperty("receivedQuantity").GetDecimal());
        }
        using (var balance = JsonDocument.Parse(await (await client.GetAsync(BalanceRoute(setup))).Content.ReadAsStringAsync()))
        {
            Assert.Equal(10m, balance.RootElement.GetProperty("balance").GetProperty("qtyOnHand").GetDecimal());
            Assert.Equal(4m, balance.RootElement.GetProperty("balance").GetProperty("weightedAverageCost").GetDecimal());
        }

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Equal(2, await db.StockLedgerEntries.CountAsync(x => x.SourceDocumentId == receiptId));
        Assert.Equal(1, await db.StockLedgerEntries.CountAsync(x => x.SourceDocumentType == "GoodsReceiptNoteCancellation" && x.SourceDocumentId == receiptId));
        Assert.Contains(await db.SecurityAuditEntries.Where(x => x.EntityId == receiptId.ToString()).ToListAsync(), x => x.Action == "GoodsReceiptNotePosted");
        Assert.Contains(await db.SecurityAuditEntries.Where(x => x.EntityId == receiptId.ToString()).ToListAsync(), x => x.Action == "GoodsReceiptNoteCancelled");
        var postedAudit = await db.SecurityAuditEntries.SingleAsync(x => x.EntityId == receiptId.ToString() && x.Action == "GoodsReceiptNotePosted");
        Assert.NotEqual(Guid.Empty, postedAudit.ActorUserId);
        Assert.NotEqual(default, postedAudit.OccurredAt);
        Assert.False(string.IsNullOrWhiteSpace(postedAudit.CorrelationId));
        Assert.Equal("Draft", postedAudit.OldValue);
        Assert.Equal("Posted", postedAudit.NewValue);
    }

    [Fact]
    public async Task OverReceiptDisabled_RejectsAtPostWithoutStockPoOrAuditMutation()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var setup = await SetupAsync(client);
        var orderId = await CreateOrderAsync(client, setup, 10m, 2m);
        var lineId = await ReadOrderLineIdAsync(client, orderId);
        var receiptId = await CreateGrnAsync(client, setup, orderId, lineId, 11m);

        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsync($"/api/purchasing/goods-receipt-notes/{receiptId}/post", null)).StatusCode);
        using var noteJson = JsonDocument.Parse(await (await client.GetAsync($"/api/purchasing/goods-receipt-notes/{receiptId}")).Content.ReadAsStringAsync());
        Assert.Equal("Draft", noteJson.RootElement.GetProperty("goodsReceiptNote").GetProperty("status").GetString());
        using var orderJson = JsonDocument.Parse(await (await client.GetAsync($"/api/purchasing/purchase-orders/{orderId}")).Content.ReadAsStringAsync());
        Assert.Equal(0m, orderJson.RootElement.GetProperty("purchaseOrder").GetProperty("lines")[0].GetProperty("receivedQuantity").GetDecimal());
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Empty(await db.StockLedgerEntries.Where(x => x.SourceDocumentId == receiptId).ToListAsync());
        Assert.False(await db.StockBalances.AnyAsync(x => x.ProductId == setup.ProductId && x.WarehouseId == setup.WarehouseId));
        Assert.False(await db.SecurityAuditEntries.AnyAsync(x => x.EntityId == receiptId.ToString() && x.Action == "GoodsReceiptNotePosted"));

        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync("/api/purchasing/configuration/over-receipt",
            new { allowOverReceipt = true })).StatusCode);
        var allowedReceiptId = await CreateGrnAsync(client, setup, orderId, lineId, 11m);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/purchasing/goods-receipt-notes/{allowedReceiptId}/post", null)).StatusCode);
        using var balance = JsonDocument.Parse(await (await client.GetAsync(BalanceRoute(setup))).Content.ReadAsStringAsync());
        Assert.Equal(11m, balance.RootElement.GetProperty("balance").GetProperty("qtyOnHand").GetDecimal());
    }

    [Fact]
    public async Task ConcurrentGrns_CannotCollectivelyOverReceivePurchaseOrder()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var setup = await SetupAsync(client);
        var orderId = await CreateOrderAsync(client, setup, 10m, 5m);
        var lineId = await ReadOrderLineIdAsync(client, orderId);
        var firstId = await CreateGrnAsync(client, setup, orderId, lineId, 6m);
        var secondId = await CreateGrnAsync(client, setup, orderId, lineId, 6m);

        var results = await Task.WhenAll(
            client.PostAsync($"/api/purchasing/goods-receipt-notes/{firstId}/post", null),
            client.PostAsync($"/api/purchasing/goods-receipt-notes/{secondId}/post", null));
        Assert.Single(results, x => x.StatusCode == HttpStatusCode.OK);
        Assert.Single(results, x => x.StatusCode == HttpStatusCode.Conflict);
        using var orderJson = JsonDocument.Parse(await (await client.GetAsync($"/api/purchasing/purchase-orders/{orderId}")).Content.ReadAsStringAsync());
        Assert.Equal(6m, orderJson.RootElement.GetProperty("purchaseOrder").GetProperty("lines")[0].GetProperty("receivedQuantity").GetDecimal());
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Equal(1, await db.StockLedgerEntries.CountAsync(x => x.MovementType == StockMovementType.Receipt));
    }

    [Fact]
    public async Task LinkedGrn_ValidatesSupplierWarehouseProductLineAndPositiveQuantity()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var setup = await SetupAsync(client);
        var other = await SetupAsync(client);
        var orderId = await CreateOrderAsync(client, setup, 10m, 5m);
        var lineId = await ReadOrderLineIdAsync(client, orderId);

        async Task<HttpResponseMessage> Submit(Guid supplierId, Guid warehouseId, Guid productId,
            Guid? purchaseOrderLineId, decimal quantity) => await client.PostAsJsonAsync(
            "/api/purchasing/goods-receipt-notes", new
            {
                purchaseOrderId = orderId, supplierId, warehouseId, receiptDate = new DateOnly(2026, 9, 28),
                lines = new[] { new { productId, purchaseOrderLineId, receivedQuantity = quantity, unitCost = (decimal?)null, taxCodeId = (Guid?)null } }
            });

        Assert.Equal(HttpStatusCode.Conflict, (await Submit(other.SupplierId, setup.WarehouseId,
            setup.ProductId, lineId, 1m)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await Submit(Guid.NewGuid(), setup.WarehouseId,
            setup.ProductId, lineId, 1m)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await Submit(setup.SupplierId, other.WarehouseId,
            setup.ProductId, lineId, 1m)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await Submit(setup.SupplierId, setup.WarehouseId,
            other.ProductId, lineId, 1m)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await Submit(setup.SupplierId, setup.WarehouseId,
            setup.ProductId, null, 1m)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Submit(setup.SupplierId, setup.WarehouseId,
            setup.ProductId, lineId, 0m)).StatusCode);

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Empty(await db.GoodsReceiptNotes.ToListAsync());
    }

    [Fact]
    public async Task GrnPost_ForcedSqlFailureRollsBackStatusPoStockValuationAndAudit()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var setup = await SetupAsync(client);
        var orderId = await CreateOrderAsync(client, setup, 100m, 5m);
        var lineId = await ReadOrderLineIdAsync(client, orderId);
        var receiptId = await CreateGrnAsync(client, setup, orderId, lineId, 40m);
        const string triggerName = "TR_Pur01_RejectTestGoodsReceipt";
        var triggerSql = $"""
            CREATE TRIGGER dbo.{triggerName} ON dbo.StockLedgerEntries AFTER INSERT AS
            BEGIN
                IF EXISTS (SELECT 1 FROM inserted WHERE SourceDocumentType = 'GoodsReceiptNote'
                    AND SourceDocumentId = CONVERT(uniqueidentifier, '{receiptId:D}'))
                    THROW 51000, 'Injected PUR-01 integration failure.', 1;
            END;
            """;

        await using (var setupScope = factory.Services.CreateAsyncScope())
        {
            var db = setupScope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            await db.Database.ExecuteSqlRawAsync(triggerSql);
        }

        HttpResponseMessage post;
        try
        {
            post = await client.PostAsync($"/api/purchasing/goods-receipt-notes/{receiptId}/post", null);
        }
        finally
        {
            await using var cleanupScope = factory.Services.CreateAsyncScope();
            var db = cleanupScope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            await db.Database.ExecuteSqlRawAsync($"DROP TRIGGER IF EXISTS dbo.{triggerName};");
        }

        Assert.Equal(HttpStatusCode.InternalServerError, post.StatusCode);
        await using var verifyScope = factory.Services.CreateAsyncScope();
        var verify = verifyScope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var note = await verify.GoodsReceiptNotes.AsNoTracking().SingleAsync(x => x.Id == receiptId);
        var order = await verify.PurchaseOrders.Include(x => x.Lines).AsNoTracking().SingleAsync(x => x.Id == orderId);
        Assert.Equal(GoodsReceiptNoteStatus.Draft, note.Status);
        Assert.Equal(PurchaseOrderStatus.Draft, order.Status);
        Assert.Equal(0m, order.Lines.Single().ReceivedQuantity);
        Assert.Empty(await verify.StockLedgerEntries.Where(x => x.SourceDocumentId == receiptId).ToListAsync());
        Assert.False(await verify.StockBalances.AnyAsync(x => x.ProductId == setup.ProductId && x.WarehouseId == setup.WarehouseId));
        Assert.False(await verify.SecurityAuditEntries.AnyAsync(x => x.EntityId == receiptId.ToString() && x.Action == "GoodsReceiptNotePosted"));
    }

    [Fact]
    public async Task StandaloneGrnAndAuthorization_UseExistingPermissionCapabilities()
    {
        await factory.ResetDatabaseAsync();
        using var admin = await factory.CreateAuthenticatedClientAsync();
        var setup = await SetupAsync(admin);
        var standaloneId = await CreateGrnAsync(admin, setup, null, null, 3m, 7m);
        Assert.Equal(HttpStatusCode.OK, (await admin.PutAsJsonAsync($"/api/purchasing/goods-receipt-notes/{standaloneId}",
            GrnBody(setup, null, null, 4m, 7m))).StatusCode);
        using var purchaseUser = await factory.CreateAuthenticatedClientAsync("Purchase User");
        using var auditor = await factory.CreateAuthenticatedClientAsync("Auditor");
        Assert.Equal(HttpStatusCode.OK, (await purchaseUser.GetAsync($"/api/purchasing/goods-receipt-notes/{standaloneId}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await purchaseUser.PostAsync($"/api/purchasing/goods-receipt-notes/{standaloneId}/post", null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await auditor.GetAsync("/api/purchasing/purchase-orders")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await auditor.PostAsync($"/api/purchasing/goods-receipt-notes/{standaloneId}/post", null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await admin.PostAsync($"/api/purchasing/goods-receipt-notes/{standaloneId}/post", null)).StatusCode);

        var orderResponse = await purchaseUser.PostAsJsonAsync("/api/purchasing/purchase-orders", OrderBody(setup, 1m, 1m));
        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);
        using var orderJson = JsonDocument.Parse(await orderResponse.Content.ReadAsStringAsync());
        var orderId = orderJson.RootElement.GetProperty("purchaseOrder").GetProperty("id").GetGuid();
        Assert.Equal(HttpStatusCode.OK, (await purchaseUser.PutAsJsonAsync($"/api/purchasing/purchase-orders/{orderId}",
            OrderBody(setup, 2m, 1m))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await purchaseUser.PostAsJsonAsync("/api/purchasing/goods-receipt-notes",
            GrnBody(setup, null, null, 1m))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await purchaseUser.PutAsJsonAsync($"/api/purchasing/goods-receipt-notes/{standaloneId}",
            GrnBody(setup, null, null, 1m, 7m))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await auditor.PostAsJsonAsync("/api/purchasing/purchase-orders", OrderBody(setup, 1m, 1m))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await auditor.PutAsJsonAsync($"/api/purchasing/goods-receipt-notes/{standaloneId}",
            GrnBody(setup, null, null, 1m, 7m))).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await auditor.PostAsync($"/api/purchasing/goods-receipt-notes/{standaloneId}/cancel", null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/api/purchasing/goods-receipt-notes?pageNumber=1&pageSize=10")).StatusCode);
        Assert.NotEqual(Guid.Empty, orderId);
    }

    [Fact]
    public async Task InvalidAndInactiveMasterData_AreControlledBeforePosting()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var setup = await SetupAsync(client);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/master-data/products/{setup.ProductId}/deactivate", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/purchasing/purchase-orders", OrderBody(setup, 1m, 1m))).StatusCode);

        var activeSetup = await SetupAsync(client);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/master-data/warehouses/{activeSetup.WarehouseId}/deactivate", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/purchasing/purchase-orders", OrderBody(activeSetup, 1m, 1m))).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PostAsJsonAsync("/api/purchasing/purchase-orders", OrderBody(activeSetup with { SupplierId = Guid.NewGuid() }, 1m, 1m))).StatusCode);
    }

    private sealed record Setup(Guid ProductId, Guid SupplierId, Guid WarehouseId, Guid TaxCodeId);

    private async Task<Setup> SetupAsync(HttpClient client)
    {
        var categoryId = await CreateIdAsync(client, "/api/master-data/categories", new { name = $"Purchasing {Guid.NewGuid():N}" });
        var uomId = await CreateIdAsync(client, "/api/master-data/units-of-measure", new { code = $"U{Guid.NewGuid():N}"[..12], name = $"Unit {Guid.NewGuid():N}" });
        Guid taxCodeId;
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            taxCodeId = await db.TaxCodes.Where(x => x.Code == "VAT5").Select(x => x.Id).SingleAsync();
        }
        var productId = await CreateIdAsync(client, "/api/master-data/products", new
        {
            productCode = $"P{Guid.NewGuid():N}"[..12], name = "Purchasing test product", categoryId,
            stockUomId = uomId, taxCodeId, minimumStockLevel = 0m, reorderQuantity = 0m, salesPrice = 10m, purchasePrice = 5m
        });
        var supplierId = await CreateIdAsync(client, "/api/master-data/suppliers", new
        { supplierCode = $"S{Guid.NewGuid():N}"[..12], name = "Purchasing test supplier", address = "Address", contactInformation = "Contact", paymentTerms = "Net 30", trn = "TRN-P" });
        var warehouseId = await CreateIdAsync(client, "/api/master-data/warehouses", new
        { warehouseCode = $"W{Guid.NewGuid():N}"[..12], name = "Purchasing warehouse" });
        return new(productId, supplierId, warehouseId, taxCodeId);
    }

    private static object OrderBody(Setup setup, decimal quantity, decimal cost) => new
    {
        supplierId = setup.SupplierId, warehouseId = setup.WarehouseId, orderDate = new DateOnly(2026, 9, 28), expectedDate = (DateOnly?)null,
        lines = new[] { new { productId = setup.ProductId, orderedQuantity = quantity, unitPurchaseCost = cost, taxCodeId = setup.TaxCodeId, discountAmount = 0m } }
    };

    private async Task<Guid> CreateOrderAsync(HttpClient client, Setup setup, decimal quantity, decimal cost)
    {
        var response = await client.PostAsJsonAsync("/api/purchasing/purchase-orders", OrderBody(setup, quantity, cost));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("purchaseOrder").GetProperty("id").GetGuid();
    }

    private async Task<Guid> ReadOrderLineIdAsync(HttpClient client, Guid orderId)
    {
        using var json = JsonDocument.Parse(await (await client.GetAsync($"/api/purchasing/purchase-orders/{orderId}")).Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("purchaseOrder").GetProperty("lines")[0].GetProperty("id").GetGuid();
    }

    private async Task<Guid> CreateGrnAsync(HttpClient client, Setup setup, Guid? orderId,
        Guid? orderLineId, decimal quantity, decimal? unitCost = null)
    {
        var response = await client.PostAsJsonAsync("/api/purchasing/goods-receipt-notes",
            GrnBody(setup, orderId, orderLineId, quantity, unitCost));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("goodsReceiptNote").GetProperty("id").GetGuid();
    }

    private static object GrnBody(Setup setup, Guid? orderId, Guid? orderLineId, decimal quantity, decimal? unitCost = null) => new
    {
        purchaseOrderId = orderId, supplierId = setup.SupplierId, warehouseId = setup.WarehouseId,
        receiptDate = new DateOnly(2026, 9, 28),
        lines = new[] { new { productId = setup.ProductId, purchaseOrderLineId = orderLineId,
            receivedQuantity = quantity, unitCost, taxCodeId = orderId.HasValue ? (Guid?)null : setup.TaxCodeId } }
    };

    private static string BalanceRoute(Setup setup) => $"/api/inventory/stock-balances/{setup.ProductId}/{setup.WarehouseId}";

    private static async Task<Guid> CreateIdAsync(HttpClient client, string route, object body)
    {
        var response = await client.PostAsJsonAsync(route, body);
        Assert.True(response.IsSuccessStatusCode, $"{route}: {(int)response.StatusCode} {await response.Content.ReadAsStringAsync()}");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("id").GetGuid();
    }
}
