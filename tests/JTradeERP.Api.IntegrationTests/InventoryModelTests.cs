using JTradeERP.Domain.Inventory;
using JTradeERP.Domain.MasterData;
using JTradeERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JTradeERP.Api.IntegrationTests;

public sealed class InventoryModelTests
{
    [Fact]
    public void StockBalance_UsesCompositeKeyRowVersionAndRestrictedReferences()
    {
        using var db = CreateContext();
        var model = db.GetService<IDesignTimeModel>().Model;
        var balance = model.FindEntityType(typeof(StockBalance))!;
        Assert.Equal(new[] { nameof(StockBalance.ProductId), nameof(StockBalance.WarehouseId) },
            balance.FindPrimaryKey()!.Properties.Select(x => x.Name));
        var version = balance.FindProperty(nameof(StockBalance.RowVersion))!;
        Assert.True(version.IsConcurrencyToken);
        Assert.Equal(ValueGenerated.OnAddOrUpdate, version.ValueGenerated);
        Assert.All(balance.GetForeignKeys(), fk => Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior));
    }

    [Fact]
    public void StockLedger_HasSeparateUniqueIndexesForDocumentAndLineIdempotency()
    {
        using var db = CreateContext();
        var ledger = db.Model.FindEntityType(typeof(StockLedgerEntry))!;
        Assert.Contains(ledger.GetIndexes(), x => x.IsUnique && x.GetFilter() == "[SourceLineId] IS NOT NULL");
        Assert.Contains(ledger.GetIndexes(), x => x.IsUnique && x.GetFilter() == "[SourceLineId] IS NULL");
        Assert.All(ledger.GetForeignKeys(), fk => Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior));
    }

    [Fact]
    public void InventoryModel_IncludesPurchasingAndBillingEntities()
    {
        using var db = CreateContext();
        var entityNames = db.Model.GetEntityTypes().Select(x => x.ClrType.Name).ToHashSet(StringComparer.Ordinal);
        Assert.Contains("SalesOrder", entityNames);
        Assert.Contains("Invoice", entityNames);
        Assert.Contains("PurchaseOrder", entityNames);
        Assert.Contains("PurchaseOrderLine", entityNames);
        Assert.Contains("GoodsReceiptNote", entityNames);
        Assert.Contains("GoodsReceiptNoteLine", entityNames);
        Assert.Contains("Payment", entityNames);
        Assert.Contains("CreditNote", entityNames);
        Assert.True(db.Model.FindEntityType(typeof(Product))!.GetForeignKeys().All(x => x.DeleteBehavior == DeleteBehavior.Restrict));
    }

    private static JTradeERPDbContext CreateContext() => new(new DbContextOptionsBuilder<JTradeERPDbContext>()
        .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=JTradeERP_ModelTest;Trusted_Connection=True;TrustServerCertificate=True")
        .Options);
}
