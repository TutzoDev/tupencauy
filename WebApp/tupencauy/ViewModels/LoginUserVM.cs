using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using tupencauy.Models;

namespace tupencauy.ViewModels;
public class LoginUserVM
{
    [Required(ErrorMessage = "Username es requerido.")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password es requerido.")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }

    //public string UrlRetorno { get; set; }

    //public IList<AuthenticationScheme> LoginExternos { get; set; }

    public string? TenantId { get; set; }
}