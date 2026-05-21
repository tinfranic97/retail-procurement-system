using RetailProcurement.Core.DTOs.Statistics;

namespace RetailProcurement.Core.Interfaces.Services;

public interface ISupplierStoreItemService
{
    Task<IEnumerable<SupplierStoreItemDto>> GetAllAsync();
    Task<SupplierStoreItemDto> CreateAsync(CreateSupplierStoreItemDto dto);
    Task<bool> DeleteAsync(int supplierId, int storeItemId);
}
