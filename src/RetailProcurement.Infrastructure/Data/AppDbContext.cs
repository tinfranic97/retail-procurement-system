using Microsoft.EntityFrameworkCore;
using RetailProcurement.Core.Entities;
using RetailProcurement.Infrastructure.Data.Configurations;

namespace RetailProcurement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<StoreItem> StoreItems => Set<StoreItem>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierStoreItem> SupplierStoreItems => Set<SupplierStoreItem>();
    public DbSet<SalesRecord> SalesRecords => Set<SalesRecord>();
    public DbSet<QuarterlyPlanEntry> QuarterlyPlanEntries => Set<QuarterlyPlanEntry>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new StoreItemConfiguration());
        modelBuilder.ApplyConfiguration(new SupplierConfiguration());
        modelBuilder.ApplyConfiguration(new SupplierStoreItemConfiguration());
        modelBuilder.ApplyConfiguration(new SalesRecordConfiguration());
        modelBuilder.ApplyConfiguration(new QuarterlyPlanEntryConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && e.State == EntityState.Modified);

        foreach (var entry in entries)
            ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;

        return base.SaveChangesAsync(cancellationToken);
    }
}
