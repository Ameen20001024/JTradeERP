namespace JTradeERP.Domain.MasterData;

public sealed class Category
{
    private Category() { }

    public Category(string name) 
    {
        Id = Guid.NewGuid();
        Rename(name);
        IsActive = true; 
    }
    
    public Guid Id { get; private set; }
    
    public string Name { get; private set; } = null!;
    
    public bool IsActive { get; private set; }
    
    public void Rename(string name) 
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        
        Name = name.Trim();
    }
    
    public void Activate() => IsActive = true;
    
    public void Deactivate() => IsActive = false;
}
