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

    public DbSet<Sale> Sales => Set<Sale>();

    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<CashSession> CashSessions => Set<CashSession>();
    public DbSet<CashMovement> CashMovements => Set<CashMovement>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas las configuraciones que implementen IEntityTypeConfiguration<T>
        // ubicadas en el ensamblado de Infrastructure.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeporchevesDbContext).Assembly);
    }
}