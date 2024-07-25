using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tupencauy.Data;
using tupencauywebapi.DTOs;
using tupencauywebapi.Services;

namespace tupencauywebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SitiosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IServicioSitio _servicioSitio;

        public SitiosController(AppDbContext context, IServicioSitio servicioSitio)
        {
            _context = context;
            _servicioSitio = servicioSitio;
        }

        // GET: api/Sitios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SitioDTO>>> GetSitios()
        {
            var sitios = await _context.Sitios.ToListAsync();
            if(sitios == null)
            {
                return NotFound();
            }
            return Ok(sitios);
        }

        // GET: api/Sitios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SitioDTO>> GetSitio(string id)
        {
            var sitio = await _context.Sitios.FirstOrDefaultAsync(s=>s.TenantId==id);

            if (sitio == null)
            {
                return NotFound();
            }

            SitioDTO sitioRet = new SitioDTO
            {
                Nombre = sitio.Nombre,
                Url = sitio.Url,
                ColorPrincipal = sitio.ColorPrincipal,
                ColorSecundario = sitio.ColorSecundario,
                ColorTipografia = sitio.ColorTipografia
            };

            return sitioRet;
        }

        [HttpGet("{tenantId}/Users")]
        public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetUsersByTenantId(string tenantId)
        {
            var users = await _context.Usuarios.Where(u => u.TenantId == tenantId).ToListAsync();
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }

        [HttpGet("{tenantId}/Pencas")]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<PencaDTO>>> GetPencasByTenantId(string tenantId, string idUser)
        {
            var pencas = await _servicioSitio.GetPencasByTenantId(tenantId, idUser);

            if (pencas == null)
            {
                return NotFound();
            }

            return pencas; //aca por alguna razon si retorno Ok(pencas) no me llegan las pencas en MAUI
        }

    }
}
