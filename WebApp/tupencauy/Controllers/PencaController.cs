using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using tupencauy.Data;
using tupencauy.Models;
using tupencauy.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static tupencauy.Controllers.PencaController;
using static tupencauy.ViewModels.EventoDeportivoVM;

namespace tupencauy.Controllers
{
    public class PencaController : Controller
    {
        private readonly AppDbContext _context;

        public PencaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Penca
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pencas.ToListAsync());
        }

        // GET: Penca/Create
        public IActionResult Create()
        {
            var pencaVM = new PencaEventoVM
            {
                Eventos = _context.EventosDeportivos
                    .OfType<UnoVsUno>() // Filtra solo los eventos de tipo UnoVsUno
                    .Select(e => new Evento
                    {
                        Id = e.Id.ToString(),
                        Nombre = e.Nombre,
                        EquipoUno = e.EquipoUno,
                        EquipoDos = e.EquipoDos,
                        Fecha = e.FechaInicio
                    })
                    .ToList(),
            };


            return View(pencaVM);
        }


        [HttpPost]
        public async Task<IActionResult> Create(PencaEventoVM pencaEventoVM)
        {
            var penca = new Penca
            {
                Id = Guid.NewGuid().ToString(),
                Nombre = pencaEventoVM.Nombre,
                FechaInicio = DateTime.Now,
                EventosDeportivos = new List<EventoDeportivo>()
            };

            var eventos = pencaEventoVM.SelectedEventsId.Split(',');
            // Procesar los IDs de los eventos seleccionados
           
            foreach (var idEvento in eventos)
            {
                var evento = await _context.EventosDeportivos.FindAsync(idEvento);
                if (evento != null)
                {
                    penca.EventosDeportivos.Add(evento);
                }

                // Imprimir el tipo de datos de idEvento y su valor
                Console.WriteLine($"Evento ID: {idEvento}, Tipo de datos: {idEvento.GetType()}");
            }
            var eventoMasReciente = penca.EventosDeportivos
                                          .Where(e => e.FechaInicio >= DateTime.Today)
                                          .OrderByDescending(e => e.FechaInicio)
                                          .FirstOrDefault();
            if (eventoMasReciente != null)
            {
                penca.FechaInicio = eventoMasReciente.FechaInicio;
            }


            _context.Add(penca);
            await _context.SaveChangesAsync();

            return RedirectToAction("Pencas", "SuperAdmin");
        }
    }
}
