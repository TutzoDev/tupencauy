using tupencauywebapi.DTOs;
using tupencauywebapi.Models;

namespace tupencauywebapi.Services
{
    public interface IServicioPenca
    {
        Task<List<UnoVsUnoDTO>> GetEventosUnoVsUnoByPencaId(string id);
    }
}
