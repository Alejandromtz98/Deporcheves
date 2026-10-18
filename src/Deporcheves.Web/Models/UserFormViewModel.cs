using Deporcheves.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Deporcheves.Web.Models;

public class UserFormViewModel
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contrasena es obligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Rol")]
    public UserRole Role { get; set; } = UserRole.Cashier;
}