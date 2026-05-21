namespace RetailProcurement.Core.Entities;

public class QuarterlyPlanEntry : BaseEntity
{
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public int Quarter { get; set; }
    public int Year { get; set; }
    public decimal PlannedBudget { get; set; }
    public string Notes { get; set; } = string.Empty;
}
