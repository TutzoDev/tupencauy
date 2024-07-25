using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tupencauy.Data;
using tupencauy.Models;
using tupencauywebapi.DTOs;
using tupencauywebapi.Models;
using Newtonsoft.Json;
using tupencauywebapi.Services;

namespace tupencauywebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosDeportivosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IServicioPrediccion _servicioPrediccion;
        private readonly IHttpClientFactory _httpClientFactory;

        public EventosDeportivosController(AppDbContext context, IServicioPrediccion servicioPrediccion, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _servicioPrediccion = servicioPrediccion;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoDeportivo>>> GetEventosDeportivos()
        {
            return await _context.EventosDeportivos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventoDeportivo>> GetEventoDeportivo(string id)
        {
            var eventoDeportivo = await _context.EventosDeportivos.FindAsync(id);

            if (eventoDeportivo == null)
            {
                return NotFound();
            }

            return eventoDeportivo;
        }

        [HttpGet("GetAllUnoVsUno")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<UnoVsUnoDTO>>> GetAllUnoVsUno()
        {
            var eventos = await _context.UnoVsUno.ToListAsync();

            if (eventos == null || !eventos.Any())
            {
                return NotFound();
            }

            return Ok(eventos);
        }

        [HttpPost("Predecir")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PredecirEventoPencaSitio([FromBody]PrediccionDTO prediccion)
        {
            if(prediccion == null)
            {
                return BadRequest();
            }

            if (prediccion.ScoreTeamUno == null || prediccion.ScoreTeamDos == null)
            {
                return BadRequest("Debes ingresar los goles");
            }

            var prediccionRepetida = await _context.Prediccion.FirstOrDefaultAsync(pr => pr.IdPencaSitioUsuario == prediccion.IdPencaSitioUsuario && pr.IdUnoVsUno == prediccion.IdEvento);

            if (prediccionRepetida!=null)
            {
                return BadRequest("Esta prediccion ya fue realizada");
            }

            var prediccionFetch = new Prediccion
            {
                IdPencaSitioUsuario = prediccion.IdPencaSitioUsuario,
                IdUnoVsUno = prediccion.IdEvento,
                ScoreTeam1 = prediccion.ScoreTeamUno.Value,
                ScoreTeam2 = prediccion.ScoreTeamDos.Value
            };
            _context.Prediccion.Add(prediccionFetch);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("GetPrediccionesPencaUser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPrediccionesPencaUser(string pencaSitioUserId)
        {
            var predicciones = await _context.Prediccion.Where(pr => pr.IdPencaSitioUsuario== pencaSitioUserId).ToListAsync();

            if (predicciones == null)
            {
                return NotFound();
            }
            return Ok(predicciones);
        }

        [HttpPost("GetPrediccionUser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        //En esta funcion fue necesario buscar traer todos los IDs necesarios por separado, pero se retorna un IDPencaSitioUsuario para no necesitar hacerlo en los demás endpoints
        public async Task<ActionResult<PrediccionDTO>> GetPrediccion([FromBody] PrediccionReq prediccion)
        {
            if (prediccion == null)
            {
                return BadRequest();
            }

            var prediccionreturn = await _servicioPrediccion.GetPrediccionUser(prediccion);

            if (prediccionreturn == null)
            {
                return NotFound();
            }
            return Ok(prediccionreturn);
        }

        [HttpPost("EditarPrediccion")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EditarPrediccion([FromBody]PrediccionDTO prediccion)
        {
            if (prediccion == null)
            {
                return BadRequest();
            }

            var prediccionUpdate = await _context.Prediccion.FirstOrDefaultAsync(pr => pr.IdUnoVsUno == prediccion.IdEvento && pr.IdPencaSitioUsuario == prediccion.IdPencaSitioUsuario);

            if (prediccion.ScoreTeamUno == null || prediccion.ScoreTeamDos == null)
            {
                return BadRequest("Los goles no pueden ser nulos");
            }

            prediccionUpdate.ScoreTeam1 = prediccion.ScoreTeamUno.Value;
            prediccionUpdate.ScoreTeam2 = prediccion.ScoreTeamDos.Value;

            _context.Prediccion.Update(prediccionUpdate);
            await _context.SaveChangesAsync();

            return Ok(prediccionUpdate);
        }


        [HttpGet("CargarEventos")]
        public async Task<ActionResult<IEnumerable<EventoDeportivo>>> CargarEventosDeportivos([FromQuery] string fecha)
        {
            // Validar la fecha
            if (!DateTime.TryParseExact(fecha, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime fechaParsed))
            {
                return BadRequest("Formato de fecha incorrecto. Debe ser YYYY-MM-DD.");
            }

            var requestUrl = $"https://futboltimeapi.p.rapidapi.com/v1/partidos-por-fecha?fecha={fecha}";

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("x-rapidapi-host", "futboltimeapi.p.rapidapi.com");
            request.Headers.Add("x-rapidapi-key", "1d3eddc9admsh930e0254e3f3a4bp1b52d4jsn2f63cecf1d69");

            try
            {
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error al llamar a la API externa");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var partidosAPI = JsonConvert.DeserializeObject<List<PartidoAPI>>(responseString);
                if (partidosAPI != null)
                {
                    // Procesa los datos de la API externa y mapea a tu modelo EventoDeportivo
                    var eventosDeportivos = new List<UnoVsUno>();
                    foreach (var partidoAPI in partidosAPI)
                    {
                        var evento = new UnoVsUno
                        {
                            Id = Guid.NewGuid().ToString(),
                            Nombre = partidoAPI.Campeonato,
                            EquipoUno = partidoAPI.Equipo1,
                            EquipoDos = partidoAPI.Equipo2,
                            HoraPartido = partidoAPI.Hora,
                            FechaInicio = fechaParsed, // Usa la fecha parseada
                            ScoreUno = "0", // Inicialmente en 0, ajusta según tu lógica
                            ScoreDos = "0", // Inicialmente en 0, ajusta según tu lógica
                            Pencas = new List<Penca>() // Inicializa con una lista vacía, ajusta según tu lógica
                        };

                        eventosDeportivos.Add(evento);
                    }

                    // Guarda los nuevos eventos deportivos en la base de datos si es necesario
                    _context.EventosDeportivos.AddRange(eventosDeportivos);
                    await _context.SaveChangesAsync();
                    return Ok(eventosDeportivos);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar la solicitud: {ex.Message}");
            }
        }

        [HttpGet("ObtenerResultados")]
        public async Task<IActionResult> ObtenerResultados()
        {
            // Asumimos que tienes una entidad llamada "Evento" en tu DbContext
            var eventos = await _context.UnoVsUno.ToListAsync();
            return Ok(eventos); // Devuelve los eventos como JSON
        }

        [HttpPost("ActualizarResultados")]
        public async Task<IActionResult> ActualizarResultados([FromBody] ActualizarResultadosRequest request)
        {
            if (request == null || request.Partidos == null || !request.Partidos.Any())
            {
                return BadRequest("Lista de partidos no válida.");
            }

            foreach (var partido in request.Partidos)
            {
                if (string.IsNullOrEmpty(partido.Id) || string.IsNullOrEmpty(partido.ScoreUno) || string.IsNullOrEmpty(partido.ScoreDos))
                {
                    return BadRequest("Datos de partido no válidos.");
                }

                var evento = await _context.UnoVsUno.FindAsync(partido.Id);
                if (evento != null)
                {
                    // Actualizo evento deportivo
                    evento.ScoreUno = partido.ScoreUno;
                    evento.ScoreDos = partido.ScoreDos;
                    evento.FechaFin = DateTime.UtcNow; // Asigna la hora actual en UTC

                    // Obtengo todas las predicciones que tienen ese id
                    var predicciones = _context.Prediccion.Where(p => p.IdUnoVsUno == partido.Id).ToList();

                    foreach (var prediccion in predicciones)
                    {
                        // Obtengo la relación PencaSitioUsuario
                        var pencaSitioUsuario = await _context.PencaSitioUsuario.FindAsync(prediccion.IdPencaSitioUsuario);
                        if (pencaSitioUsuario != null)
                        {
                            // Lógica para actualizar puntaje y aciertos
                            int puntos = 0;
                            bool aciertoExacto = prediccion.ScoreTeam1 == int.Parse(evento.ScoreUno) && prediccion.ScoreTeam2 == int.Parse(evento.ScoreDos);
                            bool aciertoDiferencia = (prediccion.ScoreTeam1 - prediccion.ScoreTeam2) == (int.Parse(evento.ScoreUno) - int.Parse(evento.ScoreDos));
                            bool aciertoGanador = (prediccion.ScoreTeam1 > prediccion.ScoreTeam2) == (int.Parse(evento.ScoreUno) > int.Parse(evento.ScoreDos));

                            if (aciertoExacto)
                            {
                                puntos = 8;
                                pencaSitioUsuario.Aciertos += 1;
                            }
                            else if (aciertoDiferencia)
                            {
                                puntos = 5;
                            }
                            else if (aciertoGanador)
                            {
                                puntos = 3;
                            }

                            pencaSitioUsuario.Puntaje += puntos;
                        }
                    }
                }
                else
                {
                    return NotFound($"Evento deportivo con ID {partido.Id} no encontrado.");
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Resultados y predicciones actualizados correctamente.");
        }
    }
}

