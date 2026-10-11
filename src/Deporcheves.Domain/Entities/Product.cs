using Deporcheves.Domain.Enums;

namespace Deporcheves.Domain.Entities;

/// <summary>
/// Producto vendible en Deporcheves: cerveza, botana o abarrote.
/// El precio incluye IVA (regla de negocio del negocio).
/// </summary>
public class Product
{
    public int Id { get; set; }

    /// <summary>
    /// Codigo interno unico. Se usa para busquedas rapidas en el punto de venta.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>
    /// Precio de venta al publico. Incluye IVA (16% Mexico).
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Costo de adquisicion del proveedor. Sin IVA.
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// Unidades disponibles en inventario.
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// Umbral bajo el cual se dispara una alerta de reposicion.
    /// </summary>
    public int MinimumStock { get; set; }

    public UnitOfMeasure Unit { get; set; } = UnitOfMeasure.Piece;

    /// <summary>
    /// Borrado logico. Un producto inactivo no aparece en el catalogo de venta
    /// pero conserva su historial de ventas asociado.
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // ------------------------------------------------------------
    // Comportamiento de dominio
    // ------------------------------------------------------------

    /// <summary>
    /// Reduce el stock del producto. Lanza excepcion si no hay suficiente.
    /// </summary>
    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));

        if (Stock < quantity)
            throw new InvalidOperationException(
                $"Stock insuficiente para '{Name}'. Disponible: {Stock}, solicitado: {quantity}.");

        Stock -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Aumenta el stock del producto. Se usa al recibir compras de proveedor.
    /// </summary>
    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(quantity));

        Stock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Indica si el producto esta por debajo o en el umbral minimo de stock.
    /// </summary>
    public bool IsBelowMinimumStock() => Stock <= MinimumStock;
}