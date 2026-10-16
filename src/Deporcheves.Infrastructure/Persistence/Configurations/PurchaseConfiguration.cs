using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deporcheves.Infrastructure.Persistence.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PurchaseNumber).IsRequired().HasMaxLength(30);
        builder.HasIndex(p => p.PurchaseNumber).IsUnique();

        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.Total).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.Notes).HasMaxLength(500);

        builder.HasOne(p => p.Supplier)
            .WithMany()
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Items)
            .WithOne(i => i.Purchase)
            .HasForeignKey(i => i.PurchaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}