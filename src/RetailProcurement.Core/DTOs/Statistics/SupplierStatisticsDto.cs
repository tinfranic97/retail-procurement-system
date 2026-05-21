namespace RetailProcurement.Core.DTOs.Statistics;

public class SupplierStatisticsDto
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int TotalItemsSold { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<ItemSalesSummary> ItemBreakdown { get; set; } = new();
}

public class ItemSalesSummary
{
    public int StoreItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}
