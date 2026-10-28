namespace JTradeERP.Domain.MasterData;
public sealed class Supplier
{
    private Supplier() { }
    
    public Supplier(
        string supplierCode,
        string name,
        string? address, 
        string? contactInformation,
        string? paymentTerms, 
        string? trn) 
    { 
        Id = Guid.NewGuid(); 
        
        Update(supplierCode, 
            name, 
            address, 
            contactInformation, 
            paymentTerms, 
            trn); 
    }
    
    public Guid Id { get; private set; }
    
    public string SupplierCode { get; private set; } = null!; 
    
    public string Name { get; private set; } = null!;
    
    public string? Address { get; private set; } 
    
    public string? ContactInformation { get; private set; } 
    
    public string? PaymentTerms { get; private set; } 
    
    public string? TRN { get; private set; }
    
    public void Update(string code,
        string name, 
        string? address, 
        string? contactInformation, 
        string? paymentTerms, 
        string? trn) 
    
    { 
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("Invalid supplier."); 
        
        SupplierCode = code.Trim(); 
        Name = name.Trim(); 
        Address = address; 
        ContactInformation = contactInformation; 
        PaymentTerms = paymentTerms; 
        TRN = trn;
    }
}
