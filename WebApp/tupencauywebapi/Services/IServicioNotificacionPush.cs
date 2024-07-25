using tupencauy.Models;
using tupencauywebapi.Models;

namespace tupencauywebapi.Services
{
    public interface IServicioNotificacionPush
    {
        Task EnviarNotificacionPush(string idUserToNotify, string title, string body);
        Task<AuthReturn> GuardarTokenFcm(FirebaseTokenReq userFcmToken);
        Task ActivarDesactivarNotificaciones(string idUser, bool habilitadas);
    }
}
