using JTradeERP.Domain.MasterData;
using JTradeERP.Domain.Security;
using JTradeERP.Domain.Inventory;
using JTradeERP.Domain.Purchasing;
using JTradeERP.Domain.Billing;

namespace JTradeERP.Application.Common.Persistence;

public interface IApplicationDbContext
{
    IQueryable<Category> Categories { get; }
    
    IQueryable<UnitOfMeasure> UnitsOfMeasure { get; }
    
    IQueryable<TaxCode> TaxCodes { get; }
    
    IQueryable<Product> Products { get; }
    
    IQueryable<Customer> Customers { get; }
    
    IQueryable<Supplier> Suppliers { get; }
    
    IQueryable<Warehouse> Warehouses { get; }
    
    IQueryable<User> Users { get; }
    
    IQueryable<Role> Roles { get; }
    
    IQueryable<Permission> Permissions { get; }
    
    IQueryable<UserRole> UserRoles { get; }
    
    IQueryable<RolePermission> RolePermissions { get; }
    
    IQueryable<RefreshToken> RefreshTokens { get; }
    
    IQueryable<PasswordResetOtp> PasswordResetOtps { get; }
    
    IQueryable<SecurityAuditEntry> SecurityAuditEntries { get; }
    
    IQueryable<StockBalance> StockBalances { get; }
    
    IQueryable<StockLedgerEntry> StockLedgerEntries { get; }
    
    IQueryable<OpeningBalance> OpeningBalances { get; }
    
    IQueryable<StockAdjustment> StockAdjustments { get; }
    
    IQueryable<StockTransfer> StockTransfers { get; }
    
    IQueryable<StockTransferLine> StockTransferLines { get; }
    
    IQueryable<PurchaseOrder> PurchaseOrders { get; }
    
    IQueryable<PurchaseOrder> PurchaseOrdersWithLines { get; }
    
    IQueryable<PurchaseOrderLine> PurchaseOrderLines { get; }
    
    IQueryable<GoodsReceiptNote> GoodsReceiptNotes { get; }
    
    IQueryable<GoodsReceiptNote> GoodsReceiptNotesWithLines { get; }
    
    IQueryable<GoodsReceiptNoteLine> GoodsReceiptNoteLines { get; }
    
    IQueryable<PurchasingConfiguration> PurchasingConfigurations { get; }
    
    IQueryable<Quotation> Quotations { get; }
    
    IQueryable<Quotation> QuotationsWithLines { get; }
    
    IQueryable<QuotationLine> QuotationLines { get; }
    
    IQueryable<SalesOrder> SalesOrders { get; }
    
    IQueryable<SalesOrder> SalesOrdersWithLines { get; }
    
    IQueryable<SalesOrderLine> SalesOrderLines { get; }
    
    IQueryable<Invoice> Invoices { get; }
    
    IQueryable<Invoice> InvoicesWithLines { get; }
    
    
    IQueryable<InvoiceLine> InvoiceLines { get; }
    
    IQueryable<CreditNote> CreditNotes { get; }
    
    IQueryable<CreditNote> CreditNotesWithLines { get; }
    
    IQueryable<CreditNoteLine> CreditNoteLines { get; }
    
    IQueryable<Payment> Payments { get; }
    
    IQueryable<Payment> PaymentsWithAllocations { get; }
    
    IQueryable<PaymentAllocation> PaymentAllocations { get; }
    
    IQueryable<PaymentMethod> PaymentMethods { get; }
    
    IQueryable<NumberSeries> NumberSeries { get; }
    
    IQueryable<BillingConfiguration> BillingConfigurations { get; }
    
    void Add<TEntity>(TEntity entity) where TEntity : class;
    
    void Remove<TEntity>(TEntity entity) where TEntity : class;
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
    Task<IApplicationTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
