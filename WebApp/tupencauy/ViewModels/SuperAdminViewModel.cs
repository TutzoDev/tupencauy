using Microsoft.AspNetCore.Identity;
using tupencauy.Models;

namespace tupencauy.ViewModels
{
    public class SuperAdminViewModel
    {
        public List<Sitio> Sitios { get; set; }
        public List<AppUser> Usuarios { get; set; }
    }
}
