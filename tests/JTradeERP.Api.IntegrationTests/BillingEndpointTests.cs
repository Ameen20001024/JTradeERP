using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using JTradeERP.Domain.Billing;
using JTradeERP.Domain.MasterData;
using JTradeERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JTradeERP.Api.IntegrationTests;

[Collection(nameof(SqlServerIntegrationCollection))]
public sealed class BillingEndpointTests(ApiFactory factory)
{
    [Fact]
    public async Task NumberSeries_ReadEndpointExposesConfiguredFieldsWithMatrixPermissions()
    {
        await factory.ResetDatabaseAsync();
        using var auditor = await factory.CreateAuthenticatedClientAsync("Auditor");
        using var response = await auditor.GetAsync("/api/billing/number-series");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var invoiceSeries = Assert.Single(json.RootElement.GetProperty("items").EnumerateArray(),
            x => x.GetProperty("documentType").GetString() == "Invoice");
        Assert.Equal("INV-", invoiceSeries.GetProperty("prefix").GetString());
        Assert.Equal(1, invoiceSeries.GetProperty("nextNumber").GetInt64());
        Assert.Equal(6, invoiceSeries.GetProperty("padding").GetInt32());

        using var salesUser = await factory.CreateAuthenticatedClientAsync("Sales User");
        Assert.Equal(HttpStatusCode.Forbidden, (await salesUser.GetAsync("/api/billing/number-series")).StatusCode);
    }

