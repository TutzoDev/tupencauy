using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace tupencauy.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        [MaxLength(100)]
        [Required]
        public string? Name { get; set; }
        public string? TenantId { get; set; }
        public DateTime Tmstmp { get; set; }
        public double Saldo { get; set; }
        public bool? Status { get; set; }
        public bool RecibirNotificaciones { get; set; }
    }
}
