namespace JTradeERP.Domain.Purchasing;

public sealed class PurchasingConfiguration
{
    public static readonly Guid SingletonId = new("c6bb501f-46f1-44fa-8e11-7a97188dcb41");
    private PurchasingConfiguration() { }

    public PurchasingConfiguration(bool allowOverReceipt = false)
    {
        Id = SingletonId;
        AllowOverReceipt = allowOverReceipt;
    }

    public Guid Id { get; private set; }
    public bool AllowOverReceipt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public void SetAllowOverReceipt(bool allowOverReceipt) => AllowOverReceipt = allowOverReceipt;
}
