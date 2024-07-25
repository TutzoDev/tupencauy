using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tupencauy.Data;
using tupencauy.Models;
using tupencauywebapi.DTOs;

namespace tupencauywebapi.Services
{
    public class ServicioSitio : IServicioSitio
    {

        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ServicioSitio(IConfiguration configuration, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<PencaDTO>>> GetPencasByTenantId(string tenantId, string userId)
        {
            List<PencaSitio> pencasSitio = await _context.PencasSitio.Where(ps => ps.SitioTenantId == tenantId).ToListAsync();

            if (pencasSitio.Count() < 1)
            {
                throw new ArgumentException("Este sitio aun no contiene Pencas");
            }
            List<Penca> pencas = [];

            foreach (PencaSitio penca in pencasSitio)
            {
                var pencaAux = await _context.Pencas.FirstOrDefaultAsync(p => p.Id == penca.PencaId);
                pencas.Add(pencaAux);
            }

            if (pencas == null)
            {
                throw new ArgumentException("No se pudo crear la lista de pencas correctamente");
            }

            List<PencaDTO> pencasReturn = [];

            foreach (var p in pencas)
            {
                PencaSitio infoPencaSitio = await _context.PencasSitio.FirstOrDefaultAsync(ps => ps.SitioTenantId == tenantId && ps.PencaId == p.Id);

                PencaSitioUsuario psu = await _context.PencaSitioUsuario.FirstOrDefaultAsync(psu=>psu.IdPencaSitio==infoPencaSitio.Id && psu.IdUsuario==userId);

                bool userInscrito = false;

                if(psu!=null)
                {
                    userInscrito = true;
                }

                PencaDTO pencaDTOFetch = new()
                {
                    Id = p.Id,
                    Nombre = infoPencaSitio.Nombre,
                    CostoPenca = infoPencaSitio.Costo, 
                    FechaInicio = p.FechaInicio,
                    FechaFin = p.FechaFin,
                    Inscrito = userInscrito
                };

                pencasReturn.Add(pencaDTOFetch);
            }
            return pencasReturn;
        }
    }
}
