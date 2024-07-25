using tupencauywebapi.DTOs;
using tupencauywebapi.Models;

namespace tupencauywebapi.Services
{
    public interface IServicioPrediccion
    {
        Task<PrediccionDTO> GetPrediccionUser(PrediccionReq prediccion);
    }
}
