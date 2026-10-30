using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.VisualBasic;

namespace Deporcheves.Infrastructure.Persistence.Configurations;

public class FinancialSnapshotConfiguration: IEntityTypeConfiguration<FinancialSnapshot>
{
    public void Configure(EntityTypeBuilder<FinancialSnapshot> builder)
    {
        builder.ToTable("FinancialSnapshots");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PeriodStart).IsRequired();
        builder.Property(x => x.PeriodEnd).IsRequired();
        
        builder.Property(x => x.TotalRevenue).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.TotalCogs).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.TotalExpenses).HasColumnType("decimal(18,2)").IsRequired();

        // Propiedades calculadas que no se guardan en la DB
        builder.Ignore(x => x.GrossProfit);
        builder.Ignore(x => x.NetProfit);
    }
}