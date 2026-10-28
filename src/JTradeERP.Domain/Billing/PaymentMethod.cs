namespace JTradeERP.Domain.Billing;

public sealed class PaymentMethod
{
    private PaymentMethod() { }

    public PaymentMethod(string code, string name)

    {
        Id = Guid.NewGuid(); Update(code, name); IsActive = true;
    }

    public Guid Id { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public void Update(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Payment method code and name are required.");

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
