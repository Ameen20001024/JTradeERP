namespace JTradeERP.Domain.MasterData;
public sealed class UnitOfMeasure
{
    private UnitOfMeasure() { }
    
    public UnitOfMeasure(string code, string name)
    {
        Id = Guid.NewGuid();
        
        Update(code, name); 
    }
    
    public Guid Id { get; private set; }
    
    public string Code { get; private set; } = null!;
    
    public string Name { get; private set; } = null!;
    
    public void Update(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("Code and name are required.");
        
        Code = code.Trim(); 
        
        Name = name.Trim();
    }
}
