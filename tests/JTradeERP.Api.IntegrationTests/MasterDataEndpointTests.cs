using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using JTradeERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JTradeERP.Api.IntegrationTests;

[Collection(nameof(SqlServerIntegrationCollection))]
public sealed class MasterDataEndpointTests(ApiFactory factory)
{
    [Fact]
    public async Task Database_MigratesAndSeedsUaeVat5()
    {
        await factory.ResetDatabaseAsync();
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<JTradeERPDbContext>();

        var vat = await db.TaxCodes.SingleAsync(x => x.Code == "VAT5");

        Assert.Equal(5m, vat.Rate);
        Assert.True(vat.IsActive);
    }

    [Fact]
    public async Task Category_UniquenessAndReferencedDelete_AreControlledBySqlServerBackedApi()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var categoryId = await CreateAsync(client, "/api/master-data/categories", new { name = "Consumables" });

        AssertControlledConflict(await client.PostAsJsonAsync("/api/master-data/categories", new { name = "Consumables" }));

        var (uomId, taxCodeId) = await CreateProductDependenciesAsync(client);
        await CreateAsync(client, "/api/master-data/products", Product("CAT-REF", "Referenced category product", categoryId, uomId, taxCodeId));

        var delete = await client.DeleteAsync($"/api/master-data/categories/{categoryId}");
        Assert.Equal(HttpStatusCode.Conflict, delete.StatusCode);
    }

    [Fact]
    public async Task UnitOfMeasure_CodeAndNameUniqueness_AreEnforced()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        await CreateAsync(client, "/api/master-data/units-of-measure", new { code = "EA", name = "Each" });

        AssertControlledConflict(await client.PostAsJsonAsync("/api/master-data/units-of-measure", new { code = "EA", name = "Alternative" }));
        AssertControlledConflict(await client.PostAsJsonAsync("/api/master-data/units-of-measure", new { code = "ALT", name = "Each" }));
    }

    [Fact]
    public async Task Product_UsesRequiredReferences_AndPersistsLifecycleState()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var categoryId = await CreateAsync(client, "/api/master-data/categories", new { name = "Products" });
        var (uomId, taxCodeId) = await CreateProductDependenciesAsync(client);
        var productId = await CreateAsync(client, "/api/master-data/products", Product("P-001", "Lifecycle product", categoryId, uomId, taxCodeId));

        AssertControlledConflict(await client.PostAsJsonAsync("/api/master-data/products", Product("P-001", "Duplicate code", categoryId, uomId, taxCodeId)));
        Assert.Equal(HttpStatusCode.NotFound, (await client.PostAsJsonAsync("/api/master-data/products", Product("P-MISSING", "Missing category", Guid.NewGuid(), uomId, taxCodeId))).StatusCode);

        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/master-data/products/{productId}/deactivate", null)).StatusCode);
        using var product = JsonDocument.Parse(await (await client.GetAsync($"/api/master-data/products/{productId}")).Content.ReadAsStringAsync());
        Assert.False(product.RootElement.GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task Product_NegativeQuantityAndPriceValues_AreRejected()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var categoryId = await CreateAsync(client, "/api/master-data/categories", new { name = "Validation" });
        var (uomId, taxCodeId) = await CreateProductDependenciesAsync(client);

        foreach (var invalidProduct in new[]
        {
            Product("P-NEG-MIN", "Negative minimum", categoryId, uomId, taxCodeId, minimumStockLevel: -1),
            Product("P-NEG-REORDER", "Negative reorder", categoryId, uomId, taxCodeId, reorderQuantity: -1),
            Product("P-NEG-SALES", "Negative sales", categoryId, uomId, taxCodeId, salesPrice: -1),
            Product("P-NEG-PURCHASE", "Negative purchase", categoryId, uomId, taxCodeId, purchasePrice: -1)
        })
        {
            Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/master-data/products", invalidProduct)).StatusCode);
        }
    }

    [Fact]
    public async Task Product_NamePersistsInResponses_AndCanBeUpdated()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var categoryId = await CreateAsync(client, "/api/master-data/categories", new { name = "Named products" });
        var (uomId, taxCodeId) = await CreateProductDependenciesAsync(client);
        var productId = await CreateAsync(client, "/api/master-data/products", Product("P-NAME", "Original product name", categoryId, uomId, taxCodeId));

        var update = await client.PutAsJsonAsync($"/api/master-data/products/{productId}", Product("P-NAME", "Updated product name", categoryId, uomId, taxCodeId));
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        using var product = JsonDocument.Parse(await (await client.GetAsync($"/api/master-data/products/{productId}")).Content.ReadAsStringAsync());
        Assert.Equal("Updated product name", product.RootElement.GetProperty("name").GetString());
    }

    [Fact]
    public async Task Customer_UniquenessFieldsAndCreditLimitValidation_AreEnforced()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var customerId = await CreateAsync(client, "/api/master-data/customers", Customer("C-001", 1250m));

        AssertControlledConflict(await client.PostAsJsonAsync("/api/master-data/customers", Customer("C-001", 1m)));
        using var customer = JsonDocument.Parse(await (await client.GetAsync($"/api/master-data/customers/{customerId}")).Content.ReadAsStringAsync());
        Assert.Equal("Billing address", customer.RootElement.GetProperty("billingAddress").GetString());
        Assert.Equal("Shipping address", customer.RootElement.GetProperty("shippingAddress").GetString());
        Assert.Equal("customer@example.test", customer.RootElement.GetProperty("email").GetString());
        Assert.Equal(1250m, customer.RootElement.GetProperty("creditLimit").GetDecimal());
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/master-data/customers", Customer("C-NEG", -1m))).StatusCode);
    }

    [Fact]
    public async Task Supplier_UniquenessAndSupportedFields_ArePersisted()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var supplierId = await CreateAsync(client, "/api/master-data/suppliers", Supplier("S-001"));

        AssertControlledConflict(await client.PostAsJsonAsync("/api/master-data/suppliers", Supplier("S-001")));
        using var supplier = JsonDocument.Parse(await (await client.GetAsync($"/api/master-data/suppliers/{supplierId}")).Content.ReadAsStringAsync());
        Assert.Equal("Supplier address", supplier.RootElement.GetProperty("address").GetString());
        Assert.Equal("Supplier contact", supplier.RootElement.GetProperty("contactInformation").GetString());
        Assert.Equal("Net 30", supplier.RootElement.GetProperty("paymentTerms").GetString());
        Assert.Equal("TRN-S", supplier.RootElement.GetProperty("trn").GetString());
    }

    [Fact]
    public async Task Warehouse_UniquenessAndLifecycleState_ArePersisted()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var warehouseId = await CreateAsync(client, "/api/master-data/warehouses", new { warehouseCode = "WH-001", name = "Main warehouse" });

        AssertControlledConflict(await client.PostAsJsonAsync("/api/master-data/warehouses", new { warehouseCode = "WH-001", name = "Duplicate warehouse" }));
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsync($"/api/master-data/warehouses/{warehouseId}/deactivate", null)).StatusCode);
        using var warehouse = JsonDocument.Parse(await (await client.GetAsync($"/api/master-data/warehouses/{warehouseId}")).Content.ReadAsStringAsync());
        Assert.False(warehouse.RootElement.GetProperty("isActive").GetBoolean());
    }

    [Fact]
    public async Task MissingMasterDataRead_ReturnsControlledNotFoundProblemDetails()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();

        var response = await client.GetAsync($"/api/master-data/categories/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("application/problem+json", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task CreateProduct_WithWhitespaceName_ReturnsValidationProblemDetails()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/master-data/products", Product("P1", " ", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("application/problem+json", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task CreateTaxCode_WithRateAbove100_ReturnsValidationProblemDetails()
    {
        await factory.ResetDatabaseAsync();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("/api/master-data/tax-codes", new { code = "HIGH", name = "High", rate = 101, taxType = "VAT", isActive = true });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task<Guid> CreateAsync(HttpClient client, string route, object request)
    {
        var response = await client.PostAsJsonAsync(route, request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<(Guid UomId, Guid TaxCodeId)> CreateProductDependenciesAsync(HttpClient client)
    {
        var uomId = await CreateAsync(client, "/api/master-data/units-of-measure", new { code = $"U{Guid.NewGuid():N}"[..12], name = $"Uom {Guid.NewGuid():N}" });
        var taxCodeId = await CreateAsync(client, "/api/master-data/tax-codes", new { code = $"T{Guid.NewGuid():N}"[..12], name = $"Tax {Guid.NewGuid():N}", rate = 5m, taxType = "VAT", isActive = true });
        return (uomId, taxCodeId);
    }

    private static object Product(string productCode, string name, Guid categoryId, Guid uomId, Guid taxCodeId, decimal minimumStockLevel = 0, decimal reorderQuantity = 0, decimal salesPrice = 10, decimal purchasePrice = 5) =>
        new { productCode, name, categoryId, stockUomId = uomId, taxCodeId, minimumStockLevel, reorderQuantity, salesPrice, purchasePrice };

    private static object Customer(string customerCode, decimal creditLimit) =>
        new { customerCode, name = "Customer name", billingAddress = "Billing address", shippingAddress = "Shipping address", phone = "0500000000", email = "customer@example.test", trn = "TRN-C", creditLimit };

    private static object Supplier(string supplierCode) =>
        new { supplierCode, name = "Supplier name", address = "Supplier address", contactInformation = "Supplier contact", paymentTerms = "Net 30", trn = "TRN-S" };

    private static void AssertControlledConflict(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Contains("application/problem+json", response.Content.Headers.ContentType?.ToString());
        var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        Assert.DoesNotContain("SqlException", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Microsoft.EntityFrameworkCore", body, StringComparison.OrdinalIgnoreCase);
    }
}
