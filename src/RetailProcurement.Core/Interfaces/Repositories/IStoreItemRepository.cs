using RetailProcurement.Core.Entities;

namespace RetailProcurement.Core.Interfaces.Repositories;

public interface IStoreItemRepository : IRepository<StoreItem>
{
    Task<IEnumerable<StoreItem>> GetAllWithSuppliersAsync();
    Task<StoreItem?> GetByIdWithSuppliersAsync(int id);
}
