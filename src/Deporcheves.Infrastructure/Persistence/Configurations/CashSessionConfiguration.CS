using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Deporcheves.Infrastructure.Persistence.Configurations;

public class CashSessionConfiguration : IEntityTypeConfiguration<CashSession>
{
    public void Configure(EntityTypeBuilder<CashSession> builder)
    {
        builder.ToTable("CashSessions");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.OpenedAt).IsRequired();
        builder.Property(c => c.OpeningCash).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(c => c.ExpectedCash).HasColumnType("decimal(18,2)");
        builder.Property(c => c.ClosingCash).HasColumnType("decimal(18,2)");
        builder.Property(c => c.Difference).HasColumnType("decimal(18,2)");

        builder.Ignore(c => c.IsClosed); // Calculado en memoria

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Movements)
            .WithOne(m => m.CashSession)
            .HasForeignKey(m => m.CashSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Movements)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}