using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tupencauywebapi.Models;
using tupencauywebapi.Services;

namespace tupencauywebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionesPushController : ControllerBase
    {
        private readonly IServicioNotificacionPush _notificacionService;

        public NotificacionesPushController(IServicioNotificacionPush notificacionService)
        {

            _notificacionService = notificacionService;
        }

        [HttpPost("EnviarNotificacion")]
        public async Task<IActionResult> EnviarNotificacion([FromBody] NotificacionPush request)
        {
            await _notificacionService.EnviarNotificacionPush(request.userId, request.titulo, request.body);
            return Ok();
        }

        [HttpPost("GuardarFcmToken")]
        public async Task<IActionResult> GuardarTokenCelularFirebase([FromBody] FirebaseTokenReq userFcmToken)
        {
            if (userFcmToken == null)
            {
                return BadRequest();
            }

            var result = await _notificacionService.GuardarTokenFcm(userFcmToken);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result);
        }

        [HttpGet("{userId}/ConfigurarNotificaciones")]
        public async Task<IActionResult> ActivarDesactivarNotificaciones(string userId, bool habilitadas)
        {
            await _notificacionService.ActivarDesactivarNotificaciones(userId, habilitadas);
            return Ok("Configuracion de notificaciones modificada correctamente");
        }
    }
}
