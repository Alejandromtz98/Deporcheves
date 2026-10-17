using System.ComponentModel.DataAnnotations;

namespace Deporcheves.Web.Models;

public class SupplierFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Contacto")]
    public string? ContactName { get; set; }

    [StringLength(30)]
    [Display(Name = "Telefono")]
    public string? Phone { get; set; }

    [StringLength(150)]
    [EmailAddress(ErrorMessage = "Correo invalido.")]
    [Display(Name = "Correo")]
    public string? Email { get; set; }

    [StringLength(300)]
    [Display(Name = "Direccion")]
    public string? Address { get; set; }
}