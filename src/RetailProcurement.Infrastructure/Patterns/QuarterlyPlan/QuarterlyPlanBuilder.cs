using RetailProcurement.Core.Entities;

namespace RetailProcurement.Infrastructure.Patterns.QuarterlyPlan;

public class QuarterlyPlanBuilder
{
    private int _supplierId;
    private int _quarter;
    private int _year;
    private decimal _plannedBudget;
    private string _notes = string.Empty;

    public QuarterlyPlanBuilder ForSupplier(int supplierId)
    {
        _supplierId = supplierId;
        return this;
    }

    public QuarterlyPlanBuilder ForQuarter(int quarter, int year)
    {
        _quarter = quarter;
        _year = year;
        return this;
    }

    public QuarterlyPlanBuilder WithBudget(decimal budget)
    {
        _plannedBudget = budget;
        return this;
    }

    public QuarterlyPlanBuilder WithNotes(string notes)
    {
        _notes = notes;
        return this;
    }

    public QuarterlyPlanEntry Build()
    {
        if (_supplierId <= 0) throw new InvalidOperationException("Supplier ID must be set.");
        if (_quarter < 1 || _quarter > 4) throw new InvalidOperationException("Quarter must be between 1 and 4.");
        if (_year < 2000) throw new InvalidOperationException("Year is invalid.");

        return new QuarterlyPlanEntry
        {
            SupplierId = _supplierId,
            Quarter = _quarter,
            Year = _year,
            PlannedBudget = _plannedBudget,
            Notes = _notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
