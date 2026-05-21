using RetailProcurement.Core.DTOs.StoreItem;

namespace RetailProcurement.Core.Interfaces.Services;

public interface IStoreItemService
{
    Task<IEnumerable<StoreItemDto>> GetAllAsync();
    Task<StoreItemDto?> GetByIdAsync(int id);
    Task<StoreItemDto> CreateAsync(CreateStoreItemDto dto);
    Task<StoreItemDto?> UpdateAsync(int id, UpdateStoreItemDto dto);
    Task<bool> DeleteAsync(int id);
}
