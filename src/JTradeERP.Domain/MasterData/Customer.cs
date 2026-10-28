namespace JTradeERP.Domain.MasterData;

public sealed class Customer
{
    private Customer() { }
    
    public Customer(
        string customerCode,
        string name, 
        string? billingAddress,
        string? shippingAddress, 
        string? phone,
        string? email, 
        string? trn, 
        decimal? creditLimit) 
    
    {
        Id = Guid.NewGuid(); 
        
        Update(
            customerCode, 
            name, 
            billingAddress, 
            shippingAddress, 
            phone, 
            email, 
            trn, 
            creditLimit);
    }
    
    public Guid Id { get; private set; }
    
    public string CustomerCode { get; private set; } = null!; 
    
    public string Name { get; private set; } = null!; 
    
    public string? BillingAddress { get; private set; }
    
    public string? ShippingAddress { get; private set; }
    
    public string? Phone { get; private set; }
    
    public string? Email { get; private set; }
    
    public string? TRN { get; private set; }
    
    public decimal? CreditLimit { get; private set; }
    
    public void Update(
        string code, 
        string name, 
        string? billingAddress,
        string? shippingAddress, 
        string? phone, 
        string? email, 
        string? trn, 
        decimal? creditLimit) 
    { 
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name) || creditLimit < 0)
            throw new ArgumentException("Invalid customer.");
        
        CustomerCode = code.Trim();
        Name = name.Trim(); 
        BillingAddress = billingAddress; 
        ShippingAddress = shippingAddress; 
        Phone = phone; 
        Email = email; 
        TRN = trn; 
        CreditLimit = creditLimit; 
    }

}
