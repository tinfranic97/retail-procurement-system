namespace RetailProcurement.Core.DTOs.Statistics;

public class BestOfferDto
{
    public int StoreItemId { get; set; }
    public string StoreItemName { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal SupplierPrice { get; set; }
    public int LeadTimeDays { get; set; }
    public string SelectionCriteria { get; set; } = string.Empty;
}
