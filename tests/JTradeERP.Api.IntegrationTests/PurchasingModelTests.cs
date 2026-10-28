using JTradeERP.Domain.Purchasing;
using JTradeERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JTradeERP.Api.IntegrationTests;

public sealed class PurchasingModelTests
{
    [Fact]
    public void PurchasingModel_UsesRequiredPrecisionConcurrencyAndRestrictedHistoryReferences()
    {
        using var db = new JTradeERPDbContext(new DbContextOptionsBuilder<JTradeERPDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=JTradeERP_ModelTest;Trusted_Connection=True;TrustServerCertificate=True")
            .Options);
        var model = db.GetService<IDesignTimeModel>().Model;
        var order = model.FindEntityType(typeof(PurchaseOrder))!;
        var orderLine = model.FindEntityType(typeof(PurchaseOrderLine))!;
        var note = model.FindEntityType(typeof(GoodsReceiptNote))!;
        var noteLine = model.FindEntityType(typeof(GoodsReceiptNoteLine))!;

        Assert.True(order.FindProperty(nameof(PurchaseOrder.RowVersion))!.IsConcurrencyToken);
        Assert.True(note.FindProperty(nameof(GoodsReceiptNote.RowVersion))!.IsConcurrencyToken);
        Assert.Equal((18, 2), (order.FindProperty(nameof(PurchaseOrder.GrandTotal))!.GetPrecision(),
            order.FindProperty(nameof(PurchaseOrder.GrandTotal))!.GetScale()));
        Assert.Equal((18, 4), (orderLine.FindProperty(nameof(PurchaseOrderLine.ReceivedQuantity))!.GetPrecision(),
            orderLine.FindProperty(nameof(PurchaseOrderLine.ReceivedQuantity))!.GetScale()));
        Assert.Equal((9, 4), (noteLine.FindProperty(nameof(GoodsReceiptNoteLine.TaxRate))!.GetPrecision(),
            noteLine.FindProperty(nameof(GoodsReceiptNoteLine.TaxRate))!.GetScale()));
        Assert.All(order.GetForeignKeys(), fk => Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior));
        Assert.All(note.GetForeignKeys(), fk => Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior));
        Assert.All(orderLine.GetForeignKeys(), fk => Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior));
        Assert.All(noteLine.GetForeignKeys(), fk => Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior));

        var configuration = model.FindEntityType(typeof(PurchasingConfiguration))!;
        Assert.Contains(configuration.GetSeedData(), seed =>
            Equals(seed[nameof(PurchasingConfiguration.Id)], PurchasingConfiguration.SingletonId)
            && Equals(seed[nameof(PurchasingConfiguration.AllowOverReceipt)], false));
        var entityNames = model.GetEntityTypes().Select(x => x.ClrType.Name).ToHashSet(StringComparer.Ordinal);
        Assert.Contains("Invoice", entityNames);
        Assert.Contains("SalesOrder", entityNames);
        Assert.Contains("Payment", entityNames);
        Assert.Contains("CreditNote", entityNames);
    }
}
