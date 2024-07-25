using System.ComponentModel.DataAnnotations;

namespace tupencauy.ViewModels
{
    public class RequestSitioViewModel
    {
        public string NombreSitio { get; set; }
        public string Url { get; set; }
        public string TipoRegistro { get; set; }
        public string ColorPrincipal { get; set; }
        public string ColorSecundario { get; set; }
        public string ColorTipografia { get; set; }

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

        [Compare("Password", ErrorMessage = "Passwords no coincide.")]
        [Display(Name = "Confirmar Password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
