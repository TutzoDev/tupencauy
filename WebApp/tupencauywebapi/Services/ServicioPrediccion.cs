using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tupencauy.Data;
using tupencauy.Models;
using tupencauywebapi.DTOs;
using tupencauywebapi.Models;

namespace tupencauywebapi.Services
{
    public class ServicioPrediccion : IServicioPrediccion
    {
        readonly AppDbContext _context;
        public ServicioPrediccion(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PrediccionDTO> GetPrediccionUser(PrediccionReq prediccion)
        {
            var pencaSitio = await _context.PencasSitio.FirstOrDefaultAsync(ps => ps.PencaId == prediccion.pencaId && ps.SitioTenantId == prediccion.tenantId);

            PencaSitioUsuario pencaSitioUsuario = await _context.PencaSitioUsuario.FirstOrDefaultAsync(psu => psu.IdPencaSitio == pencaSitio.Id && psu.IdUsuario == prediccion.userId);

            UnoVsUno eventoPrediccion = await _context.UnoVsUno.FirstOrDefaultAsync(uvu => uvu.Id == prediccion.IdEvento);


            //Se verifica si ya hay una prediccion hecha de este evento por el usuario
            if (_context.Prediccion.FirstOrDefault(pr => pr.IdPencaSitioUsuario == pencaSitioUsuario.Id && pr.IdUnoVsUno==prediccion.IdEvento) != null)
            {
                Prediccion prediccionUser = await _context.Prediccion.FirstOrDefaultAsync(pr => pr.IdUnoVsUno == prediccion.IdEvento && pr.IdPencaSitioUsuario == pencaSitioUsuario.Id);

                PrediccionDTO prediccionRet = new PrediccionDTO
                {
                    //IdPencaSitio = pencaSitio.Id,
                    IdPencaSitioUsuario = prediccionUser.IdPencaSitioUsuario,
                    IdEvento = eventoPrediccion.Id,
                    EquipoUno = eventoPrediccion.EquipoUno,
                    EquipoDos = eventoPrediccion.EquipoDos,
                    ScoreTeamUno = prediccionUser.ScoreTeam1,
                    ScoreTeamDos = prediccionUser.ScoreTeam2,
                    Realizada = true
                  };
                return prediccionRet;
            }

            PrediccionDTO eventoSinPredecirRet = new PrediccionDTO
            {
                //IdPencaSitio = pencaSitio.Id,
                IdPencaSitioUsuario = pencaSitioUsuario.Id,
                IdEvento = eventoPrediccion.Id,
                EquipoUno = eventoPrediccion.EquipoUno,
                EquipoDos = eventoPrediccion.EquipoDos,
                Realizada = false
            };
            return eventoSinPredecirRet;

        }
    }
}
