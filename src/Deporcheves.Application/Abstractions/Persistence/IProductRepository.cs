using Deporcheves.Domain.Entities;

namespace Deporcheves.Application.Abstractions.Persistence;

/// <summary>
/// Contrato de acceso a datos de productos.
/// La implementacion concreta vive en Infrastructure.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Obtiene un producto por su identificador. Devuelve null si no existe.
    /// </summary>
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un producto por su SKU. Devuelve null si no existe.
    /// </summary>
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Devuelve todos los productos activos ordenados por nombre.
    /// </summary>
    Task<IReadOnlyList<Product>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca productos activos cuyo nombre o SKU contengan el termino indicado.
    /// </summary>
    Task<IReadOnlyList<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si ya existe un producto con el SKU indicado.
    /// El parametro excludeId permite excluir un producto especifico, util al editar.
    /// </summary>
    Task<bool> ExistsWithSkuAsync(string sku, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un nuevo producto al contexto (aun no persistido).
    /// </summary>
    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca un producto existente como modificado.
    /// </summary>
    void Update(Product product);

    /// <summary>
    /// Persiste todos los cambios pendientes en el contexto.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}