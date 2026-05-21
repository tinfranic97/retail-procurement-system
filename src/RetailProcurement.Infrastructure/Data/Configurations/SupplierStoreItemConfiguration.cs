using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailProcurement.Core.Entities;

namespace RetailProcurement.Infrastructure.Data.Configurations;

public class SupplierStoreItemConfiguration : IEntityTypeConfiguration<SupplierStoreItem>
{
    public void Configure(EntityTypeBuilder<SupplierStoreItem> builder)
    {
        builder.HasKey(x => new { x.SupplierId, x.StoreItemId });

        builder.HasOne(x => x.Supplier)
            .WithMany(s => s.SupplierStoreItems)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.StoreItem)
            .WithMany(si => si.SupplierStoreItems)
            .HasForeignKey(x => x.StoreItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.SupplierPrice).HasPrecision(18, 2);
    }
}
