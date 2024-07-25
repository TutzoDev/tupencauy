using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tupencauy.Models
{
    public class Notificacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public AppUser Usuario { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool Leida { get; set; }

        public DateTime FechaEnvio { get; set; }

        [Required]
        public int SitioId { get; set; }

        [ForeignKey("SitioId")]
        public Sitio Sitio { get; set; }

        public DateTime FechaNotificacion { get; set; }
    }
}