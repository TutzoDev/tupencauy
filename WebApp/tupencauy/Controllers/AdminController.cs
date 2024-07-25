using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.IO;
using tupencauy.Data;
using tupencauy.Models;
using tupencauy.ViewModels;

namespace tupencauy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(IHttpContextAccessor httpContextAccessor, AppDbContext context, UserManager<AppUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tenantId = _httpContextAccessor.HttpContext.Session.GetString("TenantId");

            // Obtener los datos necesarios desde la base de datos
            var usuarios = await _context.Users.Where(u => u.TenantId == tenantId).ToListAsync();
            var sitio = await _context.Sitios.FirstOrDefaultAsync(s => s.TenantId == tenantId);
            var pencasSitios = await _context.PencasSitio.Where(p => p.SitioTenantId == tenantId).ToListAsync();

            // Calcular saldo del sitio
            var recaudacionSitio = pencasSitios.Sum(p => p.Recaudacion);

            // Calcular estadísticas por PencaSitio
            var estadisticasPorPenca = pencasSitios.Select(p => {
                var usuariosPenca = _context.PencaSitioUsuario.Where(pu => pu.IdPencaSitio == p.Id).ToList();

                return new EstadisticasPorPenca
                {
                    PencaId = p.Id.ToString(),
                    PencaNombre = p.Nombre,
                    Inscriptos = usuariosPenca.Count,
                    MinimosUsuarios = p.Premio > 0 ? Math.Max((int)Math.Ceiling((p.Premio - p.Recaudacion) / (p.Costo * (1 - p.Comision / 100.0))) - usuariosPenca.Count, 0) : 0,
                    Premio = p.Premio,
                    Costo = p.Costo,
                    Recaudacion = p.Recaudacion,
                };
            }).ToList();

            // Crear el modelo de vista
            var model = new DashboardAdminVM
            {
                TotalUsuarios = usuarios.Count,
                Nombre = sitio.Nombre,
                TipoRegistro = sitio.TipoRegistro,
                UsuariosDisponibles = sitio.cantidadUsuarios,
                RecaudacionSitio = recaudacionSitio,
                EstadisticasPorPenca = estadisticasPorPenca
            };

            return View(model);
        }

        // GET: Admin/DetallePenca
        public async Task<IActionResult> DetallePenca(string pencaId)
        {
            var usuariosPenca = await _context.PencaSitioUsuario
                .Where(pu => pu.IdPencaSitio == pencaId)
                .Select(pu => new DatosUsuario
                {
                    Nombre = pu.NombreUsuario,
                    Puntaje = pu.Puntaje,
                    Aciertos = pu.Aciertos
                })
                .ToListAsync();

            var penca = await _context.PencasSitio.FindAsync(pencaId);
            if (penca == null)
            {
                return NotFound();
            }

            var model = new DetallePencaVM
            {
                Nombre = penca.Nombre,
                Usuarios = usuariosPenca
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult GenerateRegisterLink()
        {
            var tenantId = _httpContextAccessor.HttpContext.Session.GetString("TenantId");
            if (string.IsNullOrEmpty(tenantId))
            {
                return BadRequest("Tenant ID no esta disponible en la sesión.");
            }

            var registerUrl = Url.Action("Register", "Account", new { tenantId }, protocol: Request.Scheme);
            return Json(new { registerUrl });
        }

        // GET: Penca/Create
        [HttpGet]
        public async Task<IActionResult> CreatePencaSitio()
        {
            var tenantId = HttpContext.Session.GetString("TenantId");
            DateTime fechaActual = DateTime.Today;

            var pencaVM = new PencaSitioVM
            {
                SitioTenantId = tenantId,
                Pencas = await _context.Pencas
                    .Where(p => p.FechaInicio > fechaActual) // Filtrar por fecha de inicio posterior a hoy
                    .Where(p => !_context.PencasSitio.Any(ps => ps.SitioTenantId == tenantId && ps.PencaId == p.Id)) // Verificar que no exista en PencasSitio
                    .Select(p => new Penca // Proyección de resultados
                    {
                        Nombre = p.Nombre,
                        Id = p.Id,
                        FechaInicio = p.FechaInicio,

                    })
                    .ToListAsync()
            };
            return View(pencaVM);
        }


        [HttpPost]
        public async Task<IActionResult> CreatePenca(PencaSitioVM pencaEventoVM)
        {
            var sistema = await _context.Sistema.FirstOrDefaultAsync();
            var porcentajeComision = sistema.Comision;
            var nuevaComision = pencaEventoVM.Costo * (porcentajeComision / 100.0f);
            var pencaSitio = new PencaSitio
            {
                Id = Guid.NewGuid().ToString(),
                SitioTenantId = pencaEventoVM.SitioTenantId,
                PencaId = pencaEventoVM.SelectedPencaId,
                Costo = pencaEventoVM.Costo,
                Premio = pencaEventoVM.Premio,
                Nombre = pencaEventoVM.Nombre,
                Inscriptos = 0,
                Recaudacion = 0,
                Comision = nuevaComision

            };
            _context.PencasSitio.Add(pencaSitio);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> Usuarios()
        {
            var tenantId = HttpContext.Session.GetString("TenantId");

            var currentUser = await _userManager.GetUserAsync(User);
            var userId = currentUser.Id; // Obtiene el ID del usuario actual

            var sitio = await _context.Sitios
                                .FirstOrDefaultAsync(s => s.TenantId == tenantId);

            var usuarios = await _context.Usuarios
                .Where(s => s.TenantId == tenantId && s.Id != userId)
                .ToListAsync();

            var model = new AdminViewModel
            {
                Usuarios = usuarios,
                Sitio = sitio
            };
            return View(model);
        }


        // POST: Usuarios/AprobarSitio/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprobarUsuario(string id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.Status = false;
                _context.Update(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction("Usuarios", "Admin");

            }
            catch (DbUpdateConcurrencyException)
            {
                // Manejo de error en caso de problemas con la concurrencia
                ModelState.AddModelError("", "No se pudo actualizar el usuario. Inténtalo de nuevo.");
                return View(await _context.Usuarios.ToListAsync());
            }
        }

        // POST: Usuarios/HabilitarSitio/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HabilitarUsuario(string id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.Status = true;
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Usuarios", "Admin");
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "No se pudo habilitar el usuario. Inténtalo de nuevo.");
                return View(await _context.Usuarios.ToListAsync());
            }
        }

        // POST: Usuarios/DeshabilitarSitio/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeshabilitarUsuario(string id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.Status = false;
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Usuarios", "Admin");
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "No se pudo deshabilitar el usuario. Inténtalo de nuevo.");
                return View(await _context.Usuarios.ToListAsync());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Pencas()
        {
            var tenantId = HttpContext.Session.GetString("TenantId");

            var pencas = await _context.PencasSitio
                        .Where(s => s.SitioTenantId == tenantId)
                        .ToListAsync();
            var model = new AdminViewModel
            {
                PencasSitio = pencas
            };
            return View(model);
        }

        // GET: EDITAR PREMIOS PENCA
        [HttpGet]
        public async Task<IActionResult> EditPencaSitio(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pencaSitio = await _context.PencasSitio.FindAsync(id);
            int inscriptos = await _context.PencaSitioUsuario
                .Where(p => p.IdPencaSitio == id)
                .CountAsync();

            if (pencaSitio == null)
            {
                return NotFound();
            }

            var model = new PencaSitioVM
            {
                Id = pencaSitio.Id,
                Nombre = pencaSitio.Nombre,
                Premio = pencaSitio.Premio,
            };

            return View(model);
        }

        // POST: EDITAR PREMIOS PENCA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPencaSitio(string id, [Bind("Nombre,Premio")] PencaSitioVM model)
        {
            try
            {
                var pencaSitio = await _context.PencasSitio.FindAsync(id);

                if (pencaSitio == null)
                {
                    return NotFound(); // Manejo de caso donde no se encuentra el registro
                }

                // Actualizar los valores con los del modelo
                pencaSitio.Nombre = model.Nombre;
                pencaSitio.Premio = model.Premio;
                _context.Update(pencaSitio);
                await _context.SaveChangesAsync();

                return RedirectToAction("Pencas", "Admin");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError("", "Error al intentar guardar los cambios. Inténtelo nuevamente.");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "Se produjo un error inesperado. Por favor, contacte al administrador.");
            }

            return RedirectToAction("Pencas", "Admin");
        }

    }
}

