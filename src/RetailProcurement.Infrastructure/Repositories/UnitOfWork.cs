using RetailProcurement.Core.Entities;
using RetailProcurement.Core.Interfaces.Repositories;
using RetailProcurement.Infrastructure.Data;

namespace RetailProcurement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IStoreItemRepository? _storeItems;
    private ISupplierRepository? _suppliers;
    private ISupplierStoreItemRepository? _supplierStoreItems;
    private IRepository<SalesRecord>? _salesRecords;
    private IRepository<QuarterlyPlanEntry>? _quarterlyPlanEntries;
    private IRepository<User>? _users;

    public UnitOfWork(AppDbContext context) => _context = context;

    public IStoreItemRepository StoreItems =>
        _storeItems ??= new StoreItemRepository(_context);

    public ISupplierRepository Suppliers =>
        _suppliers ??= new SupplierRepository(_context);

    public ISupplierStoreItemRepository SupplierStoreItems =>
        _supplierStoreItems ??= new SupplierStoreItemRepository(_context);

    public IRepository<SalesRecord> SalesRecords =>
        _salesRecords ??= new Repository<SalesRecord>(_context);

    public IRepository<QuarterlyPlanEntry> QuarterlyPlanEntries =>
        _quarterlyPlanEntries ??= new Repository<QuarterlyPlanEntry>(_context);

    public IRepository<User> Users =>
        _users ??= new Repository<User>(_context);

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
