namespace JTradeERP.Domain.Inventory;

public sealed class StockBalance
{
    private StockBalance() { }

    public StockBalance(Guid productId, Guid warehouseId)
    {
        if (productId == Guid.Empty || warehouseId == Guid.Empty)
            throw new ArgumentException("Product and warehouse are required.");
        ProductId = productId;
        WarehouseId = warehouseId;
    }

    public Guid ProductId { get; private set; }
    
    public Guid WarehouseId { get; private set; }
    
    public decimal QtyOnHand { get; private set; }
    
    public decimal QtyReserved { get; private set; }
    
    public decimal AvailableQuantity => QtyOnHand - QtyReserved;
    
    public decimal WeightedAverageCost { get; private set; }
    
    public byte[] RowVersion { get; private set; } = [];

    public void Receive(decimal quantity, decimal unitCost)
    {
        if (quantity <= 0 || unitCost < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Receipt quantity must be positive and unit cost cannot be negative.");

        var resultingQuantity = QtyOnHand + quantity;
        WeightedAverageCost = QtyOnHand > 0
            ? ((QtyOnHand * WeightedAverageCost) + (quantity * unitCost)) / resultingQuantity
            : unitCost;
        QtyOnHand = resultingQuantity;
    }

    public void Issue(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Issue quantity must be positive.");
        QtyOnHand -= quantity;
    }

    public bool CanIssue(decimal quantity) => quantity > 0 && AvailableQuantity >= quantity;

    // Reservation operations are domain primitives for later Sales Order integration; Phase 4 exposes no reservation endpoint.
    public bool TryReserve(decimal quantity)
    {
        return TryReserve(quantity, allowNegativeStock: false);
    }

    public bool TryReserve(decimal quantity, bool allowNegativeStock)
    {
        if (quantity <= 0 || (!allowNegativeStock && AvailableQuantity < quantity))
            return false;
        QtyReserved += quantity;
        return true;
    }

    public bool TryReleaseReservation(decimal quantity)
    {
        if (quantity <= 0 || QtyReserved < quantity)
            return false;
        QtyReserved -= quantity;
        return true;
    }

    public bool TryConvertReservationToIssue(decimal quantity)
    {
        return TryConvertReservationToIssue(quantity, allowNegativeStock: false);
    }

    public bool TryConvertReservationToIssue(decimal quantity, bool allowNegativeStock)
    {
        if (quantity <= 0 || QtyReserved < quantity || (!allowNegativeStock && QtyOnHand < quantity))
            return false;
        QtyReserved -= quantity;
        QtyOnHand -= quantity;
        return true;
    }

    public void ReverseReceipt(decimal quantity, decimal originalUnitCost, bool allowNegativeStock)
    {
        if (quantity <= 0 || originalUnitCost < 0 || (!allowNegativeStock && AvailableQuantity < quantity))
            throw new InvalidOperationException("Receipt reversal would produce an invalid stock balance.");

        var remainingQuantity = QtyOnHand - quantity;
        if (remainingQuantity > 0 && QtyOnHand > 0)
        {
            var remainingValue = (QtyOnHand * WeightedAverageCost) - (quantity * originalUnitCost);
            WeightedAverageCost = Math.Max(0, remainingValue / remainingQuantity);
        }
        QtyOnHand = remainingQuantity;
    }
}
