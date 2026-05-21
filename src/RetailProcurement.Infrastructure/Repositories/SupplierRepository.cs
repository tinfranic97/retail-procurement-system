using Microsoft.EntityFrameworkCore;
using RetailProcurement.Core.Entities;
using RetailProcurement.Core.Interfaces.Repositories;
using RetailProcurement.Infrastructure.Data;

namespace RetailProcurement.Infrastructure.Repositories;

public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Supplier>> GetAllWithItemsAsync()
        => await _context.Suppliers
            .Include(s => s.SupplierStoreItems)
                .ThenInclude(ssi => ssi.StoreItem)
            .ToListAsync();

    public async Task<Supplier?> GetByIdWithItemsAsync(int id)
        => await _context.Suppliers
            .Include(s => s.SupplierStoreItems)
                .ThenInclude(ssi => ssi.StoreItem)
            .FirstOrDefaultAsync(s => s.Id == id);
}
