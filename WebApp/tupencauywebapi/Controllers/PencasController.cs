using Microsoft.AspNetCore.Mvc;
using tupencauywebapi.DTOs;
using tupencauy.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using tupencauywebapi.Services;
using tupencauywebapi.Models;
using tupencauy.Models;

namespace tupencauywebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PencasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IServicioPenca _servicioPenca;

        public PencasController(AppDbContext context, IServicioPenca servicioPenca)
        {
            _context = context;
            _servicioPenca = servicioPenca;
        }


        // GET: api/PencasApi/
        [HttpGet(Name ="GetPencas")]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<PencaDTO>>> GetPencas()
        {
            var pencasRet = await _context.Pencas.ToListAsync();
            if (pencasRet == null)
            {
                return NotFound();
            }
            return Ok(pencasRet);
        }

        //// GET: api/PencasApi/{id}
        [HttpGet("{id}", Name = "GetInfoPenca")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public async Task<ActionResult<PencaDTO>> GetInfoPenca(string id)
        {
            if (id == null || id == "")
            {
                return BadRequest();
            }
            var penca = await _context.Pencas.FindAsync(id);
            if (penca == null)
            {
                return NotFound();
            }

            return Ok(penca);
        }

        //// GET: api/PencasApi/{id}/GetUnoVsUnoPenca
        [HttpGet("{id}/GetEventosUnoVsUno")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<UnoVsUnoDTO>>> GetEventosUnoVsUno(string id)
        {
            var eventos = await _servicioPenca.GetEventosUnoVsUnoByPencaId(id);

            if (eventos == null || !eventos.Any())
            {
                return NotFound();
            }

            return Ok(eventos);
        }

        [HttpPost("Inscribirse")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> InscripcionPenca(InscripcionPencaReq inscripcion)
        {
            // Verificar la existencia de la penca en el sitio
            var pencaSitio = _context.PencasSitio.FirstOrDefault(ps => ps.PencaId == inscripcion.IdPenca && ps.SitioTenantId == inscripcion.TenantId);
            if (pencaSitio == null)
            {
                return NotFound("Penca no encontrada en el sitio.");
            }

            // Verificar la existencia del usuario
            if (inscripcion.IdUsuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            AppUser userAInscribir = _context.Usuarios.FirstOrDefault(u => u.Id == inscripcion.IdUsuario);

            if (userAInscribir.Saldo < pencaSitio.Costo)
            {
                return BadRequest("No tienes saldo suficiente para inscribirte en esta penca");
            }

            var userPencaSitio = _context.PencaSitioUsuario.FirstOrDefault(psu => psu.IdUsuario == inscripcion.IdUsuario && psu.IdPencaSitio == pencaSitio.Id);
            if (userPencaSitio != null)
            {
                return BadRequest("El usuario ya está inscrito en la penca.");
            }

            var plataforma = await _context.Sistema.FirstOrDefaultAsync();

            var dineroPlataforma = (pencaSitio.Comision);

            pencaSitio.Recaudacion += (pencaSitio.Costo) - dineroPlataforma;
            plataforma.Billetera += dineroPlataforma;
            userAInscribir.Saldo -= pencaSitio.Costo;

            var nuevaInscripcion = new PencaSitioUsuario
            {
                IdPencaSitio = pencaSitio.Id,
                IdUsuario = userAInscribir.Id,
                NombreUsuario = userAInscribir.UserName,
                Puntaje = 0,
                Aciertos = 0,
                Predicciones = new List<Prediccion>()
            };

            _context.PencaSitioUsuario.Add(nuevaInscripcion);
            pencaSitio.Inscriptos++;
            _context.Update(pencaSitio);
            _context.Update(userAInscribir);
            _context.Update(plataforma);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Has sido inscrito a la penca, recuerda que tienes 3 días para pagar la inscripición, si excedes el plazo se te dará de baja de la penca." });

        }

        [HttpGet("Sitio/{tenantId}/Penca/{pencaId}/GetUsuarios")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObtenerUsuariosPenca([FromRoute]string pencaId, string tenantId)
        {
            // Verificar la existencia de la penca en el sitio
            var pencaSitio = await _context.PencasSitio.FirstOrDefaultAsync(ps => ps.PencaId == pencaId && ps.SitioTenantId == tenantId);
            var UsersReturn = await _context.PencaSitioUsuario.Where(psu => psu.IdPencaSitio == pencaSitio.Id).ToListAsync();

            if (UsersReturn == null || UsersReturn.Count == 0)
            {
                return NotFound("No se encontraron usuarios inscritos en esta penca.");
            }
            var posicionesDTO = new List<PosicionDTO>();
            foreach(var user in UsersReturn)
            {
                PosicionDTO userFetch = new PosicionDTO
                {
                    NombreUsuario = user.NombreUsuario,
                    Puntaje = user.Puntaje,
                    Aciertos = user.Aciertos
                };
                posicionesDTO.Add(userFetch);
            }
            
            return Ok(posicionesDTO);
        }
    }
}
