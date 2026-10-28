namespace JTradeERP.Domain.MasterData;
public sealed class Warehouse
{
    private Warehouse() { }
    
    public Warehouse(string warehouseCode, string name)
    {
        Id = Guid.NewGuid();
        
        Update(warehouseCode, name);
        
        IsActive = true; 
        
        AllowNegativeStock = false;
    }

    public Guid Id { get; private set; } 
    
    public string WarehouseCode { get; private set; } = null!;
    
    public string Name { get; private set; } = null!; 
    
    public bool IsActive { get; private set; }
    
    public bool AllowNegativeStock { get; private set; }
    
    public void Update(string code, string name) 
    { 
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("Invalid warehouse."); 
        
        WarehouseCode = code.Trim();
        
        Name = name.Trim(); 
    }
    
    public void Activate() => IsActive = true; 
    
    public void Deactivate() => IsActive = false;
    
    public void SetAllowNegativeStock(bool allowNegativeStock) => AllowNegativeStock = allowNegativeStock;
}
