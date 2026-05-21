using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailProcurement.Core.Entities;

namespace RetailProcurement.Infrastructure.Data.Configurations;

public class QuarterlyPlanEntryConfiguration : IEntityTypeConfiguration<QuarterlyPlanEntry>
{
    public void Configure(EntityTypeBuilder<QuarterlyPlanEntry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Supplier)
            .WithMany(s => s.QuarterlyPlanEntries)
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.PlannedBudget).HasPrecision(18, 2);
        builder.HasIndex(x => new { x.SupplierId, x.Quarter, x.Year }).IsUnique();
    }
}
