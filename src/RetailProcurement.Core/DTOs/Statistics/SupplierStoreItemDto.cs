using System.ComponentModel.DataAnnotations;

namespace RetailProcurement.Core.DTOs.Statistics;

public class SupplierStoreItemDto
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int StoreItemId { get; set; }
    public string StoreItemName { get; set; } = string.Empty;
    public decimal SupplierPrice { get; set; }
    public int LeadTimeDays { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateSupplierStoreItemDto
{
    [Required]
    public int SupplierId { get; set; }

    [Required]
    public int StoreItemId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal SupplierPrice { get; set; }

    [Range(1, 365)]
    public int LeadTimeDays { get; set; }
}
