namespace JTradeERP.Domain.Billing;

public sealed class BillingConfiguration
{
    public static readonly Guid SingletonId = new("ba9b928e-c683-4d9a-9927-293c9477e45a");
    private BillingConfiguration() { }
    public BillingConfiguration(string? companyTrn = null) { Id = SingletonId; SetCompanyTrn(companyTrn); }
    public Guid Id { get; private set; }
    public string? CompanyTrn { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public void SetCompanyTrn(string? trn)
    {
        if (trn?.Length > 50) throw new ArgumentException("Company TRN exceeds the maximum length.");
        CompanyTrn = string.IsNullOrWhiteSpace(trn) ? null : trn.Trim();
    }
}
