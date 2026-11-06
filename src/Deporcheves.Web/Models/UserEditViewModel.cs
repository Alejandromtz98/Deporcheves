using Deporcheves.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Deporcheves.Web.Models;

public class UserEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Usuario")]
    public string Username { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Nueva Contrasena (dejar en blanco para conservar)")]
    public string? Password { get; set; }

    [Display(Name = "Rol")]
    public UserRole Role { get; set; }

    [Display(Name = "Activo")]
    public bool IsActive { get; set; }
}