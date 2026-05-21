using Microsoft.EntityFrameworkCore;
using RetailProcurement.Core.Entities;
using RetailProcurement.Core.Interfaces.Repositories;
using RetailProcurement.Infrastructure.Data;

namespace RetailProcurement.Infrastructure.Repositories;

public class StoreItemRepository : Repository<StoreItem>, IStoreItemRepository
{
    public StoreItemRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<StoreItem>> GetAllWithSuppliersAsync()
        => await _context.StoreItems
            .Include(si => si.SupplierStoreItems)
                .ThenInclude(ssi => ssi.Supplier)
            .ToListAsync();

    public async Task<StoreItem?> GetByIdWithSuppliersAsync(int id)
        => await _context.StoreItems
            .Include(si => si.SupplierStoreItems)
                .ThenInclude(ssi => ssi.Supplier)
            .FirstOrDefaultAsync(si => si.Id == id);
}