    [Fact]
    public async Task QuotationToInvoice_PartialPaymentAndCancellation_UsesAtomicSqlServerEffects()
    {
        await factory.ResetDatabaseAsync();
        var references = await SeedAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();

        var opening = await client.PostAsJsonAsync("/api/inventory/opening-balances", new
        {
            sourceDocumentId = Guid.NewGuid(), references.ProductId, references.WarehouseId, quantity = 10m, unitCost = 5m
        });
        Assert.Equal(HttpStatusCode.OK, opening.StatusCode);

        var quotationResponse = await client.PostAsJsonAsync("/api/billing/quotations", new
        {
            references.CustomerId, quotationDate = DateOnly.FromDateTime(DateTime.UtcNow), validityDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            lines = new[] { new { references.ProductId, quantity = 5m, unitPrice = 10m, discountAmount = 0m, references.TaxCodeId } }
        });
        Assert.Equal(HttpStatusCode.Created, quotationResponse.StatusCode);
        using var quotationJson = JsonDocument.Parse(await quotationResponse.Content.ReadAsStringAsync());
        var quotation = quotationJson.RootElement.GetProperty("quotation");
        var quotationId = quotation.GetProperty("id").GetGuid();
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            var audit = await db.SecurityAuditEntries.SingleAsync(x => x.Entity == "Quotation" && x.EntityId == quotationId.ToString() && x.Action == "QuotationCreated");
            Assert.Null(audit.OldValue);
            Assert.Contains(quotationId.ToString(), audit.NewValue);
            Assert.Contains("Lines", audit.NewValue);
        }
        var printableQuotation = await client.GetAsync($"/api/billing/quotations/{quotationId}/print");
        Assert.Equal("text/html", printableQuotation.Content.Headers.ContentType?.MediaType);
        Assert.Contains("Quotation", await printableQuotation.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/billing/quotations/{quotationId}/send", null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/billing/quotations/{quotationId}/accept", null)).StatusCode);

        var orderResponse = await client.PostAsync($"/api/billing/quotations/{quotationId}/convert-to-sales-order?warehouseId={references.WarehouseId}", null);
        Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);
        using var orderJson = JsonDocument.Parse(await orderResponse.Content.ReadAsStringAsync());
        var order = orderJson.RootElement.GetProperty("salesOrder");
        var orderId = order.GetProperty("id").GetGuid();
        var orderLineId = order.GetProperty("lines")[0].GetProperty("id").GetGuid();
        var retryConversion = await client.PostAsync($"/api/billing/quotations/{quotationId}/convert-to-sales-order?warehouseId={references.WarehouseId}", null);
        using (var retryJson = JsonDocument.Parse(await retryConversion.Content.ReadAsStringAsync()))
            Assert.Equal(orderId, retryJson.RootElement.GetProperty("salesOrder").GetProperty("id").GetGuid());

        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/billing/sales-orders/{orderId}/post", null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/billing/sales-orders/{orderId}/post", null)).StatusCode);
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            var reserved = await db.StockBalances.SingleAsync(x => x.ProductId == references.ProductId && x.WarehouseId == references.WarehouseId);
            Assert.Equal(10m, reserved.QtyOnHand);
            Assert.Equal(5m, reserved.QtyReserved);
        }

        var invoiceResponse = await client.PostAsJsonAsync("/api/billing/invoices", new
        {
            references.CustomerId, references.WarehouseId, sourceSalesOrderId = orderId, invoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
            lines = new[] { new { references.ProductId, sourceSalesOrderLineId = orderLineId, quantity = 2m, unitPrice = 10m, discountAmount = 0m, references.TaxCodeId } }
        });
        Assert.Equal(HttpStatusCode.Created, invoiceResponse.StatusCode);
        using var invoiceJson = JsonDocument.Parse(await invoiceResponse.Content.ReadAsStringAsync());
        var invoiceId = invoiceJson.RootElement.GetProperty("invoice").GetProperty("id").GetGuid();
        var postedResponse = await client.PostAsync($"/api/billing/invoices/{invoiceId}/post", null);
        Assert.Equal(HttpStatusCode.OK, postedResponse.StatusCode);
        using var postedJson = JsonDocument.Parse(await postedResponse.Content.ReadAsStringAsync());
        var posted = postedJson.RootElement.GetProperty("invoice");
        Assert.StartsWith("INV-", posted.GetProperty("invoiceNumber").GetString());
        Assert.Equal("COMPANY-TRN", posted.GetProperty("companyTrnSnapshot").GetString());
        Assert.Equal("CUSTOMER-TRN", posted.GetProperty("customerTrnSnapshot").GetString());
        Assert.Equal(5m, posted.GetProperty("lines")[0].GetProperty("taxRateSnapshot").GetDecimal());
        Assert.Equal(21m, posted.GetProperty("grandTotal").GetDecimal());
        var printableInvoice = await client.GetAsync($"/api/billing/invoices/{invoiceId}/print");
        Assert.Equal("text/html", printableInvoice.Content.Headers.ContentType?.MediaType);
        var printableInvoiceHtml = await printableInvoice.Content.ReadAsStringAsync();
        Assert.Contains("COMPANY-TRN", printableInvoiceHtml);
        Assert.Contains("CUSTOMER-TRN", printableInvoiceHtml);
        using (var postRetry = JsonDocument.Parse(await (await client.PostAsync($"/api/billing/invoices/{invoiceId}/post", null)).Content.ReadAsStringAsync()))
            Assert.Equal(posted.GetProperty("invoiceNumber").GetString(), postRetry.RootElement.GetProperty("invoice").GetProperty("invoiceNumber").GetString());

        Guid paymentMethodId;
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            paymentMethodId = await db.PaymentMethods.Where(x => x.Code == "CASH").Select(x => x.Id).SingleAsync();
            var balance = await db.StockBalances.SingleAsync(x => x.ProductId == references.ProductId && x.WarehouseId == references.WarehouseId);
            Assert.Equal(8m, balance.QtyOnHand);
            Assert.Equal(3m, balance.QtyReserved);
            Assert.Equal(1, await db.StockLedgerEntries.CountAsync(x => x.SourceDocumentId == invoiceId && x.MovementType == JTradeERP.Domain.Inventory.StockMovementType.Issue));
        }

        var paymentResponse = await client.PostAsJsonAsync("/api/billing/payments", new
        {
            paymentDate = DateOnly.FromDateTime(DateTime.UtcNow), paymentMethodId, amount = 5m, referenceNumber = "TEST-PAY-1",
            allocations = new[] { new { invoiceId, amount = 5m } }
        });
        Assert.Equal(HttpStatusCode.OK, paymentResponse.StatusCode);
        using var paymentJson = JsonDocument.Parse(await paymentResponse.Content.ReadAsStringAsync());
        var paymentId = paymentJson.RootElement.GetProperty("id").GetGuid();
        Assert.Equal("Posted", paymentJson.RootElement.GetProperty("status").GetString());
        var overpayment = await client.PostAsJsonAsync("/api/billing/payments", new
        {
            paymentDate = DateOnly.FromDateTime(DateTime.UtcNow), paymentMethodId, amount = 17m, referenceNumber = "TEST-OVERPAY",
            allocations = new[] { new { invoiceId, amount = 17m } }
        });
        Assert.Equal(HttpStatusCode.Conflict, overpayment.StatusCode);
        var concurrentPayments = await Task.WhenAll(Enumerable.Range(1, 2).Select(index =>
            client.PostAsJsonAsync("/api/billing/payments", new
            {
                paymentDate = DateOnly.FromDateTime(DateTime.UtcNow), paymentMethodId, amount = 10m,
                referenceNumber = $"TEST-CONCURRENT-{index}", allocations = new[] { new { invoiceId, amount = 10m } }
            })));
        Assert.Single(concurrentPayments, result => result.StatusCode == HttpStatusCode.OK);
        Assert.Single(concurrentPayments, result => result.StatusCode == HttpStatusCode.Conflict);
        using var concurrentPaymentJson = JsonDocument.Parse(await concurrentPayments.Single(x => x.StatusCode == HttpStatusCode.OK).Content.ReadAsStringAsync());
        var concurrentPaymentId = concurrentPaymentJson.RootElement.GetProperty("id").GetGuid();
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/billing/payments/{concurrentPaymentId}/reverse", null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/billing/payments/{paymentId}/reverse", null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/billing/payments/{paymentId}/reverse", null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/billing/invoices/{invoiceId}/cancel", null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/billing/invoices/{invoiceId}/cancel", null)).StatusCode);

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            var balance = await db.StockBalances.SingleAsync(x => x.ProductId == references.ProductId && x.WarehouseId == references.WarehouseId);
            Assert.Equal(10m, balance.QtyOnHand);
            Assert.Equal(5m, balance.QtyReserved);
            Assert.Equal(1, await db.StockLedgerEntries.CountAsync(x => x.SourceDocumentId == invoiceId && x.MovementType == JTradeERP.Domain.Inventory.StockMovementType.Reversal));
            Assert.Equal(InvoiceStatus.Cancelled, (await db.Invoices.SingleAsync(x => x.Id == invoiceId)).Status);
            Assert.Equal(SalesOrderStatus.Posted, (await db.SalesOrders.SingleAsync(x => x.Id == orderId)).Status);
            Assert.Contains(await db.SecurityAuditEntries.Where(x => x.EntityId == invoiceId.ToString()).ToListAsync(), x => x.Action == "InvoicePosted");
        }
    }

    [Fact]
    public async Task InvoicePost_InsufficientStock_RollsBackNumberAndInvoiceState()
    {
        await factory.ResetDatabaseAsync();
        var references = await SeedAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/billing/invoices", new
        {
            references.CustomerId, references.WarehouseId, sourceSalesOrderId = (Guid?)null,
            invoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
            lines = new[] { new { references.ProductId, sourceSalesOrderLineId = (Guid?)null, quantity = 1m, unitPrice = 10m, discountAmount = 0m, references.TaxCodeId } }
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var invoiceId = json.RootElement.GetProperty("invoice").GetProperty("id").GetGuid();

        var posting = await client.PostAsync($"/api/billing/invoices/{invoiceId}/post", null);
        Assert.Equal(HttpStatusCode.Conflict, posting.StatusCode);
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var invoice = await db.Invoices.SingleAsync(x => x.Id == invoiceId);
        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
        Assert.Null(invoice.InvoiceNumber);
        Assert.Equal(1, await db.NumberSeries.Where(x => x.DocumentType == "Invoice").Select(x => x.NextNumber).SingleAsync());
        Assert.Empty(await db.StockLedgerEntries.Where(x => x.SourceDocumentId == invoiceId).ToListAsync());
    }

    [Fact]
    public async Task InvoicePost_ForcedSqlFailureRollsBackNumberInvoiceStockLedgerAndAudit()
    {
        await factory.ResetDatabaseAsync();
        var references = await SeedAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/inventory/opening-balances", new
        {
            sourceDocumentId = Guid.NewGuid(), references.ProductId, references.WarehouseId, quantity = 5m, unitCost = 5m
        })).StatusCode);
        var response = await client.PostAsJsonAsync("/api/billing/invoices", new
        {
            references.CustomerId, references.WarehouseId, sourceSalesOrderId = (Guid?)null,
            invoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
            lines = new[] { new { references.ProductId, sourceSalesOrderLineId = (Guid?)null, quantity = 1m, unitPrice = 10m, discountAmount = 0m, references.TaxCodeId } }
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var invoiceJson = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var invoiceId = invoiceJson.RootElement.GetProperty("invoice").GetProperty("id").GetGuid();

        const string triggerName = "TR_Test_FailInvoiceLedger";
        await using (var setupScope = factory.Services.CreateAsyncScope())
        {
            var db = setupScope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            await db.Database.ExecuteSqlRawAsync($"""
                CREATE TRIGGER dbo.{triggerName} ON dbo.StockLedgerEntries AFTER INSERT AS
                BEGIN
                    IF EXISTS (SELECT 1 FROM inserted WHERE SourceDocumentType = 'Invoice')
                    BEGIN
                        THROW 51002, 'Forced invoice posting transaction failure', 1;
                    END;
                END;
                """);
        }

        HttpResponseMessage? failedPost = null;
        try
        {
            failedPost = await client.PostAsync($"/api/billing/invoices/{invoiceId}/post", null);
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
        var invoice = await verify.Invoices.SingleAsync(x => x.Id == invoiceId);
        Assert.Equal(InvoiceStatus.Draft, invoice.Status);
        Assert.Null(invoice.InvoiceNumber);
        Assert.Equal(1, await verify.NumberSeries.Where(x => x.DocumentType == "Invoice").Select(x => x.NextNumber).SingleAsync());
        Assert.Equal(5m, (await verify.StockBalances.SingleAsync(x => x.ProductId == references.ProductId && x.WarehouseId == references.WarehouseId)).QtyOnHand);
        Assert.Empty(await verify.StockLedgerEntries.Where(x => x.SourceDocumentType == "Invoice" && x.SourceDocumentId == invoiceId).ToListAsync());
        Assert.False(await verify.SecurityAuditEntries.AnyAsync(x => x.Entity == "Invoice" && x.EntityId == invoiceId.ToString() && x.Action == "InvoicePosted"));
    }

    [Fact]
    public async Task ConcurrentInvoicePosts_AllocateOnlyOneNumberAndIssueAvailableStockOnce()
    {
        await factory.ResetDatabaseAsync();
        var references = await SeedAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var opening = await client.PostAsJsonAsync("/api/inventory/opening-balances", new
        {
            sourceDocumentId = Guid.NewGuid(), references.ProductId, references.WarehouseId, quantity = 1m, unitCost = 5m
        });
        Assert.Equal(HttpStatusCode.OK, opening.StatusCode);

        async Task<Guid> CreateInvoiceAsync()
        {
            var response = await client.PostAsJsonAsync("/api/billing/invoices", new
            {
                references.CustomerId, references.WarehouseId, sourceSalesOrderId = (Guid?)null,
                invoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
                lines = new[] { new { references.ProductId, sourceSalesOrderLineId = (Guid?)null, quantity = 1m, unitPrice = 10m, discountAmount = 0m, references.TaxCodeId } }
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return document.RootElement.GetProperty("invoice").GetProperty("id").GetGuid();
        }

        var invoiceIds = await Task.WhenAll(CreateInvoiceAsync(), CreateInvoiceAsync());
        var results = await Task.WhenAll(invoiceIds.Select(id => client.PostAsync($"/api/billing/invoices/{id}/post", null)));
        Assert.Single(results, result => result.StatusCode == HttpStatusCode.OK);
        Assert.Single(results, result => result.StatusCode == HttpStatusCode.Conflict);

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Equal(2, await db.NumberSeries.Where(x => x.DocumentType == "Invoice").Select(x => x.NextNumber).SingleAsync());
        Assert.Equal(1, await db.StockLedgerEntries.CountAsync(x => x.MovementType == JTradeERP.Domain.Inventory.StockMovementType.Issue));
        Assert.Single(await db.Invoices.Where(x => x.Status == InvoiceStatus.Draft).ToListAsync());
        Assert.Single(await db.Invoices.Where(x => x.Status == InvoiceStatus.Posted).ToListAsync());
    }

    [Fact]
    public async Task CreditNotePost_UsesInvoiceTaxAndReceiptCost_AndRetryIsIdempotent()
    {
        await factory.ResetDatabaseAsync();
        var references = await SeedAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/inventory/opening-balances", new
        {
            sourceDocumentId = Guid.NewGuid(), references.ProductId, references.WarehouseId, quantity = 5m, unitCost = 7m
        })).StatusCode);
        var invoiceResponse = await client.PostAsJsonAsync("/api/billing/invoices", new
        {
            references.CustomerId, references.WarehouseId, sourceSalesOrderId = (Guid?)null,
            invoiceDate = DateOnly.FromDateTime(DateTime.UtcNow),
            lines = new[] { new { references.ProductId, sourceSalesOrderLineId = (Guid?)null, quantity = 2m, unitPrice = 10m, discountAmount = 0m, references.TaxCodeId } }
        });
        using var invoiceJson = JsonDocument.Parse(await invoiceResponse.Content.ReadAsStringAsync());
        var invoice = invoiceJson.RootElement.GetProperty("invoice");
        var invoiceId = invoice.GetProperty("id").GetGuid();
        var invoiceLineId = invoice.GetProperty("lines")[0].GetProperty("id").GetGuid();
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/billing/invoices/{invoiceId}/post", null)).StatusCode);

        var noteResponse = await client.PostAsJsonAsync("/api/billing/credit-notes", new
        {
            invoiceId, creditNoteDate = DateOnly.FromDateTime(DateTime.UtcNow),
            lines = new[] { new { invoiceLineId, quantity = 1m } }
        });
        Assert.Equal(HttpStatusCode.OK, noteResponse.StatusCode);
        using var noteJson = JsonDocument.Parse(await noteResponse.Content.ReadAsStringAsync());
        var noteId = noteJson.RootElement.GetProperty("id").GetGuid();
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/billing/credit-notes/{noteId}/post", null)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/billing/credit-notes/{noteId}/post", null)).StatusCode);

        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Equal(1, await db.StockLedgerEntries.CountAsync(x => x.SourceDocumentType == "CreditNote" && x.SourceDocumentId == noteId));
        var receipt = await db.StockLedgerEntries.SingleAsync(x => x.SourceDocumentType == "CreditNote" && x.SourceDocumentId == noteId);
        Assert.Equal(7m, receipt.UnitCost);
        Assert.Equal(1m, receipt.Quantity);
        Assert.Equal(InvoiceStatus.Posted, (await db.Invoices.SingleAsync(x => x.Id == invoiceId)).Status);
    }

    private async Task<(Guid CustomerId, Guid ProductId, Guid WarehouseId, Guid TaxCodeId)> SeedAsync()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        var category = new Category("Billing test category");
        var uom = new UnitOfMeasure("BIL-EA", "Billing Each");
        var tax = new TaxCode("BIL-VAT5", "Billing VAT 5%", 5m, "VAT", true);
        var product = new Product("BIL-P-001", "Test product", category.Id, uom.Id, tax.Id, 0, 0, 10m, 5m);
        var customer = new Customer("BIL-C-001", "Billing customer", null, null, null, null, "CUSTOMER-TRN", null);
        var warehouse = new Warehouse("BIL-WH-001", "Billing warehouse");
        db.AddRange(category, uom, tax, product, customer, warehouse);
        var config = await db.BillingConfigurations.SingleAsync(x => x.Id == BillingConfiguration.SingletonId);
        config.SetCompanyTrn("COMPANY-TRN");
        await db.SaveChangesAsync();
        return (customer.Id, product.Id, warehouse.Id, tax.Id);
    }
}
