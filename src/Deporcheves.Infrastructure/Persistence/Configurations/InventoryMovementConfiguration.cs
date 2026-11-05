using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deporcheves.Infrastructure.Persistence.Configurations;

public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("InventoryMovements");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Type).HasConversion<int>().IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.UnitCost).HasColumnType("decimal(18,4)").IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.Reference).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Reason).HasMaxLength(300);

        builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}