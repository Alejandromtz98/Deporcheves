namespace Deporcheves.Domain.Enums;

public enum InventoryMovementType
{
    InitialLoad = 1,  // Carga inicial de inventario
    Purchase = 2,     // Entrada por compra a proveedor
    Sale = 3,         // Salida por venta
    Adjustment = 4,   // Ajuste manual (Merma, rotura, corrección)
    Cancellation = 5  // Reversión por venta cancelada
}