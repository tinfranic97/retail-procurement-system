using RetailProcurement.Core.DTOs.Statistics;

namespace RetailProcurement.Core.Interfaces.Services;

public interface IStatisticsService
{
    Task<SupplierStatisticsDto?> GetSupplierStatisticsAsync(int supplierId);
    Task<BestOfferDto?> GetBestOfferAsync(int productId);
    Task<QuarterlyPlanDto> CreateQuarterlyPlanAsync(CreateQuarterlyPlanDto dto);
    Task<IEnumerable<QuarterlyPlanDto>> GetCurrentQuarterlyPlanAsync();
}
