using RetailProcurement.Core.Entities;

namespace RetailProcurement.Core.Interfaces.Repositories;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<IEnumerable<Supplier>> GetAllWithItemsAsync();
    Task<Supplier?> GetByIdWithItemsAsync(int id);
}
