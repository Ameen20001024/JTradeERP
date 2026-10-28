using JTradeERP.Domain.Purchasing;

namespace JTradeERP.Application.UnitTests;

public sealed class PurchasingDomainTests
{
    [Fact]
    public void PurchaseOrder_CalculatesSnapshotsAndReceiptProgress()
    {
        var userId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var taxId = Guid.NewGuid();
        var line = new PurchaseOrderLine(Guid.NewGuid(), 10m, 10m, taxId, 5m, 2m);
        var order = new PurchaseOrder(supplierId, warehouseId, new DateOnly(2026, 9, 28), null,
            userId, [line], DateTimeOffset.Parse("2026-09-28T00:00:00Z"));

        Assert.Equal(100m, order.SubTotal);
        Assert.Equal(2m, order.DiscountAmount);
        Assert.Equal(4.9m, order.TaxAmount);
        Assert.Equal(102.9m, order.GrandTotal);
        Assert.Equal(PurchaseOrderStatus.Draft, order.Status);

        order.ApplyReceipt(new Dictionary<Guid, decimal> { [line.Id] = 4m }, DateTimeOffset.UtcNow);
        Assert.Equal(4m, order.Lines.Single().ReceivedQuantity);
        Assert.Equal(PurchaseOrderStatus.PartiallyReceived, order.Status);
        order.ApplyReceipt(new Dictionary<Guid, decimal> { [line.Id] = 6m }, DateTimeOffset.UtcNow);
        Assert.Equal(PurchaseOrderStatus.FullyReceived, order.Status);
        order.ReverseReceipt(new Dictionary<Guid, decimal> { [line.Id] = 10m }, DateTimeOffset.UtcNow);
        Assert.Equal(0m, order.Lines.Single().ReceivedQuantity);
        Assert.Equal(PurchaseOrderStatus.Open, order.Status);
    }

    [Fact]
    public void PurchaseOrder_RejectsInvalidExpectedDateAndOverDiscount()
    {
        var taxId = Guid.NewGuid();
        Assert.Throws<ArgumentOutOfRangeException>(() => new PurchaseOrderLine(Guid.NewGuid(), 1, 5, taxId, 5, 6));
        Assert.Throws<ArgumentException>(() => new PurchaseOrder(Guid.NewGuid(), Guid.NewGuid(),
            new DateOnly(2026, 9, 28), new DateOnly(2026, 9, 27), Guid.NewGuid(),
            [new PurchaseOrderLine(Guid.NewGuid(), 1, 1, taxId, 5, 0)], DateTimeOffset.UtcNow));
    }

    [Fact]
    public void GoodsReceiptNote_HasOnlyDraftPostedCancelledLifecycle()
    {
        var note = new GoodsReceiptNote(null, Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 9, 28),
            [new GoodsReceiptNoteLine(Guid.NewGuid(), null, 2, 3, Guid.NewGuid(), 5)], DateTimeOffset.UtcNow);

        Assert.Equal(GoodsReceiptNoteStatus.Draft, note.Status);
        note.Post(DateTimeOffset.UtcNow, Guid.NewGuid());
        Assert.Equal(GoodsReceiptNoteStatus.Posted, note.Status);
        Assert.NotNull(note.PostedAt);
        note.Cancel(DateTimeOffset.UtcNow, Guid.NewGuid());
        Assert.Equal(GoodsReceiptNoteStatus.Cancelled, note.Status);
        Assert.NotNull(note.CancelledAt);
    }

    [Fact]
    public void PurchasingConfiguration_DefaultsToNoOverReceipt()
    {
        var configuration = new PurchasingConfiguration();
        Assert.False(configuration.AllowOverReceipt);
        configuration.SetAllowOverReceipt(true);
        Assert.True(configuration.AllowOverReceipt);
    }
}
