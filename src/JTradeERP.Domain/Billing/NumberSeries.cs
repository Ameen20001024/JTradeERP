namespace JTradeERP.Domain.Billing;

public sealed class NumberSeries
{
    private NumberSeries() { }
    public NumberSeries(string documentType, string prefix, long nextNumber, int padding)
    {
        Id = Guid.NewGuid(); Update(documentType, prefix, nextNumber, padding);
    }
    public Guid Id { get; private set; }
    public string DocumentType { get; private set; } = null!;
    public string Prefix { get; private set; } = null!;
    public long NextNumber { get; private set; }
    public int Padding { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public void Update(string documentType, string prefix, long nextNumber, int padding)
    {
        if (string.IsNullOrWhiteSpace(documentType) || documentType.Trim().Length > 50 || (prefix?.Length ?? 0) > 30 || nextNumber <= 0 || padding is < 1 or > 18)
            throw new ArgumentException("Number series values are invalid.");
        DocumentType = documentType.Trim(); Prefix = prefix?.Trim() ?? string.Empty; NextNumber = nextNumber; Padding = padding;
    }
    public string Format(long value) => Prefix + value.ToString().PadLeft(Padding, '0');
}
