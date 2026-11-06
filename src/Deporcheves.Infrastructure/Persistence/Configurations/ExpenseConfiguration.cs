using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Deporcheves.Infrastructure.Persistence.Configurations;
public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Category).HasMaxLength(50);
        builder.Property(x => x.Description).HasMaxLength(300);
        builder.Property(x => x.Status).HasConversion<int>();
    }
}