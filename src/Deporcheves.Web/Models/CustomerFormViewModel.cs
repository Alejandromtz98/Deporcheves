using System.ComponentModel.DataAnnotations;

namespace Deporcheves.Web.Models;

public class CustomerFormViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    [Display(Name = "Nombre completo")]
    public string Name { get; set; } = string.Empty;

    [StringLength(30)]
    [Display(Name = "Teléfono")]
    public string? Phone { get; set; }
}