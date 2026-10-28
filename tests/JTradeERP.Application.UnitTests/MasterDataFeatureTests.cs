using JTradeERP.Application.Common.Exceptions;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Application.Features.MasterData.Categories.DeleteCategory;
using JTradeERP.Application.Features.MasterData.Customers.CreateCustomer;
using JTradeERP.Application.Features.MasterData.Customers.DeleteCustomer;
using JTradeERP.Application.Features.MasterData.Products.CreateProduct;
using JTradeERP.Application.Features.MasterData.Products.UpdateProduct;
using JTradeERP.Application.Features.MasterData.Suppliers.DeleteSupplier;
using JTradeERP.Application.Features.MasterData.TaxCodes.ActivateTaxCode;
using JTradeERP.Application.Features.MasterData.UnitsOfMeasure.DeleteUnitOfMeasure;
using JTradeERP.Application.Features.MasterData.Warehouses.DeactivateWarehouse;
using JTradeERP.Domain.MasterData;
using JTradeERP.Domain.Purchasing;
using JTradeERP.Domain.Billing;

namespace JTradeERP.Application.UnitTests;

public sealed class MasterDataFeatureTests
{
    [Fact]
    public async Task DeleteCategory_WhenReferencedByProduct_ThrowsConflict()
    {
        var db = new MasterDataTestDbContext(); var category = new Category("Food"); db.Add(category);
        db.Add(new Product("P1", "Product", category.Id, Guid.NewGuid(), Guid.NewGuid(), 0, 0, 0, 0));
        await Assert.ThrowsAsync<ConflictException>(() => new DeleteCategoryHandler(db).Handle(new(category.Id), default));
    }

    [Fact]
    public async Task DeleteUnitOfMeasure_WhenReferencedByProduct_ThrowsConflict()
    {
        var db = new MasterDataTestDbContext(); var uom = new UnitOfMeasure("EA", "Each"); db.Add(uom);
        db.Add(new Product("P1", "Product", Guid.NewGuid(), uom.Id, Guid.NewGuid(), 0, 0, 0, 0));
        await Assert.ThrowsAsync<ConflictException>(() => new DeleteUnitOfMeasureHandler(db).Handle(new(uom.Id), default));
    }

    [Fact]
    public async Task ActivateTaxCode_ReturnsExplicitResponse()
    {
        var db = new MasterDataTestDbContext(); var tax = new TaxCode("ZERO", "Zero", 0, "VAT", false); db.Add(tax);
        var result = await new ActivateTaxCodeHandler(db).Handle(new(tax.Id), default);
        Assert.True(result.IsActive); Assert.Equal(tax.Id, result.Id); Assert.Equal("ZERO", result.Code);
    }

    [Fact]
    public void CreateProductValidator_RejectsWhitespaceName()
    {
        var result = new CreateProductValidator().Validate(new CreateProductCommand("P1", "  ", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, 0, 0, 0));
        Assert.False(result.IsValid); Assert.Contains(result.Errors, x => x.PropertyName == "Name");
    }

    [Fact]
    public async Task CreateProduct_WithAllReferences_ReturnsNameInResponse()
    {
        var db = new MasterDataTestDbContext(); var category = new Category("Food"); var uom = new UnitOfMeasure("EA", "Each"); var tax = new TaxCode("VAT5", "VAT", 5, "VAT", true);
        db.Add(category); db.Add(uom); db.Add(tax);
        var result = await new CreateProductHandler(db).Handle(new("P1", "Orange", category.Id, uom.Id, tax.Id, 1, 2, 3, 4), default);
        Assert.Equal("Orange", result.Name); Assert.Single(db.Products);
    }

    [Fact]
    public async Task UpdateProduct_RejectsMissingCategory()
    {
        var db = new MasterDataTestDbContext(); var product = new Product("P1", "Orange", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, 0, 0, 0); db.Add(product);
        await Assert.ThrowsAsync<NotFoundException>(() => new UpdateProductHandler(db).Handle(new(product.Id, "P1", "Orange", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 0, 0, 0, 0), default));
    }

