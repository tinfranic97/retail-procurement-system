namespace RetailProcurement.Core.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IStoreItemRepository StoreItems { get; }
    ISupplierRepository Suppliers { get; }
    ISupplierStoreItemRepository SupplierStoreItems { get; }
    IRepository<Entities.SalesRecord> SalesRecords { get; }
    IRepository<Entities.QuarterlyPlanEntry> QuarterlyPlanEntries { get; }
    IRepository<Entities.User> Users { get; }
    Task<int> SaveChangesAsync();
}
