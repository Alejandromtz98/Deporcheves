using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Deporcheves.Infrastructure.Persistence.Configurations;
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(150);
        builder.Property(x => x.BaseWeeklySalary).HasColumnType("decimal(18,2)");
    }
}