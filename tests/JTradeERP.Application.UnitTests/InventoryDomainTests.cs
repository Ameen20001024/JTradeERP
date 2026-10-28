using JTradeERP.Domain.Inventory;

namespace JTradeERP.Application.UnitTests;

public sealed class InventoryDomainTests
{
    [Fact]
    public void StockBalance_ReceiptsUseWeightedAverage_AndIssuesPreserveCost()
    {
        var balance = new StockBalance(Guid.NewGuid(), Guid.NewGuid());
        balance.Receive(10, 4);
        balance.Receive(10, 6);
        Assert.Equal(20, balance.QtyOnHand);
        Assert.Equal(5, balance.WeightedAverageCost);
        balance.Issue(5);
        Assert.Equal(15, balance.QtyOnHand);
        Assert.Equal(5, balance.WeightedAverageCost);
    }

    [Fact]
    public void StockBalance_AvailableQuantitySubtractsReservations_AndReservationCanConvertToIssue()
    {
        var balance = new StockBalance(Guid.NewGuid(), Guid.NewGuid());
        balance.Receive(10, 3);
        Assert.True(balance.TryReserve(4));
        Assert.Equal(6, balance.AvailableQuantity);
        Assert.True(balance.TryConvertReservationToIssue(3));
        Assert.Equal(7, balance.QtyOnHand);
        Assert.Equal(1, balance.QtyReserved);
        Assert.Equal(6, balance.AvailableQuantity);
        Assert.False(balance.TryReserve(7));
    }

    [Fact]
    public void StockBalance_ReservationReleaseRejectsOverRelease()
    {
        var balance = new StockBalance(Guid.NewGuid(), Guid.NewGuid());
        balance.Receive(3, 1);
        Assert.True(balance.TryReserve(2));
        Assert.False(balance.TryReleaseReservation(3));
        Assert.True(balance.TryReleaseReservation(2));
        Assert.Equal(0, balance.QtyReserved);
    }

    [Fact]
    public void StockBalance_ReceiptReversalRestoresQuantityAndUsesOriginalCost()
    {
        var balance = new StockBalance(Guid.NewGuid(), Guid.NewGuid());
        balance.Receive(10, 8);
        balance.ReverseReceipt(4, 8, allowNegativeStock: false);
        Assert.Equal(6, balance.QtyOnHand);
        Assert.Equal(8, balance.WeightedAverageCost);
    }

    [Fact]
    public void StockBalance_RejectsReceiptReversalBelowAvailableStockWhenNegativeStockDisabled()
    {
        var balance = new StockBalance(Guid.NewGuid(), Guid.NewGuid());
        balance.Receive(5, 2);
        balance.TryReserve(4);
        Assert.Throws<InvalidOperationException>(() => balance.ReverseReceipt(2, 2, allowNegativeStock: false));
    }

    [Fact]
    public void StockAdjustment_OnlyDraftCanChange_AndPositivePostingRequiresCost()
    {
        var adjustment = new StockAdjustment(Guid.NewGuid(), Guid.NewGuid(), 2, "COUNT");
        Assert.Throws<ArgumentException>(() => adjustment.Post(null, DateTimeOffset.UtcNow, Guid.NewGuid()));
        adjustment.Post(0, DateTimeOffset.UtcNow, Guid.NewGuid());
        Assert.Equal(InventoryDocumentStatus.Posted, adjustment.Status);
        Assert.Throws<InvalidOperationException>(() => adjustment.Update(Guid.NewGuid(), Guid.NewGuid(), 1, "COUNT"));
    }

    [Fact]
    public void StockTransfer_RequiresDifferentWarehousesAndValidLines()
    {
        var warehouse = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() => new StockTransfer(warehouse, warehouse));
        var transfer = new StockTransfer(warehouse, Guid.NewGuid());
        Assert.Throws<ArgumentException>(() => transfer.Update(transfer.SourceWarehouseId,
            transfer.DestinationWarehouseId, Array.Empty<(Guid ProductId, decimal Quantity)>()));
    }

    [Fact]
    public void LedgerEntry_RequiresReversalLinkForReversalMovementOnly()
    {
        var product = Guid.NewGuid();
        var warehouse = Guid.NewGuid();
        var actor = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() => new StockLedgerEntry(product, warehouse, StockMovementType.Reversal,
            StockMovementDirection.In, 1, 1, "Reversal", Guid.NewGuid(), null, null,
            DateTimeOffset.UtcNow, actor, "correlation"));
        var originalId = Guid.NewGuid();
        var reversal = new StockLedgerEntry(product, warehouse, StockMovementType.Reversal,
            StockMovementDirection.In, 1, 1, "Reversal", Guid.NewGuid(), originalId, originalId,
            DateTimeOffset.UtcNow, actor, "correlation");
        Assert.Equal(originalId, reversal.ReversalOfLedgerEntryId);
    }
}
