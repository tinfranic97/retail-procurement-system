namespace RetailProcurement.Core.Entities;

public class StoreItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public int StockQuantity { get; set; }

    public ICollection<SupplierStoreItem> SupplierStoreItems { get; set; } = new List<SupplierStoreItem>();
    public ICollection<SalesRecord> SalesRecords { get; set; } = new List<SalesRecord>();
}
