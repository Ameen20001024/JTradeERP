namespace JTradeERP.Domain.MasterData;
public sealed class TaxCode
{
    private TaxCode() { }

    public TaxCode(string code,
        string name, 
        decimal rate, 
        string taxType, 
        bool isActive) 
    { 
        Id = Guid.NewGuid(); 
        Update(code, name, rate, taxType);
        IsActive = isActive;
    }

    public Guid Id { get; private set; }
    
    public string Code { get; private set; } = null!;
    
    public string Name { get; private set; } = null!;
    
    public decimal Rate { get; private set; }
    
    public string TaxType { get; private set; } = null!;
    
    public bool IsActive { get; private set; }
    
    public void Update(string code,
        string name, 
        decimal rate, 
        string taxType) 
    { 
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name) 
            || string.IsNullOrWhiteSpace(taxType) || rate < 0 || rate > 100) 
            
            throw new ArgumentException("Invalid tax code."); 
        
        Code = code.Trim();
        
        Name = name.Trim();
        
        Rate = rate;
        
        TaxType = taxType.Trim();
    
    }
    
    public void Activate() => IsActive = true;
    
    public void Deactivate() => IsActive = false;
}
