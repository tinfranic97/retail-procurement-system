using Microsoft.EntityFrameworkCore;
using RetailProcurement.Core.DTOs.Statistics;
using RetailProcurement.Core.Interfaces.Repositories;
using RetailProcurement.Core.Interfaces.Services;
using RetailProcurement.Infrastructure.Data;
using RetailProcurement.Infrastructure.Patterns.BestOffer;
using RetailProcurement.Infrastructure.Patterns.QuarterlyPlan;

namespace RetailProcurement.Infrastructure.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppDbContext _context;
    private readonly IBestOfferStrategy _bestOfferStrategy;

    public StatisticsService(IUnitOfWork unitOfWork, AppDbContext context, IBestOfferStrategy bestOfferStrategy)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _bestOfferStrategy = bestOfferStrategy;
    }

    public async Task<SupplierStatisticsDto?> GetSupplierStatisticsAsync(int supplierId)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(supplierId);
        if (supplier is null) return null;

        var salesRecords = await _context.SalesRecords
            .Include(sr => sr.StoreItem)
            .Where(sr => sr.SupplierId == supplierId)
            .ToListAsync();

        var breakdown = salesRecords
            .GroupBy(sr => sr.StoreItemId)
            .Select(g => new ItemSalesSummary
            {
                StoreItemId = g.Key,
                ItemName = g.First().StoreItem.Name,
                QuantitySold = g.Sum(sr => sr.Quantity),
                Revenue = g.Sum(sr => sr.Quantity * sr.UnitPrice)
            })
            .ToList();

        return new SupplierStatisticsDto
        {
            SupplierId = supplierId,
            SupplierName = supplier.Name,
            TotalItemsSold = breakdown.Sum(b => b.QuantitySold),
            TotalRevenue = breakdown.Sum(b => b.Revenue),
            ItemBreakdown = breakdown
        };
    }

    public async Task<BestOfferDto?> GetBestOfferAsync(int productId)
    {
        var offers = await _unitOfWork.SupplierStoreItems.GetByStoreItemIdAsync(productId);
        var offerList = offers.ToList();

        if (!offerList.Any()) return null;

        var best = _bestOfferStrategy.SelectBestOffer(offerList);
        if (best is null) return null;

        return new BestOfferDto
        {
            StoreItemId = best.StoreItemId,
            StoreItemName = best.StoreItem.Name,
            SupplierId = best.SupplierId,
            SupplierName = best.Supplier.Name,
            SupplierPrice = best.SupplierPrice,
            LeadTimeDays = best.LeadTimeDays,
            SelectionCriteria = _bestOfferStrategy.CriteriaName
        };
    }

    public async Task<QuarterlyPlanDto> CreateQuarterlyPlanAsync(CreateQuarterlyPlanDto dto)
    {
        if (!await _unitOfWork.Suppliers.ExistsAsync(dto.SupplierId))
            throw new KeyNotFoundException($"Supplier {dto.SupplierId} not found.");

        var entry = new QuarterlyPlanBuilder()
            .ForSupplier(dto.SupplierId)
            .ForQuarter(dto.Quarter, dto.Year)
            .WithBudget(dto.PlannedBudget)
            .WithNotes(dto.Notes)
            .Build();

        await _unitOfWork.QuarterlyPlanEntries.AddAsync(entry);
        await _unitOfWork.SaveChangesAsync();

        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.SupplierId);

        return new QuarterlyPlanDto
        {
            Id = entry.Id,
            SupplierId = entry.SupplierId,
            SupplierName = supplier!.Name,
            Quarter = entry.Quarter,
            Year = entry.Year,
            PlannedBudget = entry.PlannedBudget,
            Notes = entry.Notes
        };
    }

    public async Task<IEnumerable<QuarterlyPlanDto>> GetCurrentQuarterlyPlanAsync()
    {
        var now = DateTime.UtcNow;
        var currentQuarter = (now.Month - 1) / 3 + 1;
        var currentYear = now.Year;

        var entries = await _context.QuarterlyPlanEntries
            .Include(e => e.Supplier)
            .Where(e => e.Quarter == currentQuarter && e.Year == currentYear)
            .ToListAsync();

        return entries.Select(e => new QuarterlyPlanDto
        {
            Id = e.Id,
            SupplierId = e.SupplierId,
            SupplierName = e.Supplier.Name,
            Quarter = e.Quarter,
            Year = e.Year,
            PlannedBudget = e.PlannedBudget,
            Notes = e.Notes
        });
    }
}
