using tupencauywebapi.DTOs;
using tupencauywebapi.Models;

namespace tupencauywebapi.Services
{
    public interface IServicioUser
    {
        Task<AppUserDTO> GetUsuario(string id);
        Task<AuthReturn> EditarUsuario(EditUserReq user);
    }
}
