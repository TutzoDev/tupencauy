using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using tupencauy.Data;
using tupencauy.Models;
using tupencauy.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Policy;
using System.Linq;

namespace tupencauy.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAntiforgery _antiforgery;
        private readonly IWebHostEnvironment _environment;


        public SuperAdminController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IAntiforgery antiforgery, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _antiforgery = antiforgery;
            _environment = environment;

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Obtener los datos necesarios desde la base de datos
            var usuarios = await _context.Users.ToListAsync();
            var sitios = await _context.Sitios.ToListAsync();
            var pencasSitio = await _context.PencasSitio.ToListAsync();
            var pencaSitioUsuarios = await _context.PencaSitioUsuario.ToListAsync();
            var sistema = await _context.Sistema.FirstOrDefaultAsync();


            // Calcular estadísticas de sitios
            var sitioEstadisticas = sitios.Select(s => new EstadisticasSitios
            {
                NombreSitio = s.Nombre,
                CantidadUsuarios = usuarios.Count(u => u.TenantId == s.TenantId),
                Recaudacion = (double)pencasSitio.Where(p => p.SitioTenantId == s.TenantId).Sum(p => p.Recaudacion)
            }).ToList();

            var estadisticasPencas = pencasSitio.Select(ps => new EstadisticasPencas
             {
                NombrePenca = ps.Nombre,
                Costo = ps.Costo,
                Premio = ps.Premio,
                CantidadUsuarios = ps.Inscriptos,
                Recaudacion = ps.Recaudacion
            }).ToList(); ;

            var model = new DashboardVM
            {
                TotalUsuarios = usuarios.Count,
                SaldoSistema = (decimal)(sistema?.Billetera ?? 0),
                TotalSitios = sitios.Count,
                EstadisticasSitios = sitioEstadisticas,
                EstadisticasPencas = estadisticasPencas

            };

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Sitios()
        {
            var sitios = await _context.Sitios.ToListAsync();
            var model = new SuperAdminViewModel
            {
                Sitios = sitios
            };
            return View(model);
        }


        public async Task<IActionResult> Pencas()
        {
            var pencas = await _context.Pencas.ToListAsync();

            return View(pencas);
        }

        public async Task<IActionResult> EventosDeportivos()
        {
            var eventos = await _context.UnoVsUno
            .Select(e => new EventoDeportivoVM
            {
                Id = e.Id,
                Nombre = e.Nombre,
                FechaInicio = e.FechaInicio,
                EquipoUno = e.EquipoUno,
                EquipoDos = e.EquipoDos,
            }).ToListAsync();

            return View(eventos);
         
        }

        [HttpGet]
        public async Task<IActionResult> Usuarios()
        {
            var sitios = await _context.Sitios.ToListAsync();
            var usuarios = await _context.Usuarios.ToListAsync();

            var usuariosFiltrados = new List<AppUser>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                if (!roles.Contains("SuperAdmin"))
                {
                    usuariosFiltrados.Add(usuario);
                }
            }

            var model = new SuperAdminViewModel
            {
                Usuarios = usuariosFiltrados,
                Sitios = sitios
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AprobarRecarga()
        {
            var uploads = Path.Combine(@"C:\uploads\img");

            var recargasPendientes = await _context.Recargas
                .Where(r => r.Aprobado == null)
                .Join(_context.Usuarios, r => r.IdUsuario, u => u.Id, (r, u) => new { Recarga = r, Usuario = u })
                .Join(_context.Sitios, ru => ru.Usuario.TenantId, s => s.TenantId, (ru, s) => new RecargaVM
                {
                    Id = ru.Recarga.Id,
                    NombreUsuario = ru.Usuario.Name,
                    MontoRecarga = ru.Recarga.Carga,
                    NombreSitio = s.Nombre,
                    ComprobantePath = ru.Recarga.ComprobantePath,
                    Aprobado = ru.Recarga.Aprobado
                })
                .ToListAsync();

            return View(recargasPendientes);
        }

        [HttpPost]
        public async Task<IActionResult> AprobarRecarga(string id)
        {
            var recarga = await _context.Recargas.FindAsync(id);
            if (recarga == null)
            {
                return NotFound();
            }

            recarga.Aprobado = true;
            var usuario = await _context.Users.FindAsync(recarga.IdUsuario);
            if (usuario != null)
            {
                usuario.Saldo += recarga.Carga;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AprobarRecarga));
        }
        [HttpPost]
        public async Task<IActionResult> DesaprobarRecarga(string id)
        {
            var recarga = await _context.Recargas.FindAsync(id);
            if (recarga == null)
            {
                return NotFound();
            }

            recarga.Aprobado = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AprobarRecarga));
        }

        public async Task<IActionResult> ListarPencas()
        {
            return View(await _context.Pencas.ToListAsync());
        }

        public async Task<IActionResult> ListarEventosDeportivos()
        {
            return View(await _context.EventosDeportivos.ToListAsync());
        }

        public async Task<IActionResult> Sistema()
        {
            var sitios = await _context.Sitios.ToListAsync();
            var sistema = await _context.Sistema.FirstOrDefaultAsync();
            var infoSitiosList = new List<infoSitio>();

            foreach (var sitio in sitios)
            {
                var infositio = new infoSitio()
                {
                    Id = sitio.Id,
                    Nombre = sitio.Nombre,
                    cantidadUsuarios = sitio.cantidadUsuarios
                };
                infoSitiosList.Add(infositio);
            }

            var model = new SistemaVM
            {
                sitios = infoSitiosList,
                Comision = sistema?.Comision ?? 0,
                Billetera = sistema?.Billetera ?? 0
            };

            ViewData["AntiforgeryToken"] = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> FinalizarPenca(String id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var penca = await _context.Pencas.FindAsync(id);
            if (penca == null)
            {
                return NotFound();
            }
            // poner penca en finalizada
            penca.IsFinish = true;
            _context.Update(penca);
            await _context.SaveChangesAsync();

            //otorgar premios
            //obtengo las pencas con el mismo ID en los sitios
            var pencasSitios = await _context.PencasSitio
                         .Where(p => p.PencaId == id)
                         .ToListAsync();
            //entro a cada penca dentro de un sitio
            foreach (var pencasitio in pencasSitios)
            {
                //obtengo al usuario ganador
                var ganador = await _context.PencaSitioUsuario
                         .Where(p => p.IdPencaSitio == pencasitio.Id)
                         .OrderByDescending(p => p.Puntaje)
                         .ThenByDescending(p => p.Aciertos)
                         .FirstOrDefaultAsync();
                if (ganador != null)
                {
                    var userGanador = await _context.Usuarios.FindAsync(ganador.IdUsuario);
                    //le otorgo el premio
                    userGanador.Saldo += pencasitio.Premio;
                    //quito el premio de la recaudacion
                    pencasitio.Recaudacion -= pencasitio.Premio;

                    _context.Update(userGanador);
                    _context.Update(pencasitio);
                    await _context.SaveChangesAsync();
                }
                
            }
            return RedirectToAction(nameof(Pencas));
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarComision(SistemaVM model)
        {

            var sistema = await _context.Sistema.FirstOrDefaultAsync();
            if (sistema != null)
            {
                sistema.Comision = model.Comision;
                _context.Update(sistema);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Sistema));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCantidadUsuarios([FromBody] UpdateCantidadUsuariosModel model)
        {
            var sitio = await _context.Sitios.FindAsync(model.SitioId);
            if (sitio != null)
            {
                sitio.cantidadUsuarios = model.NewCantidad;
                _context.Update(sitio);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return NotFound(); 

        }

        // GET: Notificaciones/Crear
        public IActionResult CrearNotificacion()
        {
            return RedirectToAction("Crear", "Notificaciones");
        }

        public IActionResult Login()
        {
            var model = new LoginUserVM();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username!);
                var roles = await _userManager.GetRolesAsync(user);
                //recupero el tenantId que guarde en la session cuando paso por el middleware
                if (roles.Contains("SuperAdmin"))
                {
                    // El usuario es SuperAdmin
                    var result = await _signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "SuperAdmin");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Intento de inicio de sesión inválido.");
                    }
                }
                else
                {
                    // El TenantId no coincide
                    ModelState.AddModelError(string.Empty, "El usuario no está registrado o no es Super Admin");
                    return View(model);
                }
            }
            // Si el modelo no es válido, agregar un mensaje de error genérico
            ModelState.AddModelError(string.Empty, "Algo falló al mostrar el formulario.");
            return View(model);
        }
    }
}

