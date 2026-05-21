using Microsoft.EntityFrameworkCore;
using RetailProcurement.Core.Entities;
using RetailProcurement.Core.Interfaces.Repositories;
using RetailProcurement.Infrastructure.Data;

namespace RetailProcurement.Infrastructure.Repositories;

public class SupplierStoreItemRepository : ISupplierStoreItemRepository
{
    private readonly AppDbContext _context;

    public SupplierStoreItemRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<SupplierStoreItem>> GetAllAsync()
        => await _context.SupplierStoreItems
            .Include(x => x.Supplier)
            .Include(x => x.StoreItem)
            .ToListAsync();

    public async Task<SupplierStoreItem?> GetAsync(int supplierId, int storeItemId)
        => await _context.SupplierStoreItems
            .Include(x => x.Supplier)
            .Include(x => x.StoreItem)
            .FirstOrDefaultAsync(x => x.SupplierId == supplierId && x.StoreItemId == storeItemId);

    public async Task<IEnumerable<SupplierStoreItem>> GetBySupplierIdAsync(int supplierId)
        => await _context.SupplierStoreItems
            .Include(x => x.StoreItem)
            .Where(x => x.SupplierId == supplierId)
            .ToListAsync();

    public async Task<IEnumerable<SupplierStoreItem>> GetByStoreItemIdAsync(int storeItemId)
        => await _context.SupplierStoreItems
            .Include(x => x.Supplier)
            .Where(x => x.StoreItemId == storeItemId)
            .ToListAsync();

    public async Task<SupplierStoreItem> AddAsync(SupplierStoreItem entity)
    {
        await _context.SupplierStoreItems.AddAsync(entity);
        return entity;
    }

    public Task DeleteAsync(SupplierStoreItem entity)
    {
        _context.SupplierStoreItems.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(int supplierId, int storeItemId)
        => await _context.SupplierStoreItems
            .AnyAsync(x => x.SupplierId == supplierId && x.StoreItemId == storeItemId);
}
