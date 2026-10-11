using Deporcheves.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Deporcheves.Infrastructure.Persistence;

/// <summary>
/// Contexto de Entity Framework Core para el sistema Deporcheves.
/// Expone los DbSet de cada entidad persistida.
/// </summary>
public class DeporchevesDbContext : DbContext
{
    public DeporchevesDbContext(DbContextOptions<DeporchevesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas las configuraciones que implementen IEntityTypeConfiguration<T>
        // ubicadas en el ensamblado de Infrastructure.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeporchevesDbContext).Assembly);
    }
}