using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailProcurement.Core.Entities;

namespace RetailProcurement.Infrastructure.Data.Configurations;

public class SalesRecordConfiguration : IEntityTypeConfiguration<SalesRecord>
{
    public void Configure(EntityTypeBuilder<SalesRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.StoreItem)
            .WithMany(si => si.SalesRecords)
            .HasForeignKey(x => x.StoreItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Supplier)
            .WithMany(s => s.SalesRecords)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.HasIndex(x => x.SaleDate);
    }
}
