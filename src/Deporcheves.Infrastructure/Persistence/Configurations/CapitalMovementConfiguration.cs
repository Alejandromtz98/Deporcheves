using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Deporcheves.Infrastructure.Persistence.Configurations;
public class CapitalMovementConfiguration : IEntityTypeConfiguration<CapitalMovement>
{
    public void Configure(EntityTypeBuilder<CapitalMovement> builder)
    {
        builder.ToTable("CapitalMovements"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Type).HasConversion<int>();
        builder.Property(x => x.OwnerName).HasMaxLength(150);
    }
}