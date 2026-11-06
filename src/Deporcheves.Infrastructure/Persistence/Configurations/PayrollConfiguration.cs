using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Deporcheves.Infrastructure.Persistence.Configurations;
public class PayrollConfiguration : IEntityTypeConfiguration<Payroll>
{
    public void Configure(EntityTypeBuilder<Payroll> builder)
    {
        builder.ToTable("Payrolls"); builder.HasKey(x => x.Id);
        builder.Property(x => x.BaseSalary).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Advances).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Deductions).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Adjustments).HasColumnType("decimal(18,2)");
        builder.Property(x => x.TotalCalculated).HasColumnType("decimal(18,2)");
        builder.Property(x => x.AmountPaid).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Balance).HasColumnType("decimal(18,2)");
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId);
    }
}