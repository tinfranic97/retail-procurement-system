using RetailProcurement.Core.Entities;

namespace RetailProcurement.Core.Interfaces.Repositories;

public interface ISupplierStoreItemRepository
{
    Task<IEnumerable<SupplierStoreItem>> GetAllAsync();
    Task<SupplierStoreItem?> GetAsync(int supplierId, int storeItemId);
    Task<IEnumerable<SupplierStoreItem>> GetBySupplierIdAsync(int supplierId);
    Task<IEnumerable<SupplierStoreItem>> GetByStoreItemIdAsync(int storeItemId);
    Task<SupplierStoreItem> AddAsync(SupplierStoreItem entity);
    Task DeleteAsync(SupplierStoreItem entity);
    Task<bool> ExistsAsync(int supplierId, int storeItemId);
}
