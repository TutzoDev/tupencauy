using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tupencauy.Data;
using tupencauy.Models;
using tupencauy.ViewModels;

namespace tupencauy.Controllers
{
    public class EventoDeportivoController : Controller
    {
        private readonly AppDbContext _context;

        public EventoDeportivoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: EventoDeportivo
        public async Task<IActionResult> Index()
        {
            var eventos = await _context.EventosDeportivos
                .Select(e => new EventoDeportivoVM
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    FechaInicio = e.FechaInicio,
                }).ToListAsync();

            return View(eventos);
        }


        // GET: EventoDeportivo/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoDeportivo = await _context.EventosDeportivos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventoDeportivo == null)
            {
                return NotFound();
            }

            return View(eventoDeportivo);
        }

        // GET: EventoDeportivo/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventoDeportivoVM eventoDeportivo)
        {
            if (eventoDeportivo.TipoDeEvento.ToString() == "UnoVsUno")
            {
                var eventoUnoVsUno = new UnoVsUno
                {
                    Id = Guid.NewGuid().ToString(),
                    Nombre = eventoDeportivo.Nombre,
                    EquipoUno = eventoDeportivo.EquipoUno,
                    EquipoDos = eventoDeportivo.EquipoDos,
                    Deporte = eventoDeportivo.TipoDeDeporte.ToString(),
                    FechaInicio = eventoDeportivo.FechaInicio
                };
                _context.Add(eventoUnoVsUno);
                await _context.SaveChangesAsync();
                return RedirectToAction("EventosDeportivos", "SuperAdmin");
            }
            return View(eventoDeportivo);
        }

        // GET: EventoDeportivo/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoDeportivo = await _context.EventosDeportivos.FindAsync(id);
            if (eventoDeportivo == null)
            {
                return NotFound();
            }

            var eventoVM = new EventoDeportivoVM
            {
                Id = eventoDeportivo.Id,
                Nombre = eventoDeportivo.Nombre,
                FechaInicio = eventoDeportivo.FechaInicio
            };

            return View(eventoVM);
        }

        // POST: EventoDeportivo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Nombre,FechaInicio")] EventoDeportivoVM eventoDeportivo)
        {
            if (eventoDeportivo.Id == null)
            {
                return NotFound();
            }

            EventoDeportivo eventoDB = await _context.EventosDeportivos.FindAsync(eventoDeportivo.Id);

            eventoDB.Nombre = eventoDeportivo.Nombre;
            eventoDB.FechaInicio = eventoDeportivo.FechaInicio;

            try
            {
                _context.Update(eventoDB);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoDeportivoExists(eventoDeportivo.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("EventosDeportivos", "SuperAdmin");
        }

        // GET: EventoDeportivo/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventoDeportivo = await _context.EventosDeportivos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventoDeportivo == null)
            {
                return NotFound();
            }

            return View(eventoDeportivo);
        }

        // POST: EventoDeportivo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var eventoDeportivo = await _context.EventosDeportivos.FindAsync(id);
            if (eventoDeportivo != null)
            {
                _context.EventosDeportivos.Remove(eventoDeportivo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("EventosDeportivos", "SuperAdmin");
        }

        private bool EventoDeportivoExists(string id)
        {
            return _context.EventosDeportivos.Any(e => e.Id == id);
        }
    }
}
