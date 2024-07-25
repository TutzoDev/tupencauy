using Microsoft.AspNetCore.Mvc;
using tupencauywebapi.DTOs;

namespace tupencauywebapi.Services
{
    public interface IServicioSitio
    {
        Task<ActionResult<IEnumerable<PencaDTO>>> GetPencasByTenantId(string tenantId, string userId);
    }
}
