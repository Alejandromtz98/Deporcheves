using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deporcheves.Infrastructure.Persistence.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SaleNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(s => s.SaleNumber)
            .IsUnique();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.Total)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.PaymentMethod)
            .HasConversion<int>()
            .IsRequired();

        // Relacion uno-a-muchos con SaleItem.
        // La navegacion se accede mediante el backing field _items.
        builder.HasMany(s => s.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}