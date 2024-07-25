using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tupencauy.Data;

namespace tupencauy.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UsuarioController
        public ActionResult Index()
        {
            return View();
        }
    }
}
