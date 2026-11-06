using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Deporcheves.Infrastructure.Persistence.Configurations;
public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("Settings"); builder.HasKey(x => x.Key);
        builder.Property(x => x.Key).HasMaxLength(50);
        builder.Property(x => x.Value).HasMaxLength(500);
    }
}