using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deporcheves.Infrastructure.Persistence.Configurations;

public class InventoryCountConfiguration : IEntityTypeConfiguration<InventoryCount>
{
    public void Configure(EntityTypeBuilder<InventoryCount> builder)
    {
        builder.ToTable("InventoryCounts");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.TheoreticalQuantity).IsRequired();
        builder.Property(x => x.PhysicalQuantity).IsRequired();
        builder.Property(x => x.Difference).IsRequired();
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(300);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsAdjusted).IsRequired();

        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}