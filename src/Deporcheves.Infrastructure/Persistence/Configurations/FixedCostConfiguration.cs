using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deporcheves.Infrastructure.Persistence.Configurations;

public class FixedCostConfiguration: IEntityTypeConfiguration<FixedCost>
{
    public void Configure(EntityTypeBuilder<FixedCost> builder)
    {
        builder.ToTable("FixedCosts");
        builder.HasIndex(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.MonthlyAmount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}