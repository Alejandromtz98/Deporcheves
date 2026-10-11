namespace Deporcheves.Domain.Enums;

/// <summary>
/// Metodos de pago aceptados en el punto de venta.
/// Los valores numericos se persisten en base de datos, no los modifiques
/// sin migracion correspondiente.
/// </summary>
public enum PaymentMethod
{
    Cash = 1,
    Card = 2,
    Transfer = 3
}