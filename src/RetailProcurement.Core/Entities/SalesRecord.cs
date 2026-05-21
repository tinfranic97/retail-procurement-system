namespace RetailProcurement.Core.Entities;

public class SalesRecord : BaseEntity
{
    public int StoreItemId { get; set; }
    public StoreItem StoreItem { get; set; } = null!;

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime SaleDate { get; set; }
}
