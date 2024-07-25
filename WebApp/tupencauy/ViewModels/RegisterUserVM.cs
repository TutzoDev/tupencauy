using System.ComponentModel.DataAnnotations;
using tupencauy.Models;

namespace tupencauy.ViewModels;
public class RegisterUserVM
{
    [Required]
    public string? Nombre { get; set; }
    [Required]
    public string? Username { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
    public string TenantId { get; set; }

    [Compare("Password", ErrorMessage = "Passwords no coincide.")]
    [Display(Name = "Confirmar Password")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }

    public string? SitioId { get; set; }
    }

