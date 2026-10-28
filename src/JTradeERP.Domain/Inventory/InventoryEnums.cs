namespace JTradeERP.Domain.Inventory;

public enum StockMovementType
{
    OpeningBalance = 1,
    Adjustment = 2,
    Transfer = 3,
    Receipt = 4,
    Issue = 5,
    Reversal = 6
}

public enum StockMovementDirection
{
    In = 1,
    Out = 2
}

public enum InventoryDocumentStatus
{
    Draft = 1,
    Posted = 2,
    Reversed = 3
}
