namespace JTradeERP.Domain.Purchasing;

public enum PurchaseOrderStatus
{
    Draft = 1,
    Open = 2,
    PartiallyReceived = 3,
    FullyReceived = 4,
    Cancelled = 5
}

public enum GoodsReceiptNoteStatus
{
    Draft = 1,
    Posted = 2,
    Cancelled = 3
}
