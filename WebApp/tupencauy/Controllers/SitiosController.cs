using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Security.Claims;
using tupencauy.Data;
using tupencauy.Models;
using tupencauy.ViewModels;

namespace tupencauy.Controllers
{

    public class SitiosController : Controller
    {
        //Inyeccion de dependencia para tener acceso a datos de la db
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public SitiosController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        // GET: Sitios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var sitio = await _context.Sitios.FindAsync(id);

            if (sitio == null)
            {
                return NotFound();
            }

            return View(sitio);
        }

        [Authorize]
        [Authorize(Roles = "SuperAdmin")]
        // GET: Sitios/Create
        public IActionResult Create()
        {
            return View(new SitioViewModel());
        }

        // POST: Sitios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Url,TipoRegistro,ColorPrincipal,ColorSecundario,ColorTipografia")] SitioViewModel model)
        {
            var sitio = new Sitio
            {
                Nombre = model.Nombre,
                TenantId = "tenantId_" + Guid.NewGuid(),
                Url = model.Url,
                ColorPrincipal = model.ColorPrincipal,
                ColorSecundario = model.ColorSecundario,
                ColorTipografia = model.ColorTipografia,
                Status = true,
                TipoRegistro = model.TipoRegistro
            };
            _context.Add(sitio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        // GET: Sitios/RequestSite
        public IActionResult RequestSite()
        {
            var RequestSitioVM = new RequestSitioViewModel();
            return View(RequestSitioVM);
        }


        // POST: Sitios/RequestSite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestSite(RequestSitioViewModel model)
        {
            var tenantId = "tenantId_" + Guid.NewGuid();
            // Crear y guardar el sitio
            var sitio = new Sitio
            {
                Nombre = model.NombreSitio,
                TenantId = tenantId,
                Url = model.Url,
                ColorPrincipal = model.ColorPrincipal,
                ColorSecundario = model.ColorSecundario,
                ColorTipografia = model.ColorTipografia,
                Status = null,
                TipoRegistro = model.TipoRegistro,
                cantidadUsuarios = 10
            };
            _context.Add(sitio);
            await _context.SaveChangesAsync();

            var user = new AppUser
            {
                Name = model.Nombre,
                UserName = model.Username,
                Email = model.Email,
                TenantId = tenantId,
                Status = true,
                Tmstmp = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim("tenantId", tenantId));
                await _userManager.AddToRoleAsync(user, "Admin");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            else
            {
                // Si falla el registro del usuario, maneja el error adecuadamente
                ModelState.AddModelError(string.Empty, "Error al registrar el usuario.");
                return View(model);
            }
        }



        // GET: Sitios/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditSitio()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var sitio = await _context.Sitios.FirstOrDefaultAsync(s => s.TenantId == user.TenantId);

                if (sitio == null)
                {
                    return NotFound();
                }
            
            var model = new SitioViewModel
            {
                Rol = "Admin",
                Id = sitio.Id,
                Nombre = sitio.Nombre,
                Url = sitio.Url,
                ColorPrincipal = sitio.ColorPrincipal,
                ColorSecundario = sitio.ColorSecundario,
                ColorTipografia = sitio.ColorTipografia,
                TipoRegistro = sitio.TipoRegistro
            };
            return View("Edit", model); 
        }
            return NotFound();
        }

        // GET: Sitios/Edit/5
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sitio = await _context.Sitios.FindAsync(id);
            if (sitio == null)
            {
                return NotFound();
            }

            var model = new SitioViewModel
            {
                Rol = "SuperAdmin",
                Id = sitio.Id,
                Nombre = sitio.Nombre,
                Url = sitio.Url,
                ColorPrincipal = sitio.ColorPrincipal,
                ColorSecundario = sitio.ColorSecundario,
                ColorTipografia = sitio.ColorTipografia,
                TipoRegistro = sitio.TipoRegistro
            };

            return View(model);
        }

        // POST: Sitios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Url,TipoRegistro,ColorPrincipal,ColorSecundario,ColorTipografia")] SitioViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            try
            {
                var sitio = await _context.Sitios.FindAsync(id);
                sitio.Nombre = model.Nombre;
                sitio.Url = model.Url;
                sitio.ColorPrincipal = model.ColorPrincipal;
                sitio.ColorSecundario = model.ColorSecundario;
                sitio.ColorTipografia = model.ColorTipografia;
                sitio.TipoRegistro = model.TipoRegistro;
                _context.Update(sitio);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SitioExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (model.Rol == "SuperAdmin")
            {
                return RedirectToAction("Sitios", "SuperAdmin");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
                
        }

        // POST: Sitios/AprobarSitio/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprobarSitio(int id)
        {
            try
            {
                var sitio = await _context.Sitios.FindAsync(id);
                if (sitio == null)
                {
                    return NotFound();
                }

                sitio.Status = false;
                _context.Update(sitio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SuperAdminController.Sitios), "SuperAdmin");

            }
            catch (DbUpdateConcurrencyException)
            {
                // Manejo de error en caso de problemas con la concurrencia
                ModelState.AddModelError("", "No se pudo actualizar el sitio. Inténtalo de nuevo.");
                return View(await _context.Sitios.ToListAsync());
            }
        }

        // POST: Sitios/HabilitarSitio/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HabilitarSitio(int id)
        {
            try
            {
                var sitio = await _context.Sitios.FindAsync(id);
                if (sitio == null)
                {
                    return NotFound();
                }

                sitio.Status = true;
                _context.Update(sitio);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(SuperAdminController.Sitios), "SuperAdmin");
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "No se pudo habilitar el sitio. Inténtalo de nuevo.");
                return View(await _context.Sitios.ToListAsync());
            }
        }

        // POST: Sitios/DeshabilitarSitio/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeshabilitarSitio(int id)
        {
            try
            {
                var sitio = await _context.Sitios.FindAsync(id);
                if (sitio == null)
                {
                    return NotFound();
                }

                sitio.Status = false;
                _context.Update(sitio);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(SuperAdminController.Sitios), "SuperAdmin");
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "No se pudo deshabilitar el sitio. Inténtalo de nuevo.");
                return View(await _context.Sitios.ToListAsync());
            }
        }




        [Authorize(Roles = "SuperAdmin")]
        // GET: Sitios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sitio = await _context.Sitios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sitio == null)
            {
                return NotFound();
            }

            return View(sitio);
        }

        // POST: Sitios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sitio = await _context.Sitios.FindAsync(id);
            _context.Sitios.Remove(sitio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SuperAdminController.Sitios), "SuperAdmin");
        }

        private bool SitioExists(int id)
        {
            return _context.Sitios.Any(e => e.Id == id);
        }
    }
}
