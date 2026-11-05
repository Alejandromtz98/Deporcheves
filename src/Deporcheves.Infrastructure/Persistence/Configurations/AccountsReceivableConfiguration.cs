using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deporcheves.Infrastructure.Persistence.Configurations;

public class AccountsReceivableConfiguration : IEntityTypeConfiguration<AccountsReceivable>
{
    public void Configure(EntityTypeBuilder<AccountsReceivable> builder)
    {
        builder.ToTable("AccountsReceivables");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(a => a.PaidAmount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(a => a.Balance).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(a => a.Status).HasConversion<int>().IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();

        builder.HasOne(a => a.Sale).WithMany().HasForeignKey(a => a.SaleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Customer).WithMany().HasForeignKey(a => a.CustomerId).OnDelete(DeleteBehavior.Restrict);
    }
}