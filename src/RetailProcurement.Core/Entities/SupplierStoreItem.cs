namespace RetailProcurement.Core.Entities;

public class SupplierStoreItem
{
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public int StoreItemId { get; set; }
    public StoreItem StoreItem { get; set; } = null!;

    public decimal SupplierPrice { get; set; }
    public int LeadTimeDays { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
