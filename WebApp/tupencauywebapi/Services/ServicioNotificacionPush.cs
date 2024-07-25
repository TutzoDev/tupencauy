using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using tupencauy.Data;
using Microsoft.AspNetCore.Identity;
using tupencauywebapi.Models;
using tupencauy.Models;
using Microsoft.EntityFrameworkCore;

namespace tupencauywebapi.Services
{
    public class ServicioNotificacionPush : IServicioNotificacionPush
    {
        private readonly FirebaseApp _firebaseApp;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ServicioNotificacionPush(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;

            if(FirebaseApp.DefaultInstance == null ) {
                _firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("Firebase/admin_sdk_firebase.json")
                });
            }            
        }

        public async Task<AuthReturn> GuardarTokenFcm(FirebaseTokenReq userFcmToken)
        {
            var user = await _userManager.FindByIdAsync(userFcmToken.IdUser);

            if (user == null)
            {
                return new AuthReturn { Success = false, Message = "No se encontró el usuario" };
            }

            var existeUserToken = await _userManager.GetAuthenticationTokenAsync(user, "FirebaseCloudMessaging", "Token");

            if (existeUserToken == null)
            {
                var guardarTokenCelNuevo = await _userManager.SetAuthenticationTokenAsync(user, "FirebaseCloudMessaging", "Token", userFcmToken.fcmToken);

                if (!guardarTokenCelNuevo.Succeeded)
                {
                    return new AuthReturn { Success = false, Message = "No se pudo guardar el fcm token" };
                }
            }
            else
            {
                if (existeUserToken == userFcmToken.fcmToken)
                {
                    return new AuthReturn { Success = true, Message = "El token no ha cambiado, no es necesario tomar acciones" };
                }

                var updateToken = await _userManager.SetAuthenticationTokenAsync(user, "FirebaseCloudMessaging", "Token", userFcmToken.fcmToken);

                if (!updateToken.Succeeded)
                {
                    return new AuthReturn { Success = false, Message = "No se pudo actualizar el fcm token" };
                }
            }
            return new AuthReturn { Success = true, Message = "Se ha actualizado el token correctamente" };
        }

        public async Task EnviarNotificacionPush(string idUserToNotify, string title, string body)
        {
            var notificacionesActivadas = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == idUserToNotify);

            if(notificacionesActivadas.RecibirNotificaciones == true)
            {
                var message = new Message()
                {
                    Token = await GetToken(idUserToNotify),
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body
                    }
                };
                await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
        }

        private async Task<string> GetToken(string idUser)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == idUser);

            string phoneToken = await _userManager.GetAuthenticationTokenAsync(user, "FirebaseCloudMessaging", "Token");

            return phoneToken;
        }

        public async Task ActivarDesactivarNotificaciones(string idUser, bool habilitadas)
        {
            var notificacionesActivadas = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == idUser);

            notificacionesActivadas.RecibirNotificaciones = habilitadas;

            _context.Usuarios.Update(notificacionesActivadas);
            await _context.SaveChangesAsync();
        }
    }
}
