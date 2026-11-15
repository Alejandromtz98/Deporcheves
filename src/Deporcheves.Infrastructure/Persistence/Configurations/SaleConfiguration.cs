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

        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.Total).HasColumnType("decimal(18,2)").IsRequired();

        // NUEVOS CAMPOS DE PAGOS
        builder.Property(s => s.AmountCash).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(s => s.AmountTransfer).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(s => s.AmountCard).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(s => s.AmountFiado).HasColumnType("decimal(18,2)").IsRequired();

        // Relación con CashSession
        builder.HasOne(s => s.CashSession)
            .WithMany()
            .HasForeignKey(s => s.CashSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con Items
        builder.HasMany(s => s.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}