    [Fact]
    public async Task UpdateProduct_UpdatesRequiredName()
    {
        var db = new MasterDataTestDbContext(); var category = new Category("Food"); var uom = new UnitOfMeasure("EA", "Each"); var tax = new TaxCode("VAT5", "VAT", 5, "VAT", true); db.Add(category); db.Add(uom); db.Add(tax);
        var product = new Product("P1", "Orange", category.Id, uom.Id, tax.Id, 0, 0, 1, 1); db.Add(product);
        var result = await new UpdateProductHandler(db).Handle(new(product.Id, "P1", "Mandarin", category.Id, uom.Id, tax.Id, 0, 0, 1, 1), default);
        Assert.Equal("Mandarin", result.Name); Assert.Equal("Mandarin", product.Name);
    }

    [Fact]
    public async Task CreateCustomer_AllowsNullCreditLimit()
    {
        var db = new MasterDataTestDbContext();
        var result = await new CreateCustomerHandler(db).Handle(new("C1", "Customer", null, null, null, null, null, null), default);
        Assert.Null(result.CreditLimit);
    }

    [Fact]
    public async Task DeleteCustomer_RemovesCustomer()
    {
        var db = new MasterDataTestDbContext(); var customer = new Customer("C1", "Customer", null, null, null, null, null, null); db.Add(customer);
        await new DeleteCustomerHandler(db).Handle(new(customer.Id), default);
        Assert.Empty(db.Customers);
    }

    [Fact]
    public async Task DeleteSupplier_RemovesSupplier()
    {
        var db = new MasterDataTestDbContext(); var supplier = new Supplier("S1", "Supplier", null, null, null, null); db.Add(supplier);
        await new DeleteSupplierHandler(db).Handle(new(supplier.Id), default);
        Assert.Empty(db.Suppliers);
    }

    [Fact]
    public async Task DeactivateWarehouse_ReturnsInactiveResponse()
    {
        var db = new MasterDataTestDbContext(); var warehouse = new Warehouse("MAIN", "Main"); db.Add(warehouse);
        var result = await new DeactivateWarehouseHandler(db).Handle(new(warehouse.Id), default);
        Assert.False(result.IsActive); Assert.Equal("MAIN", result.WarehouseCode);
    }
}

