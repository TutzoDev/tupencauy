using tupencauywebapi.Models;

namespace tupencauywebapi.Services
{
    public interface IServicioAuth
    {
        Task<AuthReturn> RegistrarseAsync(RegistrarseReq formRegistro);
        Task<AuthReturn> LoginAsync(LoginReq request);
        Task<bool> ValidarUserAsync(string email, string password);
        Task<AuthReturn> GoogleLoginAsync(string returnUrl = null, string externalError = null, string tenantId = null);
        Task<AuthReturn> MauiGoogleLoginAsync(GoogleLoginReq googleUserMaui);
    }
}
