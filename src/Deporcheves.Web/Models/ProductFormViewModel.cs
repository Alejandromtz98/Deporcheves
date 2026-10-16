using Deporcheves.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Deporcheves.Web.Models;

public class ProductFormViewModel
{
    [Required(ErrorMessage = "El SKU es obligatorio.")]
    [StringLength(50, ErrorMessage = "El SKU no puede exceder 50 caracteres.")]
    [Display(Name = "SKU")]
    public string Sku { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Descripcion")]
    public string? Description { get; set; }

    [Range(0.01, 999999, ErrorMessage = "El precio debe ser mayor a cero.")]
    [Display(Name = "Precio (IVA incluido)")]
    public decimal Price { get; set; }

    [Range(0, 999999)]
    [Display(Name = "Costo")]
    public decimal Cost { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Stock inicial")]
    public int Stock { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Stock minimo")]
    public int MinimumStock { get; set; }

    [Display(Name = "Unidad")]
    public UnitOfMeasure Unit { get; set; } = UnitOfMeasure.Piece;
}