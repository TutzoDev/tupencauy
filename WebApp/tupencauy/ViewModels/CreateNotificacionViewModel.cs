// CreateNotificacionViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace tupencauy.ViewModels
{
    public class CreateNotificacionViewModel
    {
        [Required]
        public string UsuarioId { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public int SitioId { get; set; } 
    }
}