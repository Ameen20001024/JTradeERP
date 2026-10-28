namespace JTradeERP.Domain.MasterData;
public sealed class Product
{
    private Product() { }
    public Product(string productCode,
        string name,
        Guid categoryId,
        Guid stockUomId, 
        Guid taxCodeId, 
        decimal minimumStockLevel, 
        decimal reorderQuantity, 
        decimal salesPrice, 
        decimal purchasePrice) 
    
    {
        Id = Guid.NewGuid();
        
        Update(productCode,
            name,
            categoryId, 
            stockUomId, 
            taxCodeId, 
            minimumStockLevel, 
            reorderQuantity, 
            salesPrice, 
            purchasePrice); 
        
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public string ProductCode { get; private set; } = null!;
    
    public string Name { get; private set; } = null!;
    
    public Guid CategoryId { get; private set; }
    
    public Guid StockUomId { get; private set; }
    
    public Guid TaxCodeId { get; private set; }
    
    public decimal MinimumStockLevel { get; private set; }
    
    public decimal ReorderQuantity { get; private set; }
    
    public decimal SalesPrice { get; private set; }
    
    public decimal PurchasePrice { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public void Update(string productCode, 
        string name, 
        Guid categoryId, 
        Guid stockUomId, 
        Guid taxCodeId, 
        decimal minimumStockLevel, 
        decimal reorderQuantity, 
        decimal salesPrice, 
        decimal purchasePrice) 
    
    {
        if (string.IsNullOrWhiteSpace(productCode) || 
            string.IsNullOrWhiteSpace(name) || 
            minimumStockLevel < 0 || 
            reorderQuantity < 0 || 
            salesPrice < 0 || 
            purchasePrice < 0) 
            
            throw new ArgumentException("Invalid product.");
        
        ProductCode = productCode.Trim();
        
        Name = name.Trim();
        
        CategoryId = categoryId;
        
        StockUomId = stockUomId;
        
        TaxCodeId = taxCodeId;
        
        MinimumStockLevel = minimumStockLevel;
        
        ReorderQuantity = reorderQuantity;
        
        SalesPrice = salesPrice;
        
        PurchasePrice = purchasePrice;
    
    }

    public void Activate() => IsActive = true;
    
    public void Deactivate() => IsActive = false;
}
