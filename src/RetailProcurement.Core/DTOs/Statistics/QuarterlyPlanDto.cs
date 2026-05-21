namespace RetailProcurement.Core.DTOs.Statistics;

public class QuarterlyPlanDto
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int Quarter { get; set; }
    public int Year { get; set; }
    public decimal PlannedBudget { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class CreateQuarterlyPlanDto
{
    public int SupplierId { get; set; }
    public int Quarter { get; set; }
    public int Year { get; set; }
    public decimal PlannedBudget { get; set; }
    public string Notes { get; set; } = string.Empty;
}
