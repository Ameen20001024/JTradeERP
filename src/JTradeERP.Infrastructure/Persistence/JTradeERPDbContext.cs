using Microsoft.EntityFrameworkCore;
using JTradeERP.Application.Common.Persistence;
using JTradeERP.Domain.MasterData;
using JTradeERP.Domain.Security;
using JTradeERP.Domain.Inventory;
using JTradeERP.Domain.Purchasing;
using JTradeERP.Domain.Billing;
using Microsoft.EntityFrameworkCore.Storage;

namespace JTradeERP.Infrastructure.Persistence;

public sealed class JTradeERPDbContext(DbContextOptions<JTradeERPDbContext> options) : DbContext(options), IApplicationDbContext
{
    public override int SaveChanges()
    {
        EnsureStockLedgerIsAppendOnly();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnsureStockLedgerIsAppendOnly();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void EnsureStockLedgerIsAppendOnly()
    {
        ChangeTracker.DetectChanges();
        if (ChangeTracker.Entries<StockLedgerEntry>().Any(x =>
                x.State is EntityState.Modified or EntityState.Deleted))
            throw new InvalidOperationException("Stock ledger entries are append-only; corrections must be posted as reversals.");
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<UnitOfMeasure> UnitsOfMeasure => Set<UnitOfMeasure>();
    public DbSet<TaxCode> TaxCodes => Set<TaxCode>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetOtp> PasswordResetOtps => Set<PasswordResetOtp>();
    public DbSet<SecurityAuditEntry> SecurityAuditEntries => Set<SecurityAuditEntry>();
    public DbSet<StockBalance> StockBalances => Set<StockBalance>();
    public DbSet<StockLedgerEntry> StockLedgerEntries => Set<StockLedgerEntry>();
    public DbSet<OpeningBalance> OpeningBalances => Set<OpeningBalance>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
    public DbSet<StockTransferLine> StockTransferLines => Set<StockTransferLine>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();
    public DbSet<GoodsReceiptNote> GoodsReceiptNotes => Set<GoodsReceiptNote>();
    public DbSet<GoodsReceiptNoteLine> GoodsReceiptNoteLines => Set<GoodsReceiptNoteLine>();
    public DbSet<PurchasingConfiguration> PurchasingConfigurations => Set<PurchasingConfiguration>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<QuotationLine> QuotationLines => Set<QuotationLine>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderLine> SalesOrderLines => Set<SalesOrderLine>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLine> InvoiceLines => Set<InvoiceLine>();
    public DbSet<CreditNote> CreditNotes => Set<CreditNote>();
    public DbSet<CreditNoteLine> CreditNoteLines => Set<CreditNoteLine>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentAllocation> PaymentAllocations => Set<PaymentAllocation>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<NumberSeries> NumberSeries => Set<NumberSeries>();
    public DbSet<BillingConfiguration> BillingConfigurations => Set<BillingConfiguration>();

    IQueryable<Category> IApplicationDbContext.Categories => Categories;
    IQueryable<UnitOfMeasure> IApplicationDbContext.UnitsOfMeasure => UnitsOfMeasure;
    IQueryable<TaxCode> IApplicationDbContext.TaxCodes => TaxCodes;
    IQueryable<Product> IApplicationDbContext.Products => Products;
    IQueryable<Customer> IApplicationDbContext.Customers => Customers;
    IQueryable<Supplier> IApplicationDbContext.Suppliers => Suppliers;
    IQueryable<Warehouse> IApplicationDbContext.Warehouses => Warehouses;
    IQueryable<User> IApplicationDbContext.Users => Users;
    IQueryable<Role> IApplicationDbContext.Roles => Roles;
    IQueryable<Permission> IApplicationDbContext.Permissions => Permissions;
    IQueryable<UserRole> IApplicationDbContext.UserRoles => UserRoles;
    IQueryable<RolePermission> IApplicationDbContext.RolePermissions => RolePermissions;
    IQueryable<RefreshToken> IApplicationDbContext.RefreshTokens => RefreshTokens;
    IQueryable<PasswordResetOtp> IApplicationDbContext.PasswordResetOtps => PasswordResetOtps;
    IQueryable<SecurityAuditEntry> IApplicationDbContext.SecurityAuditEntries => SecurityAuditEntries;
    IQueryable<StockBalance> IApplicationDbContext.StockBalances => StockBalances;
    IQueryable<StockLedgerEntry> IApplicationDbContext.StockLedgerEntries => StockLedgerEntries;
    IQueryable<OpeningBalance> IApplicationDbContext.OpeningBalances => OpeningBalances;
    IQueryable<StockAdjustment> IApplicationDbContext.StockAdjustments => StockAdjustments;
    IQueryable<StockTransfer> IApplicationDbContext.StockTransfers => StockTransfers;
    IQueryable<StockTransferLine> IApplicationDbContext.StockTransferLines => StockTransferLines;
    IQueryable<PurchaseOrder> IApplicationDbContext.PurchaseOrders => PurchaseOrders;
    IQueryable<PurchaseOrder> IApplicationDbContext.PurchaseOrdersWithLines => PurchaseOrders.Include(x => x.Lines);
    IQueryable<PurchaseOrderLine> IApplicationDbContext.PurchaseOrderLines => PurchaseOrderLines;
    IQueryable<GoodsReceiptNote> IApplicationDbContext.GoodsReceiptNotes => GoodsReceiptNotes;
    IQueryable<GoodsReceiptNote> IApplicationDbContext.GoodsReceiptNotesWithLines => GoodsReceiptNotes.Include(x => x.Lines);
    IQueryable<GoodsReceiptNoteLine> IApplicationDbContext.GoodsReceiptNoteLines => GoodsReceiptNoteLines;
    IQueryable<PurchasingConfiguration> IApplicationDbContext.PurchasingConfigurations => PurchasingConfigurations;
    IQueryable<Quotation> IApplicationDbContext.Quotations => Quotations;
    IQueryable<Quotation> IApplicationDbContext.QuotationsWithLines => Quotations.Include(x => x.Lines);
    IQueryable<QuotationLine> IApplicationDbContext.QuotationLines => QuotationLines;
    IQueryable<SalesOrder> IApplicationDbContext.SalesOrders => SalesOrders;
    IQueryable<SalesOrder> IApplicationDbContext.SalesOrdersWithLines => SalesOrders.Include(x => x.Lines);
    IQueryable<SalesOrderLine> IApplicationDbContext.SalesOrderLines => SalesOrderLines;
    IQueryable<Invoice> IApplicationDbContext.Invoices => Invoices;
    IQueryable<Invoice> IApplicationDbContext.InvoicesWithLines => Invoices.Include(x => x.Lines);
    IQueryable<InvoiceLine> IApplicationDbContext.InvoiceLines => InvoiceLines;
    IQueryable<CreditNote> IApplicationDbContext.CreditNotes => CreditNotes;
    IQueryable<CreditNote> IApplicationDbContext.CreditNotesWithLines => CreditNotes.Include(x => x.Lines);
    IQueryable<CreditNoteLine> IApplicationDbContext.CreditNoteLines => CreditNoteLines;
    IQueryable<Payment> IApplicationDbContext.Payments => Payments;
    IQueryable<Payment> IApplicationDbContext.PaymentsWithAllocations => Payments.Include(x => x.Allocations);
    IQueryable<PaymentAllocation> IApplicationDbContext.PaymentAllocations => PaymentAllocations;
    IQueryable<PaymentMethod> IApplicationDbContext.PaymentMethods => PaymentMethods;
    IQueryable<NumberSeries> IApplicationDbContext.NumberSeries => NumberSeries;
    IQueryable<BillingConfiguration> IApplicationDbContext.BillingConfigurations => BillingConfigurations;
    
    
    async Task<IApplicationTransaction> IApplicationDbContext.BeginTransactionAsync(CancellationToken cancellationToken) =>
        new EfApplicationTransaction(await Database.BeginTransactionAsync(cancellationToken));
    
    void IApplicationDbContext.Add<TEntity>(TEntity entity) => Add(entity);
    
    void IApplicationDbContext.Remove<TEntity>(TEntity entity) => Remove(entity);

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(b => { b.HasKey(x => x.Id); b.Property(x => x.Name).HasMaxLength(200).IsRequired(); b.HasIndex(x => x.Name).IsUnique(); });
        modelBuilder.Entity<UnitOfMeasure>(b => { b.HasKey(x => x.Id); b.Property(x => x.Code).HasMaxLength(50).IsRequired(); b.Property(x => x.Name).HasMaxLength(200).IsRequired(); b.HasIndex(x => x.Code).IsUnique(); b.HasIndex(x => x.Name).IsUnique(); });
        modelBuilder.Entity<TaxCode>(b => { b.HasKey(x => x.Id); b.Property(x => x.Code).HasMaxLength(50).IsRequired(); b.Property(x => x.Name).HasMaxLength(200).IsRequired(); b.Property(x => x.TaxType).HasMaxLength(50).IsRequired(); b.Property(x => x.Rate).HasPrecision(5, 2); b.HasIndex(x => x.Code).IsUnique(); b.HasData(new { Id = new Guid("7f1f3f3b-9ca7-4d20-9af6-2bb2d1a7a501"), Code = "VAT5", Name = "UAE VAT Standard", Rate = 5m, TaxType = "VAT", IsActive = true }); });
        modelBuilder.Entity<Product>(b => { b.HasKey(x => x.Id); b.Property(x => x.ProductCode).HasMaxLength(50).IsRequired(); b.Property(x => x.Name).HasMaxLength(200).IsRequired(); b.Property(x => x.MinimumStockLevel).HasPrecision(18, 4); b.Property(x => x.ReorderQuantity).HasPrecision(18, 4); b.Property(x => x.SalesPrice).HasPrecision(18, 2); b.Property(x => x.PurchasePrice).HasPrecision(18, 2); b.HasIndex(x => x.ProductCode).IsUnique(); b.HasOne<Category>().WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict); b.HasOne<UnitOfMeasure>().WithMany().HasForeignKey(x => x.StockUomId).OnDelete(DeleteBehavior.Restrict); b.HasOne<TaxCode>().WithMany().HasForeignKey(x => x.TaxCodeId).OnDelete(DeleteBehavior.Restrict); });
        modelBuilder.Entity<Customer>(b => { b.HasKey(x => x.Id); b.Property(x => x.CustomerCode).HasMaxLength(50).IsRequired(); b.Property(x => x.Name).HasMaxLength(200).IsRequired(); b.Property(x => x.CreditLimit).HasPrecision(18, 2); b.HasIndex(x => x.CustomerCode).IsUnique(); });
        modelBuilder.Entity<Supplier>(b => { b.HasKey(x => x.Id); b.Property(x => x.SupplierCode).HasMaxLength(50).IsRequired(); b.Property(x => x.Name).HasMaxLength(200).IsRequired(); b.HasIndex(x => x.SupplierCode).IsUnique(); });
        modelBuilder.Entity<Warehouse>(b => { b.HasKey(x => x.Id); b.Property(x => x.WarehouseCode).HasMaxLength(50).IsRequired(); b.Property(x => x.Name).HasMaxLength(200).IsRequired(); b.Property(x => x.AllowNegativeStock).HasDefaultValue(false); b.HasIndex(x => x.WarehouseCode).IsUnique(); });
        ConfigureSecurity(modelBuilder);
        ConfigurePurchasing(modelBuilder);
        ConfigureBilling(modelBuilder);
    }

