using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deporcheves.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuracion de mapeo de la entidad Product a la tabla Products.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.Sku)
            .IsUnique();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.Cost)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.Stock)
            .IsRequired();

        builder.Property(p => p.MinimumStock)
            .IsRequired();

        builder.Property(p => p.Unit)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);
    }
}