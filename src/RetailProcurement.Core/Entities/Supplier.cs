namespace RetailProcurement.Core.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;

    public ICollection<SupplierStoreItem> SupplierStoreItems { get; set; } = new List<SupplierStoreItem>();
    public ICollection<SalesRecord> SalesRecords { get; set; } = new List<SalesRecord>();
    public ICollection<QuarterlyPlanEntry> QuarterlyPlanEntries { get; set; } = new List<QuarterlyPlanEntry>();
}
