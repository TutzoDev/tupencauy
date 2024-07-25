using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using tupencauywebapi.Models;
using tupencauywebapi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using tupencauy.Models;

namespace tupencauy.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IServicioAuth _servicioAuth;
        private readonly IConfiguration _config;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(IServicioAuth servicioAuth, IConfiguration config, SignInManager<AppUser> signInManager)
        {
            _servicioAuth = servicioAuth;
            _config = config;
            _signInManager = signInManager;
        }


        [HttpPost("registrarse")]
        public async Task<IActionResult> RegistarUsuario([FromBody] RegistrarseReq request)
        {
            var result = await _servicioAuth.RegistrarseAsync(request);
            if (!result.Success)
                return BadRequest();

            return Ok(result);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginReq request)
        {
            var result = await _servicioAuth.LoginAsync(request);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }


        [HttpPost("google-login")]
        public IActionResult GoogleLogin(string tenantId, string returnUrl = "/")
        {
            var redirigir = Url.Action("GoogleResponse", "Auth", new { ReturnUrl = returnUrl/*, TenantId = tenantId*/ });
            var props = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirigir);

            return new ChallengeResult(GoogleDefaults.AuthenticationScheme, props);
        }


        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = null, string externalError = null, string tenantId = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var authenticateResult = await _servicioAuth.GoogleLoginAsync(returnUrl, externalError, tenantId);

            if (!authenticateResult.Success)
            {
                return BadRequest(authenticateResult.Message);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);
            
            var tokenRet = new JwtSecurityTokenHandler().WriteToken(token);


            return Ok(new { Token = tokenRet });
        }

        [HttpPost("google-login-maui")]
        public async Task<IActionResult> GuardarGoogleUserMaui(GoogleLoginReq userGoogle)
        {
            var guardarUser = await _servicioAuth.MauiGoogleLoginAsync(userGoogle);

            if(!guardarUser.Success)
            {
                return BadRequest(guardarUser.Message);
            }

            return Ok(guardarUser);
        }
    }
}