internal sealed class MasterDataTestDbContext : IApplicationDbContext
{
    private readonly List<Category> categories = []; private readonly List<UnitOfMeasure> units = []; private readonly List<TaxCode> taxes = [];
    private readonly List<Product> products = []; private readonly List<Customer> customers = []; private readonly List<Supplier> suppliers = []; private readonly List<Warehouse> warehouses = [];
    private readonly List<JTradeERP.Domain.Security.User> users = []; private readonly List<JTradeERP.Domain.Security.Role> roles = []; private readonly List<JTradeERP.Domain.Security.Permission> permissions = []; private readonly List<JTradeERP.Domain.Security.UserRole> userRoles = []; private readonly List<JTradeERP.Domain.Security.RolePermission> rolePermissions = []; private readonly List<JTradeERP.Domain.Security.RefreshToken> refreshTokens = []; private readonly List<JTradeERP.Domain.Security.PasswordResetOtp> passwordResetOtps = []; private readonly List<JTradeERP.Domain.Security.SecurityAuditEntry> securityAuditEntries = [];
    private readonly List<JTradeERP.Domain.Inventory.StockBalance> stockBalances = []; private readonly List<JTradeERP.Domain.Inventory.StockLedgerEntry> stockLedgerEntries = []; private readonly List<JTradeERP.Domain.Inventory.OpeningBalance> openingBalances = []; private readonly List<JTradeERP.Domain.Inventory.StockAdjustment> stockAdjustments = []; private readonly List<JTradeERP.Domain.Inventory.StockTransfer> stockTransfers = []; private readonly List<JTradeERP.Domain.Inventory.StockTransferLine> stockTransferLines = [];
    private readonly List<PurchaseOrder> purchaseOrders = []; private readonly List<PurchaseOrderLine> purchaseOrderLines = []; private readonly List<GoodsReceiptNote> goodsReceiptNotes = []; private readonly List<GoodsReceiptNoteLine> goodsReceiptNoteLines = []; private readonly List<PurchasingConfiguration> purchasingConfigurations = [];
    private readonly List<Quotation> quotations = []; private readonly List<QuotationLine> quotationLines = []; private readonly List<SalesOrder> salesOrders = []; private readonly List<SalesOrderLine> salesOrderLines = []; private readonly List<Invoice> invoices = []; private readonly List<InvoiceLine> invoiceLines = []; private readonly List<CreditNote> creditNotes = []; private readonly List<CreditNoteLine> creditNoteLines = []; private readonly List<Payment> payments = []; private readonly List<PaymentAllocation> paymentAllocations = []; private readonly List<PaymentMethod> paymentMethods = []; private readonly List<NumberSeries> numberSeries = []; private readonly List<BillingConfiguration> billingConfigurations = [];
    public IQueryable<Category> Categories => categories.AsQueryable(); public IQueryable<UnitOfMeasure> UnitsOfMeasure => units.AsQueryable(); public IQueryable<TaxCode> TaxCodes => taxes.AsQueryable(); public IQueryable<Product> Products => products.AsQueryable(); public IQueryable<Customer> Customers => customers.AsQueryable(); public IQueryable<Supplier> Suppliers => suppliers.AsQueryable(); public IQueryable<Warehouse> Warehouses => warehouses.AsQueryable();
    public IQueryable<JTradeERP.Domain.Security.User> Users => users.AsQueryable(); public IQueryable<JTradeERP.Domain.Security.Role> Roles => roles.AsQueryable(); public IQueryable<JTradeERP.Domain.Security.Permission> Permissions => permissions.AsQueryable(); public IQueryable<JTradeERP.Domain.Security.UserRole> UserRoles => userRoles.AsQueryable(); public IQueryable<JTradeERP.Domain.Security.RolePermission> RolePermissions => rolePermissions.AsQueryable(); public IQueryable<JTradeERP.Domain.Security.RefreshToken> RefreshTokens => refreshTokens.AsQueryable(); public IQueryable<JTradeERP.Domain.Security.PasswordResetOtp> PasswordResetOtps => passwordResetOtps.AsQueryable(); public IQueryable<JTradeERP.Domain.Security.SecurityAuditEntry> SecurityAuditEntries => securityAuditEntries.AsQueryable();
    public IQueryable<JTradeERP.Domain.Inventory.StockBalance> StockBalances => stockBalances.AsQueryable(); public IQueryable<JTradeERP.Domain.Inventory.StockLedgerEntry> StockLedgerEntries => stockLedgerEntries.AsQueryable(); public IQueryable<JTradeERP.Domain.Inventory.OpeningBalance> OpeningBalances => openingBalances.AsQueryable(); public IQueryable<JTradeERP.Domain.Inventory.StockAdjustment> StockAdjustments => stockAdjustments.AsQueryable(); public IQueryable<JTradeERP.Domain.Inventory.StockTransfer> StockTransfers => stockTransfers.AsQueryable(); public IQueryable<JTradeERP.Domain.Inventory.StockTransferLine> StockTransferLines => stockTransferLines.AsQueryable();
    public IQueryable<PurchaseOrder> PurchaseOrders => purchaseOrders.AsQueryable(); public IQueryable<PurchaseOrder> PurchaseOrdersWithLines => purchaseOrders.AsQueryable(); public IQueryable<PurchaseOrderLine> PurchaseOrderLines => purchaseOrderLines.AsQueryable(); public IQueryable<GoodsReceiptNote> GoodsReceiptNotes => goodsReceiptNotes.AsQueryable(); public IQueryable<GoodsReceiptNote> GoodsReceiptNotesWithLines => goodsReceiptNotes.AsQueryable(); public IQueryable<GoodsReceiptNoteLine> GoodsReceiptNoteLines => goodsReceiptNoteLines.AsQueryable(); public IQueryable<PurchasingConfiguration> PurchasingConfigurations => purchasingConfigurations.AsQueryable();
    public IQueryable<Quotation> Quotations => quotations.AsQueryable(); public IQueryable<Quotation> QuotationsWithLines => quotations.AsQueryable(); public IQueryable<QuotationLine> QuotationLines => quotationLines.AsQueryable(); public IQueryable<SalesOrder> SalesOrders => salesOrders.AsQueryable(); public IQueryable<SalesOrder> SalesOrdersWithLines => salesOrders.AsQueryable(); public IQueryable<SalesOrderLine> SalesOrderLines => salesOrderLines.AsQueryable(); public IQueryable<Invoice> Invoices => invoices.AsQueryable(); public IQueryable<Invoice> InvoicesWithLines => invoices.AsQueryable(); public IQueryable<InvoiceLine> InvoiceLines => invoiceLines.AsQueryable(); public IQueryable<CreditNote> CreditNotes => creditNotes.AsQueryable(); public IQueryable<CreditNote> CreditNotesWithLines => creditNotes.AsQueryable(); public IQueryable<CreditNoteLine> CreditNoteLines => creditNoteLines.AsQueryable(); public IQueryable<Payment> Payments => payments.AsQueryable(); public IQueryable<Payment> PaymentsWithAllocations => payments.AsQueryable(); public IQueryable<PaymentAllocation> PaymentAllocations => paymentAllocations.AsQueryable(); public IQueryable<PaymentMethod> PaymentMethods => paymentMethods.AsQueryable(); public IQueryable<NumberSeries> NumberSeries => numberSeries.AsQueryable(); public IQueryable<BillingConfiguration> BillingConfigurations => billingConfigurations.AsQueryable();
    public void Add<TEntity>(TEntity entity) where TEntity : class { switch (entity) { case Category x: categories.Add(x); break; case UnitOfMeasure x: units.Add(x); break; case TaxCode x: taxes.Add(x); break; case Product x: products.Add(x); break; case Customer x: customers.Add(x); break; case Supplier x: suppliers.Add(x); break; case Warehouse x: warehouses.Add(x); break; default: throw new ArgumentOutOfRangeException(nameof(entity)); } }
    public void Remove<TEntity>(TEntity entity) where TEntity : class { switch (entity) { case Category x: categories.Remove(x); break; case UnitOfMeasure x: units.Remove(x); break; case TaxCode x: taxes.Remove(x); break; case Product x: products.Remove(x); break; case Customer x: customers.Remove(x); break; case Supplier x: suppliers.Remove(x); break; case Warehouse x: warehouses.Remove(x); break; case JTradeERP.Domain.Security.UserRole x: userRoles.Remove(x); break; case JTradeERP.Domain.Security.RolePermission x: rolePermissions.Remove(x); break; case JTradeERP.Domain.Security.RefreshToken x: refreshTokens.Remove(x); break; case JTradeERP.Domain.Security.PasswordResetOtp x: passwordResetOtps.Remove(x); break; case JTradeERP.Domain.Security.SecurityAuditEntry x: securityAuditEntries.Remove(x); break; default: throw new ArgumentOutOfRangeException(nameof(entity)); } }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => Task.FromResult(1);
    public Task<IApplicationTransaction> BeginTransactionAsync(CancellationToken cancellationToken) => Task.FromResult<IApplicationTransaction>(new FakeApplicationTransaction());
}

internal sealed class FakeApplicationTransaction : IApplicationTransaction
{
    public Task CommitAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