    private sealed class EfApplicationTransaction(IDbContextTransaction transaction) : IApplicationTransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken) => transaction.CommitAsync(cancellationToken);
        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }

    private static void ConfigureSecurity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(b => { b.HasKey(x => x.Id); b.Property(x => x.UserName).HasMaxLength(100).IsRequired(); b.Property(x => x.Email).HasMaxLength(320).IsRequired(); b.Property(x => x.PasswordHash).HasMaxLength(1000).IsRequired(); b.HasIndex(x => x.UserName).IsUnique(); b.HasIndex(x => x.Email).IsUnique(); });
        modelBuilder.Entity<Role>(b => { b.HasKey(x => x.Id); b.Property(x => x.Name).HasMaxLength(100).IsRequired(); b.HasIndex(x => x.Name).IsUnique(); b.HasData(SecuritySeed.Roles); });
        modelBuilder.Entity<Permission>(b => { b.HasKey(x => x.Id); b.Property(x => x.Code).HasMaxLength(100).IsRequired(); b.HasIndex(x => x.Code).IsUnique(); b.HasData(SecuritySeed.Permissions); });
        modelBuilder.Entity<UserRole>(b => { b.HasKey(x => new { x.UserId, x.RoleId }); b.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict); b.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict); });
        modelBuilder.Entity<RolePermission>(b => { b.HasKey(x => new { x.RoleId, x.PermissionId }); b.Property(x => x.Capability).HasConversion<int>(); b.ToTable(t => t.HasCheckConstraint("CK_RolePermissions_Capability", "[Capability] IN (1, 2, 3, 4)")); b.HasOne<Role>().WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict); b.HasOne<Permission>().WithMany().HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Restrict); b.HasData(SecuritySeed.RolePermissions); });
        modelBuilder.Entity<RefreshToken>(b => { b.HasKey(x => x.Id); b.Property(x => x.TokenHash).HasMaxLength(128).IsRequired(); b.Property(x => x.RevocationReason).HasMaxLength(100); b.Property(x => x.Version).IsRowVersion(); b.HasIndex(x => x.TokenHash).IsUnique(); b.HasIndex(x => new { x.UserId, x.RevokedAt }); b.HasIndex(x => new { x.FamilyId, x.RevokedAt }); b.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict); });
        modelBuilder.Entity<PasswordResetOtp>(b => { b.HasKey(x => x.Id); b.Property(x => x.OtpHash).HasMaxLength(128).IsRequired(); b.HasIndex(x => new { x.UserId, x.ExpiresAt }); b.HasOne<User>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict); });
        modelBuilder.Entity<SecurityAuditEntry>(b => { b.HasKey(x => x.Id); b.Property(x => x.Action).HasMaxLength(100).IsRequired(); b.Property(x => x.Entity).HasMaxLength(100).IsRequired(); b.Property(x => x.EntityId).HasMaxLength(100).IsRequired(); b.Property(x => x.CorrelationId).HasMaxLength(100).IsRequired(); b.HasIndex(x => x.OccurredAt); });
        ConfigureInventory(modelBuilder);
    }

    private static void ConfigureInventory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockBalance>(b =>
        {
            b.HasKey(x => new { x.ProductId, x.WarehouseId });
            b.Property(x => x.QtyOnHand).HasPrecision(18, 4);
            b.Property(x => x.QtyReserved).HasPrecision(18, 4);
            b.Property(x => x.WeightedAverageCost).HasPrecision(18, 4);
            b.Property(x => x.RowVersion).IsRowVersion();
            b.Ignore(x => x.AvailableQuantity);
            b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_StockBalances_QtyReserved_NonNegative", "[QtyReserved] >= 0");
            });
        });

        modelBuilder.Entity<StockLedgerEntry>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.MovementType).HasConversion<int>();
            b.Property(x => x.Direction).HasConversion<int>();
            b.Property(x => x.Quantity).HasPrecision(18, 4);
            b.Property(x => x.UnitCost).HasPrecision(18, 4);
            b.Property(x => x.SourceDocumentType).HasMaxLength(64).IsRequired();
            b.Property(x => x.CorrelationId).HasMaxLength(100).IsRequired();
            b.HasIndex(x => new { x.SourceDocumentType, x.SourceDocumentId, x.SourceLineId, x.MovementType, x.Direction })
                .IsUnique().HasFilter("[SourceLineId] IS NOT NULL").HasDatabaseName("UX_StockLedger_SourceEffect_Line");
            b.HasIndex(x => new { x.SourceDocumentType, x.SourceDocumentId, x.MovementType, x.Direction })
                .IsUnique().HasFilter("[SourceLineId] IS NULL").HasDatabaseName("UX_StockLedger_SourceEffect_Document");
            b.HasIndex(x => x.ReversalOfLedgerEntryId).IsUnique().HasFilter("[ReversalOfLedgerEntryId] IS NOT NULL");
            b.HasIndex(x => new { x.ProductId, x.WarehouseId, x.PostedAt });
            b.HasIndex(x => new { x.SourceDocumentType, x.SourceDocumentId });
            b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.PostedByUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<StockLedgerEntry>().WithMany().HasForeignKey(x => x.ReversalOfLedgerEntryId).OnDelete(DeleteBehavior.Restrict);
            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_StockLedger_Quantity_Positive", "[Quantity] > 0");
                t.HasCheckConstraint("CK_StockLedger_UnitCost_NonNegative", "[UnitCost] >= 0");
                t.HasCheckConstraint("CK_StockLedger_MovementType", "[MovementType] IN (1, 2, 3, 4, 5, 6)");
                t.HasCheckConstraint("CK_StockLedger_Direction", "[Direction] IN (1, 2)");
            });
        });

        modelBuilder.Entity<OpeningBalance>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Quantity).HasPrecision(18, 4);
            b.Property(x => x.UnitCost).HasPrecision(18, 4);
            b.Property(x => x.Status).HasConversion<int>();
            b.HasIndex(x => x.SourceDocumentId).IsUnique();
            b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.PostedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockAdjustment>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.QuantityDelta).HasPrecision(18, 4);
            b.Property(x => x.UnitCost).HasPrecision(18, 4);
            b.Property(x => x.ReasonCode).HasMaxLength(100).IsRequired();
            b.Property(x => x.Status).HasConversion<int>();
            b.HasIndex(x => new { x.ProductId, x.WarehouseId, x.Status });
            b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.PostedByUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.ReversedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockTransfer>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Status).HasConversion<int>();
            b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.StockTransferId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.SourceWarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.DestinationWarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.PostedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockTransferLine>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Quantity).HasPrecision(18, 4);
            b.HasIndex(x => new { x.StockTransferId, x.ProductId });
            b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurePurchasing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseOrder>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Status).HasConversion<int>();
            b.Property(x => x.SubTotal).HasPrecision(18, 2);
            b.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            b.Property(x => x.TaxAmount).HasPrecision(18, 2);
            b.Property(x => x.GrandTotal).HasPrecision(18, 2);
            b.Property(x => x.RowVersion).IsRowVersion();
            b.HasIndex(x => new { x.Status, x.OrderDate });
            b.HasIndex(x => x.SupplierId);
            b.HasIndex(x => x.WarehouseId);
            b.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.UpdatedByUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
            b.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_PurchaseOrders_Status", "[Status] IN (1,2,3,4,5)");
                t.HasCheckConstraint("CK_PurchaseOrders_Totals", "[SubTotal] >= 0 AND [DiscountAmount] >= 0 AND [DiscountAmount] <= [SubTotal] AND [TaxAmount] >= 0 AND [GrandTotal] = [SubTotal] - [DiscountAmount] + [TaxAmount]");
            });
        });

        modelBuilder.Entity<PurchaseOrderLine>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.OrderedQuantity).HasPrecision(18, 4);
            b.Property(x => x.ReceivedQuantity).HasPrecision(18, 4);
            b.Property(x => x.UnitPurchaseCost).HasPrecision(18, 4);
            b.Property(x => x.TaxRate).HasPrecision(9, 4);
            b.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            b.Property(x => x.LineSubTotal).HasPrecision(18, 2);
            b.Property(x => x.TaxAmount).HasPrecision(18, 2);
            b.Property(x => x.LineTotal).HasPrecision(18, 2);
            b.HasIndex(x => x.PurchaseOrderId);
            b.HasIndex(x => x.ProductId);
            b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<TaxCode>().WithMany().HasForeignKey(x => x.TaxCodeId).OnDelete(DeleteBehavior.Restrict);
            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_PurchaseOrderLines_Quantities", "[OrderedQuantity] > 0 AND [ReceivedQuantity] >= 0");
                t.HasCheckConstraint("CK_PurchaseOrderLines_CostTaxDiscount", "[UnitPurchaseCost] >= 0 AND [TaxRate] >= 0 AND [TaxRate] <= 100 AND [DiscountAmount] >= 0 AND [DiscountAmount] <= [LineSubTotal]");
                t.HasCheckConstraint("CK_PurchaseOrderLines_LineTotal", "[LineTotal] = [LineSubTotal] - [DiscountAmount] + [TaxAmount]");
            });
        });

        modelBuilder.Entity<GoodsReceiptNote>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Status).HasConversion<int>();
            b.Property(x => x.RowVersion).IsRowVersion();
            b.HasIndex(x => new { x.Status, x.ReceiptDate });
            b.HasIndex(x => new { x.PurchaseOrderId, x.ReceiptDate });
            b.HasIndex(x => new { x.SupplierId, x.ReceiptDate });
            b.HasOne<PurchaseOrder>().WithMany().HasForeignKey(x => x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Supplier>().WithMany().HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.PostedByUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.CancelledByUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.GoodsReceiptNoteId).OnDelete(DeleteBehavior.Restrict);
            b.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.ToTable(t => t.HasCheckConstraint("CK_GoodsReceiptNotes_Status", "[Status] IN (1,2,3)"));
        });

        modelBuilder.Entity<GoodsReceiptNoteLine>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.ReceivedQuantity).HasPrecision(18, 4);
            b.Property(x => x.UnitCost).HasPrecision(18, 4);
            b.Property(x => x.TaxRate).HasPrecision(9, 4);
            b.Property(x => x.LineSubTotal).HasPrecision(18, 2);
            b.Property(x => x.TaxAmount).HasPrecision(18, 2);
            b.Property(x => x.LineTotal).HasPrecision(18, 2);
            b.HasIndex(x => x.GoodsReceiptNoteId);
            b.HasIndex(x => x.PurchaseOrderLineId);
            b.HasIndex(x => x.ProductId);
            b.HasOne<PurchaseOrderLine>().WithMany().HasForeignKey(x => x.PurchaseOrderLineId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<TaxCode>().WithMany().HasForeignKey(x => x.TaxCodeId).OnDelete(DeleteBehavior.Restrict);
            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_GoodsReceiptNoteLines_QuantityCost", "[ReceivedQuantity] > 0 AND [UnitCost] >= 0");
                t.HasCheckConstraint("CK_GoodsReceiptNoteLines_Tax", "[TaxRate] >= 0 AND [TaxRate] <= 100 AND [TaxAmount] >= 0");
                t.HasCheckConstraint("CK_GoodsReceiptNoteLines_LineTotal", "[LineTotal] = [LineSubTotal] + [TaxAmount]");
            });
        });

        modelBuilder.Entity<PurchasingConfiguration>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.RowVersion).IsRowVersion();
            b.HasData(new { Id = PurchasingConfiguration.SingletonId, AllowOverReceipt = false });
        });
    }

    private static void ConfigureBilling(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Quotation>(b =>
        {
            b.HasKey(x => x.Id); b.Property(x => x.Status).HasConversion<int>(); b.Property(x => x.RowVersion).IsRowVersion();
            b.Property(x => x.SubTotal).HasPrecision(18, 2); b.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            b.Property(x => x.TaxAmount).HasPrecision(18, 2); b.Property(x => x.GrandTotal).HasPrecision(18, 2);
            b.HasIndex(x => new { x.Status, x.QuotationDate }); b.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.QuotationId).OnDelete(DeleteBehavior.Restrict);
            b.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.ToTable(t => t.HasCheckConstraint("CK_Quotations_Status", "[Status] IN (1,2,3,4,5,6)"));
        });
        modelBuilder.Entity<QuotationLine>(b =>
        {
            b.HasKey(x => x.Id); b.Property(x => x.Quantity).HasPrecision(18, 4); b.Property(x => x.UnitPrice).HasPrecision(18, 4);
            b.Property(x => x.DiscountAmount).HasPrecision(18, 2); b.Property(x => x.TaxRate).HasPrecision(9, 4);
            b.Property(x => x.LineSubTotal).HasPrecision(18, 2); b.Property(x => x.TaxAmount).HasPrecision(18, 2); b.Property(x => x.LineTotal).HasPrecision(18, 2);
            b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<TaxCode>().WithMany().HasForeignKey(x => x.TaxCodeId).OnDelete(DeleteBehavior.Restrict);
            b.ToTable(t => { t.HasCheckConstraint("CK_QuotationLines_Values", "[Quantity] > 0 AND [UnitPrice] >= 0 AND [DiscountAmount] >= 0 AND [DiscountAmount] <= [LineSubTotal] AND [TaxRate] >= 0 AND [TaxRate] <= 100"); });
        });
        modelBuilder.Entity<SalesOrder>(b =>
        {
            b.HasKey(x => x.Id); b.Property(x => x.Status).HasConversion<int>(); b.Property(x => x.RowVersion).IsRowVersion();
            b.Property(x => x.DeliveryAddress).HasMaxLength(500); b.Property(x => x.SubTotal).HasPrecision(18, 2);
            b.Property(x => x.DiscountAmount).HasPrecision(18, 2); b.Property(x => x.TaxAmount).HasPrecision(18, 2); b.Property(x => x.GrandTotal).HasPrecision(18, 2);
            b.HasIndex(x => new { x.Status, x.OrderDate }); b.HasIndex(x => x.SourceQuotationId).IsUnique().HasFilter("[SourceQuotationId] IS NOT NULL");
            b.HasOne<Quotation>().WithMany().HasForeignKey(x => x.SourceQuotationId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.PostedByUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.CancelledByUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
            b.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.ToTable(t => t.HasCheckConstraint("CK_SalesOrders_Status", "[Status] IN (1,2,3,4,5)"));
        });
        modelBuilder.Entity<SalesOrderLine>(b =>
        {
            b.HasKey(x => x.Id); b.Property(x => x.OrderedQuantity).HasPrecision(18, 4); b.Property(x => x.ReservedQuantity).HasPrecision(18, 4);
            b.Property(x => x.FulfilledQuantity).HasPrecision(18, 4); b.Property(x => x.UnitPrice).HasPrecision(18, 4); b.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            b.Property(x => x.TaxRate).HasPrecision(9, 4); b.Property(x => x.LineSubTotal).HasPrecision(18, 2); b.Property(x => x.TaxAmount).HasPrecision(18, 2); b.Property(x => x.LineTotal).HasPrecision(18, 2);
            b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict); b.HasOne<TaxCode>().WithMany().HasForeignKey(x => x.TaxCodeId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<QuotationLine>().WithMany().HasForeignKey(x => x.SourceQuotationLineId).OnDelete(DeleteBehavior.Restrict);
            b.ToTable(t => t.HasCheckConstraint("CK_SalesOrderLines_Quantities", "[OrderedQuantity] > 0 AND [ReservedQuantity] >= 0 AND [FulfilledQuantity] >= 0 AND [ReservedQuantity] + [FulfilledQuantity] <= [OrderedQuantity]"));
        });
        modelBuilder.Entity<Invoice>(b =>
        {
            b.HasKey(x => x.Id); b.Property(x => x.Status).HasConversion<int>(); b.Property(x => x.RowVersion).IsRowVersion();
            b.Property(x => x.InvoiceNumber).HasMaxLength(50); b.Property(x => x.CompanyTrnSnapshot).HasMaxLength(50); b.Property(x => x.CustomerTrnSnapshot).HasMaxLength(50);
            foreach (var p in new[] { nameof(Invoice.SubTotal), nameof(Invoice.DiscountAmount), nameof(Invoice.TaxAmount), nameof(Invoice.GrandTotal), nameof(Invoice.PaidAmount), nameof(Invoice.OutstandingAmount) }) b.Property<decimal>(p).HasPrecision(18, 2);
            b.HasIndex(x => x.InvoiceNumber).IsUnique().HasFilter("[InvoiceNumber] IS NOT NULL"); b.HasIndex(x => new { x.Status, x.InvoiceDate });
            b.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict); b.HasOne<Warehouse>().WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<SalesOrder>().WithMany().HasForeignKey(x => x.SourceSalesOrderId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<User>().WithMany().HasForeignKey(x => x.PostedByUserId).OnDelete(DeleteBehavior.Restrict); b.HasOne<User>().WithMany().HasForeignKey(x => x.CancelledByUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict); b.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.ToTable(t => t.HasCheckConstraint("CK_Invoices_StatusAndAmounts", "[Status] IN (1,2,3,4,5) AND [GrandTotal] >= 0 AND [PaidAmount] >= 0 AND [PaidAmount] <= [GrandTotal] AND [OutstandingAmount] >= 0 AND (([Status] = 1 AND [PaidAmount] = 0 AND [OutstandingAmount] = 0) OR ([Status] IN (2,3,4) AND [OutstandingAmount] = [GrandTotal] - [PaidAmount]) OR ([Status] = 5 AND [PaidAmount] = 0 AND [OutstandingAmount] = 0))"));
        });
        modelBuilder.Entity<InvoiceLine>(b =>
        {
            b.HasKey(x => x.Id); b.Property(x => x.Quantity).HasPrecision(18, 4); b.Property(x => x.UnitPrice).HasPrecision(18, 4);
            b.Property(x => x.DiscountAmount).HasPrecision(18, 2); b.Property(x => x.TaxRateSnapshot).HasPrecision(9, 4); b.Property(x => x.LineSubTotal).HasPrecision(18, 2);
            b.Property(x => x.TaxAmountSnapshot).HasPrecision(18, 2); b.Property(x => x.LineTotal).HasPrecision(18, 2);
            b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict); b.HasOne<TaxCode>().WithMany().HasForeignKey(x => x.TaxCodeId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne<SalesOrderLine>().WithMany().HasForeignKey(x => x.SourceSalesOrderLineId).OnDelete(DeleteBehavior.Restrict);
            b.ToTable(t => t.HasCheckConstraint("CK_InvoiceLines_Values", "[Quantity] > 0 AND [UnitPrice] >= 0 AND [DiscountAmount] >= 0 AND [TaxRateSnapshot] >= 0 AND [TaxRateSnapshot] <= 100"));
        });
        modelBuilder.Entity<CreditNote>(b =>
        {
            b.HasKey(x => x.Id); b.Property(x => x.Status).HasConversion<int>(); b.Property(x => x.RowVersion).IsRowVersion();
            b.Property(x => x.SubTotal).HasPrecision(18, 2); b.Property(x => x.TaxAmount).HasPrecision(18, 2); b.Property(x => x.GrandTotal).HasPrecision(18, 2);
            b.HasIndex(x => new { x.InvoiceId, x.Status }); b.HasOne<Invoice>().WithMany().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict); b.HasOne<User>().WithMany().HasForeignKey(x => x.PostedByUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.CreditNoteId).OnDelete(DeleteBehavior.Restrict); b.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.ToTable(t => t.HasCheckConstraint("CK_CreditNotes_Status", "[Status] IN (1,2,3)"));
        });
        modelBuilder.Entity<CreditNoteLine>(b =>
        {
            b.HasKey(x => x.Id); b.Property(x => x.Quantity).HasPrecision(18, 4); b.Property(x => x.UnitPrice).HasPrecision(18, 4); b.Property(x => x.TaxRateSnapshot).HasPrecision(9, 4);
            b.Property(x => x.LineSubTotal).HasPrecision(18, 2); b.Property(x => x.TaxAmount).HasPrecision(18, 2); b.Property(x => x.LineTotal).HasPrecision(18, 2);
            b.HasOne<InvoiceLine>().WithMany().HasForeignKey(x => x.InvoiceLineId).OnDelete(DeleteBehavior.Restrict); b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            b.ToTable(t => t.HasCheckConstraint("CK_CreditNoteLines_Values", "[Quantity] > 0 AND [UnitPrice] >= 0 AND [TaxRateSnapshot] >= 0 AND [TaxRateSnapshot] <= 100"));
        });
        modelBuilder.Entity<PaymentMethod>(b => { b.HasKey(x => x.Id); b.Property(x => x.Code).HasMaxLength(30).IsRequired(); b.Property(x => x.Name).HasMaxLength(100).IsRequired(); b.HasIndex(x => x.Code).IsUnique(); b.HasIndex(x => x.Name).IsUnique(); b.HasData(new { Id = new Guid("44c247b3-3bf6-48b7-9be7-010000000001"), Code = "CASH", Name = "Cash", IsActive = true }, new { Id = new Guid("44c247b3-3bf6-48b7-9be7-010000000002"), Code = "BANK_TRANSFER", Name = "Bank Transfer", IsActive = true }, new { Id = new Guid("44c247b3-3bf6-48b7-9be7-010000000003"), Code = "CARD", Name = "Card", IsActive = true }, new { Id = new Guid("44c247b3-3bf6-48b7-9be7-010000000004"), Code = "CHEQUE", Name = "Cheque", IsActive = true }); });
        modelBuilder.Entity<Payment>(b =>
        {
            b.HasKey(x => x.Id); b.Property(x => x.Status).HasConversion<int>(); b.Property(x => x.RowVersion).IsRowVersion(); b.Property(x => x.Amount).HasPrecision(18, 2); b.Property(x => x.ReferenceNumber).HasMaxLength(100);
            b.HasIndex(x => new { x.Status, x.PaymentDate }); b.HasOne<PaymentMethod>().WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict); b.HasOne<User>().WithMany().HasForeignKey(x => x.ReversedByUserId).OnDelete(DeleteBehavior.Restrict);
            b.HasMany(x => x.Allocations).WithOne().HasForeignKey(x => x.PaymentId).OnDelete(DeleteBehavior.Restrict); b.Navigation(x => x.Allocations).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.ToTable(t => t.HasCheckConstraint("CK_Payments_AmountAndStatus", "[Amount] > 0 AND [Status] IN (1,2)"));
        });
        modelBuilder.Entity<PaymentAllocation>(b =>
        {
            b.HasKey(x => x.Id); b.Property(x => x.Amount).HasPrecision(18, 2); b.HasIndex(x => new { x.PaymentId, x.InvoiceId }).IsUnique();
            b.HasOne<Invoice>().WithMany().HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Restrict); b.ToTable(t => t.HasCheckConstraint("CK_PaymentAllocations_Amount", "[Amount] > 0"));
        });
        modelBuilder.Entity<NumberSeries>(b =>
        {
            b.HasKey(x => x.Id); b.Property(x => x.DocumentType).HasMaxLength(50).IsRequired(); b.Property(x => x.Prefix).HasMaxLength(30).IsRequired(); b.Property(x => x.RowVersion).IsRowVersion(); b.HasIndex(x => x.DocumentType).IsUnique();
            b.ToTable(t => t.HasCheckConstraint("CK_NumberSeries_Values", "[NextNumber] > 0 AND [Padding] BETWEEN 1 AND 18"));
            b.HasData(new { Id = new Guid("a19394d1-005d-4ef7-a0a3-dc68c164b744"), DocumentType = "Invoice", Prefix = "INV-", NextNumber = 1L, Padding = 6 });
        });
        modelBuilder.Entity<BillingConfiguration>(b => { b.HasKey(x => x.Id); b.Property(x => x.CompanyTrn).HasMaxLength(50); b.Property(x => x.RowVersion).IsRowVersion(); b.HasData(new { Id = BillingConfiguration.SingletonId, CompanyTrn = (string?)null }); });
    }
}
