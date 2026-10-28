using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using JTradeERP.Domain.Billing;
using JTradeERP.Domain.Inventory;
using JTradeERP.Domain.MasterData;
using JTradeERP.Application.Common.Reports;
using JTradeERP.Application.Features.Reports.SalesSummary;
using JTradeERP.Application.Features.Reports.CustomerAging;
using JTradeERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JTradeERP.Api.IntegrationTests;

[Collection(nameof(SqlServerIntegrationCollection))]
public sealed class ReportsRelationalQueryTests(ApiFactory factory)
{
    [Fact]
    public async Task CustomerAging_UsesApprovedBoundariesAndHistoricalAsOfDate()
    {
        await factory.ResetDatabaseAsync();
        using var auditor = await factory.CreateAuthenticatedClientAsync("Auditor");
        var actor = Guid.Parse(new JwtSecurityTokenHandler().ReadJwtToken(auditor.DefaultRequestHeaders.Authorization!.Parameter).Subject);
        var asOf = new DateOnly(2026, 6, 30);
        var category = new Category($"Aging-{Guid.NewGuid():N}");
        var unit = new UnitOfMeasure($"A{Guid.NewGuid():N}"[..16], "Unit");
        var tax = new TaxCode($"A{Guid.NewGuid():N}"[..16], "Aging VAT", 5m, "VAT", true);
        var product = new Product($"A{Guid.NewGuid():N}"[..16], "Aging Product", category.Id, unit.Id, tax.Id, 10m, 4m, 100m, 50m);
        var customer = new Customer($"A{Guid.NewGuid():N}"[..16], "Aging Customer", null, null, null, null, null, null);
        var secondCustomer = new Customer($"B{Guid.NewGuid():N}"[..16], "Zeta Aging Customer", null, null, null, null, null, null);
        var warehouse = new Warehouse($"A{Guid.NewGuid():N}"[..16], "Aging Warehouse");
        var ages = new int[] { 0, 1, 30, 31, 60, 61, 90, 91, -1 };
        var invoices = ages.Select((age, index) => CreateInvoice(customer, warehouse, product, tax, asOf.AddDays(-age),
            $"AGE-{Guid.NewGuid():N}"[..20], asOf.AddDays(-2).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), actor)).ToArray();
        // This invoice is cancelled after the report date: historically it remained open.
        invoices[0].Cancel(asOf.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), actor);
        var cancelledBeforeAsOf = CreateInvoice(customer, warehouse, product, tax, asOf.AddDays(-2),
            $"AGE-{Guid.NewGuid():N}"[..20], asOf.AddDays(-5).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), actor);
        cancelledBeforeAsOf.Cancel(asOf.AddDays(-1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), actor);
        var postedAfterAsOf = CreateInvoice(customer, warehouse, product, tax, asOf.AddDays(-10),
            $"AGE-{Guid.NewGuid():N}"[..20], asOf.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), actor);
        var secondCustomerInvoice = CreateInvoice(secondCustomer, warehouse, product, tax, asOf,
            $"AGE-{Guid.NewGuid():N}"[..20], asOf.AddDays(-2).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), actor);

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            db.AddRange(category, unit, tax, product, customer, secondCustomer, warehouse);
            db.Invoices.AddRange(invoices);
            db.Invoices.AddRange(cancelledBeforeAsOf, postedAfterAsOf, secondCustomerInvoice);
            await db.SaveChangesAsync();
            var service = scope.ServiceProvider.GetRequiredService<IReportReadService>();
            var result = await service.GetCustomerAgingAsync(new GetCustomerAgingQuery(asOf, customer.Id), CancellationToken.None);
            var row = Assert.Single(result.Items);
            Assert.Equal(asOf, row.AsOfDate);
            Assert.Equal(21m, row.Current); // age 0 and a future-dated invoice
            Assert.Equal(21m, row.Days1To30);
            Assert.Equal(21m, row.Days31To60);
            Assert.Equal(21m, row.Days61To90);
            Assert.Equal(10.5m, row.Days91Plus);
            Assert.Equal(94.5m, row.TotalOutstanding);
            Assert.Equal(row.TotalOutstanding, row.Current + row.Days1To30 + row.Days31To60 + row.Days61To90 + row.Days91Plus);
            Assert.Equal(0, (await service.GetCustomerAgingAsync(new GetCustomerAgingQuery(asOf, Guid.NewGuid()), CancellationToken.None)).TotalCount);
            var sortedPage = await service.GetCustomerAgingAsync(new GetCustomerAgingQuery(asOf, Page: 1, PageSize: 1,
                SortBy: "totalOutstanding", SortDirection: "desc"), CancellationToken.None);
            Assert.Equal(2, sortedPage.TotalCount);
            var firstPageRow = Assert.Single(sortedPage.Items);
            Assert.Equal(customer.Id, firstPageRow.CustomerId);
            var nextPage = await service.GetCustomerAgingAsync(new GetCustomerAgingQuery(asOf, Page: 2, PageSize: 1,
                SortBy: "totalOutstanding", SortDirection: "desc"), CancellationToken.None);
            var secondPageRow = Assert.Single(nextPage.Items);
            Assert.Equal(secondCustomer.Id, secondPageRow.CustomerId);
            Assert.NotEqual(firstPageRow.CustomerId, secondPageRow.CustomerId);
        }

        using var response = await auditor.GetAsync($"/api/reports/customer-aging?asOfDate={asOf:yyyy-MM-dd}&customerId={customer.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(94.5m, json.RootElement.GetProperty("items")[0].GetProperty("totalOutstanding").GetDecimal());
    }

    [Fact]
    public async Task CustomerAging_ReconstructsPartialPaidReversedAndFutureActivityAtAsOfDate()
    {
        await factory.ResetDatabaseAsync();
        using var auditor = await factory.CreateAuthenticatedClientAsync("Auditor");
        var actor = Guid.Parse(new JwtSecurityTokenHandler().ReadJwtToken(auditor.DefaultRequestHeaders.Authorization!.Parameter).Subject);
        var asOf = new DateOnly(2026, 6, 30);
        var category = new Category($"AgePay-{Guid.NewGuid():N}");
        var unit = new UnitOfMeasure($"P{Guid.NewGuid():N}"[..16], "Unit");
        var tax = new TaxCode($"P{Guid.NewGuid():N}"[..16], "Aging VAT", 5m, "VAT", true);
        var product = new Product($"P{Guid.NewGuid():N}"[..16], "Aging Payment Product", category.Id, unit.Id, tax.Id, 10m, 4m, 100m, 50m);
        var customer = new Customer($"P{Guid.NewGuid():N}"[..16], "Aging Payment Customer", null, null, null, null, null, null);
        var warehouse = new Warehouse($"P{Guid.NewGuid():N}"[..16], "Aging Payment Warehouse");
        var invoice = CreateInvoice(customer, warehouse, product, tax, asOf.AddDays(-10), $"PAY-{Guid.NewGuid():N}"[..20],
            asOf.AddDays(-20).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), actor);
        var amounts = new[] { 2.5m, 1m, 1.5m, 0.5m };
        var paymentDates = new[] { asOf.AddDays(-1), asOf.AddDays(-3), asOf.AddDays(-4), asOf.AddDays(-5) };
        var payments = new List<Payment>();
        for (var i = 0; i < amounts.Length; i++)
        {
            var activityAt = (i == 3 ? asOf.AddDays(1) : asOf.AddDays(-2))
                .ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            invoice.ApplyPayment(amounts[i], activityAt);
            var payment = new Payment(paymentDates[i], new Guid("44c247b3-3bf6-48b7-9be7-010000000001"), amounts[i], null,
                [new PaymentAllocation(invoice.Id, amounts[i])], activityAt);
            if (i == 1) payment.Reverse(asOf.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), actor);
            if (i == 2) payment.Reverse(asOf.AddDays(-1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), actor);
            payments.Add(payment);
        }
        invoice.ReversePayment(1m, asOf.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
        invoice.ReversePayment(1.5m, asOf.AddDays(-1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
        var fullyPaid = CreateInvoice(customer, warehouse, product, tax, asOf.AddDays(-40), $"FULL-{Guid.NewGuid():N}"[..20],
            asOf.AddDays(-50).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), actor);
        fullyPaid.ApplyPayment(fullyPaid.GrandTotal, asOf.AddDays(-2).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
        var fullPayment = new Payment(asOf.AddDays(-2), new Guid("44c247b3-3bf6-48b7-9be7-010000000001"), fullyPaid.GrandTotal, null,
            [new PaymentAllocation(fullyPaid.Id, fullyPaid.GrandTotal)], asOf.AddDays(-2).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            db.AddRange(category, unit, tax, product, customer, warehouse, invoice, fullyPaid);
            db.Payments.AddRange(payments);
            db.Payments.Add(fullPayment);
            await db.SaveChangesAsync();
            var result = await scope.ServiceProvider.GetRequiredService<IReportReadService>()
                .GetCustomerAgingAsync(new GetCustomerAgingQuery(asOf, customer.Id), CancellationToken.None);
            var row = Assert.Single(result.Items);
            Assert.Equal(7m, row.Days1To30); // 10.50 - (2.50 valid + 1.00 reversed only after AsOfDate)
            Assert.Equal(7m, row.TotalOutstanding);
        }

        using var csv = await auditor.GetAsync($"/api/reports/customer-aging/export.csv?asOfDate={asOf:yyyy-MM-dd}&customerId={customer.Id}");
        Assert.Equal(HttpStatusCode.OK, csv.StatusCode);
        Assert.Equal("text/csv", csv.Content.Headers.ContentType?.MediaType);
        var csvContent = await csv.Content.ReadAsStringAsync();
        Assert.Contains("TotalOutstanding", csvContent);
        var exportedOutstanding = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries)[1].TrimEnd('\r').Split(',')[^1].Trim('"');
        Assert.Equal(7m, decimal.Parse(exportedOutstanding, System.Globalization.CultureInfo.InvariantCulture));
    }

    private static Invoice CreateInvoice(Customer customer, Warehouse warehouse, Product product, TaxCode tax,
        DateOnly invoiceDate, string number, DateTimeOffset postedAt, Guid actor)
    {
        var invoice = new Invoice(customer.Id, warehouse.Id, null, invoiceDate,
            [new InvoiceLine(product.Id, null, 1m, 10m, 0m, tax.Id, 5m)], postedAt.AddMinutes(-1));
        invoice.SnapshotTaxRates(new Dictionary<Guid, decimal> { [tax.Id] = 5m });
        invoice.Post(number, null, null, postedAt, actor);
        return invoice;
    }

    [Fact]
    public async Task Reports_UsePersistedInvoiceSnapshotsStockBalancesAndAppendOnlyLedger()
    {
        await factory.ResetDatabaseAsync();
        using var auditor = await factory.CreateAuthenticatedClientAsync("Auditor");
        var userId = Guid.Parse(new JwtSecurityTokenHandler().ReadJwtToken(auditor.DefaultRequestHeaders.Authorization!.Parameter).Subject);
        var category = new Category($"Reporting-{Guid.NewGuid():N}");
        var unit = new UnitOfMeasure($"U{Guid.NewGuid():N}"[..16], "Unit");
        var tax = new TaxCode($"T{Guid.NewGuid():N}"[..16], "UAE VAT", 5m, "VAT", true);
        var product = new Product($"P{Guid.NewGuid():N}"[..16], "Reporting Product", category.Id, unit.Id, tax.Id, 10m, 4m, 100m, 50m);
        var customer = new Customer($"C{Guid.NewGuid():N}"[..16], "Reporting Customer", null, null, null, null, null, null);
        var warehouse = new Warehouse($"W{Guid.NewGuid():N}"[..16], "Reporting Warehouse");
        var balance = new StockBalance(product.Id, warehouse.Id);
        balance.Receive(4m, 12.5m);
        Assert.True(balance.TryReserve(1m));
        var documentId = Guid.NewGuid();
        var ledger = new StockLedgerEntry(product.Id, warehouse.Id, StockMovementType.OpeningBalance,
            StockMovementDirection.In, 4m, 12.5m, "OpeningBalance", documentId, null, null,
            DateTimeOffset.UtcNow, userId, Guid.NewGuid().ToString("N"));
        var invoiceDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-2));
        var invoice = new Invoice(customer.Id, warehouse.Id, null, invoiceDate,
            [new InvoiceLine(product.Id, null, 1m, 100m, 0m, tax.Id, 5m)], DateTimeOffset.UtcNow);
        invoice.SnapshotTaxRates(new Dictionary<Guid, decimal> { [tax.Id] = 5m });
        invoice.Post("RPT-000001", null, null, DateTimeOffset.UtcNow, userId);
        var cancelledInvoice = new Invoice(customer.Id, warehouse.Id, null, invoiceDate,
            [new InvoiceLine(product.Id, null, 2m, 100m, 0m, tax.Id, 5m)], DateTimeOffset.UtcNow);
        cancelledInvoice.SnapshotTaxRates(new Dictionary<Guid, decimal> { [tax.Id] = 5m });
        cancelledInvoice.Post("RPT-000002", null, null, DateTimeOffset.UtcNow, userId);
        cancelledInvoice.Cancel(DateTimeOffset.UtcNow, userId);
        balance.Issue(1m);
        balance.Receive(1m, 12.5m); // Reversal restores on-hand quantity without rewriting the original movement.
        var issueLedger = new StockLedgerEntry(product.Id, warehouse.Id, StockMovementType.Issue,
            StockMovementDirection.Out, 1m, 12.5m, "Invoice", invoice.Id, invoice.Lines.Single().Id, null,
            DateTimeOffset.UtcNow, userId, Guid.NewGuid().ToString("N"));
        var reversalLedger = new StockLedgerEntry(product.Id, warehouse.Id, StockMovementType.Reversal,
            StockMovementDirection.In, 1m, 12.5m, "InvoiceReversal", invoice.Id, invoice.Lines.Single().Id, issueLedger.Id,
            DateTimeOffset.UtcNow, userId, Guid.NewGuid().ToString("N"));

        int invoiceCountBefore;
        int ledgerCountBefore;
        int balanceCountBefore;
        int auditCountBefore;

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            db.AddRange(category, unit, tax, product, customer, warehouse, balance, ledger, invoice, cancelledInvoice, issueLedger, reversalLedger);
            await db.SaveChangesAsync();
            invoiceCountBefore = await db.Invoices.CountAsync();
            ledgerCountBefore = await db.StockLedgerEntries.CountAsync();
            balanceCountBefore = await db.StockBalances.CountAsync();
            auditCountBefore = await db.SecurityAuditEntries.CountAsync();
            tax.Update(tax.Code, tax.Name, 20m, tax.TaxType); // Posted invoice reporting must use persisted line snapshots.
            await db.SaveChangesAsync();
            var reporting = scope.ServiceProvider.GetRequiredService<IReportReadService>();
            var salesReport = await reporting.GetSalesSummaryAsync(new GetSalesSummaryQuery(invoiceDate, invoiceDate), CancellationToken.None);
            Assert.Equal(1, salesReport.TotalCount);
            var salesRow = Assert.Single(salesReport.Items);
            Assert.Equal(100m, salesRow.NetAmount);
            Assert.Equal(105m, salesRow.GrandTotal);
        }

        using var sales = await auditor.GetAsync($"/api/reports/sales-summary?fromDate={invoiceDate:yyyy-MM-dd}&toDate={invoiceDate:yyyy-MM-dd}&customerId={customer.Id}&productId={product.Id}");
        Assert.True(sales.StatusCode == HttpStatusCode.OK, await sales.Content.ReadAsStringAsync());
        using var salesJson = JsonDocument.Parse(await sales.Content.ReadAsStringAsync());
        Assert.Equal(105m, salesJson.RootElement.GetProperty("items")[0].GetProperty("grandTotal").GetDecimal());

        using var valuation = await auditor.GetAsync($"/api/reports/stock-valuation?warehouseId={warehouse.Id}");
        Assert.Equal(HttpStatusCode.OK, valuation.StatusCode);
        using var valuationJson = JsonDocument.Parse(await valuation.Content.ReadAsStringAsync());
        Assert.Equal(50m, valuationJson.RootElement.GetProperty("items")[0].GetProperty("inventoryValue").GetDecimal());

        using var lowStock = await auditor.GetAsync($"/api/reports/low-stock?warehouseId={warehouse.Id}");
        Assert.Equal(HttpStatusCode.OK, lowStock.StatusCode);
        using var lowStockJson = JsonDocument.Parse(await lowStock.Content.ReadAsStringAsync());
        Assert.Equal(3m, lowStockJson.RootElement.GetProperty("items")[0].GetProperty("availableQuantity").GetDecimal());
        await using (var releaseScope = factory.Services.CreateAsyncScope())
        {
            var db = releaseScope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
            var currentBalance = await db.StockBalances.SingleAsync(x => x.ProductId == product.Id && x.WarehouseId == warehouse.Id);
            Assert.True(currentBalance.TryReleaseReservation(1m));
            await db.SaveChangesAsync();
        }
        using var releasedLowStock = await auditor.GetAsync($"/api/reports/low-stock?warehouseId={warehouse.Id}");
        using var releasedLowStockJson = JsonDocument.Parse(await releasedLowStock.Content.ReadAsStringAsync());
        Assert.Equal(4m, releasedLowStockJson.RootElement.GetProperty("items")[0].GetProperty("availableQuantity").GetDecimal());

        using var movement = await auditor.GetAsync($"/api/reports/stock-movement?productId={product.Id}&warehouseId={warehouse.Id}&fromUtc={Uri.EscapeDataString(DateTimeOffset.UtcNow.AddMinutes(-10).ToString("O"))}&toUtc={Uri.EscapeDataString(DateTimeOffset.UtcNow.AddMinutes(10).ToString("O"))}");
        Assert.Equal(HttpStatusCode.OK, movement.StatusCode);
        using var movementJson = JsonDocument.Parse(await movement.Content.ReadAsStringAsync());
        var movementRows = movementJson.RootElement.GetProperty("items").EnumerateArray().ToArray();
        Assert.Equal(3, movementRows.Length);
        Assert.Contains(movementRows, x => x.GetProperty("sourceDocumentId").GetGuid() == documentId);
        Assert.Contains(movementRows, x => x.TryGetProperty("reversalOfLedgerEntryId", out var reversalId)
            && reversalId.ValueKind == JsonValueKind.String && reversalId.GetGuid() == issueLedger.Id);

        await using var verifyScope = factory.Services.CreateAsyncScope();
        var verify = verifyScope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();
        Assert.Equal(invoiceCountBefore, await verify.Invoices.CountAsync());
        Assert.Equal(ledgerCountBefore, await verify.StockLedgerEntries.CountAsync());
        Assert.Equal(balanceCountBefore, await verify.StockBalances.CountAsync());
        Assert.Equal(auditCountBefore, await verify.SecurityAuditEntries.CountAsync());
    }
}
