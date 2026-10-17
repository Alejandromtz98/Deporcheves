using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deporcheves.Infrastructure.Persistence.Configurations;

public class CashMovementConfiguration : IEntityTypeConfiguration<CashMovement>
{
    public void Configure(EntityTypeBuilder<CashMovement> builder)
    {
        builder.ToTable("CashMovements");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Type).HasConversion<int>().IsRequired();
        builder.Property(m => m.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(m => m.Reason).IsRequired().HasMaxLength(300);
        builder.Property(m => m.CreatedAt).IsRequired();
    }
}