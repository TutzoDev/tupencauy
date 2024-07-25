using tupencauy.Models;

namespace tupencauy.ViewModels
{
    public class AdminViewModel
    {
        public Sitio Sitio { get; set; }
        public List<AppUser> Usuarios { get; set; }
        public List<PencaSitio> PencasSitio { get; set; }
    }
}
