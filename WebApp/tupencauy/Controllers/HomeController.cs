using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using tupencauy.Data;
using tupencauy.Models;
using tupencauy.ViewModels;

namespace tupencauy.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sitios = await _context.Sitios.Where(s => s.Status == true).ToListAsync();
            var notificaciones = new List<NotificacionViewModel>();

            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                var notificacionesDb = await _context.Notificaciones
                                                      .Where(n => n.UsuarioId == userId)
                                                      .Include(n => n.Sitio)
                                                      .ToListAsync();
                if (notificacionesDb != null)
                {
                    notificaciones = notificacionesDb.Select(n => new NotificacionViewModel
                    {
                        Id = n.Id,
                        Mensaje = n.Mensaje,
                        FechaCreacion = n.FechaCreacion,
                        FechaEnvio = n.FechaEnvio,
                        SitioNombre = n.Sitio?.Nombre,
                        Leida = n.Leida
                    }).ToList();
                }
            }

            var model = new IndexViewModel
            {
                Sitios = sitios,
                Notificaciones = notificaciones
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}