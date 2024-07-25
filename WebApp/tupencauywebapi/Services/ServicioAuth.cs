using tupencauywebapi.Models;
using tupencauy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using tupencauy.Data;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin.Auth.Multitenancy;

namespace tupencauywebapi.Services
{
    public class ServicioAuth : IServicioAuth
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ServicioAuth(IConfiguration configuration, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<bool> ValidarUserAsync(string emailUsername, string password)
        {
            AppUser user;

            user = await _userManager.FindByNameAsync(emailUsername);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(emailUsername);
            }

            if (user == null)
            {
                return false;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            return result.Succeeded;
        }

        public async Task<AuthReturn> RegistrarseAsync(RegistrarseReq formRegistro)
        {
            if(formRegistro.TenantId == null)
            {
                return new AuthReturn { Success = false, Message = "Algo ha salido mal!" };
            }

            if (formRegistro.Nombre != "" && formRegistro.Username != "" && formRegistro.Email.Contains('@') && formRegistro.Password != "")
            {
                var usernameRepetido = await _userManager.FindByNameAsync(formRegistro.Username);
                if (usernameRepetido != null)
                {
                    return new AuthReturn { Success = false, Message = "Ese username ya está en uso" };
                }

                var tiporesgistroSitio = await _context.Sitios.FirstOrDefaultAsync(s => s.TenantId  == formRegistro.TenantId);

                if(tiporesgistroSitio.TipoRegistro == "Abierto")
                {
                    var user = new AppUser
                    {
                        Name = formRegistro.Nombre,
                        UserName = formRegistro.Username,
                        Email = formRegistro.Email,
                        TenantId = formRegistro.TenantId,
                        Status = true,
                        Tmstmp = DateTime.Now
                    };
                    var result = await _userManager.CreateAsync(user, formRegistro.Password!);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Usuario");
                        return new AuthReturn { Success = true, Message = "Te has registrado exitosamente!" };
                    }
                }
                else if(tiporesgistroSitio.TipoRegistro == "Con aprobacion")
                {
                    var user = new AppUser
                    {
                        Name = formRegistro.Nombre,
                        UserName = formRegistro.Username,
                        Email = formRegistro.Email,
                        TenantId = formRegistro.TenantId,
                        Status = false,
                        Tmstmp = DateTime.Now
                    };
                    var result = await _userManager.CreateAsync(user, formRegistro.Password!);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Usuario");

                        return new AuthReturn { Success = true, Message = "Te has registrado exitosamente! Debes esperar a que aprueben tu usuario para poder iniciar sesión con tu usuario." };
                    }
                }
                else if(tiporesgistroSitio.TipoRegistro == "Invitacion Link")
                {
                    return new AuthReturn { Success = false, Message = "Solo puedes ingresar a este sitio mediante un link de  invitación." };
                }
                else if (tiporesgistroSitio.TipoRegistro == "Cerrado")
                {
                    return new AuthReturn { Success = false, Message = "Este sitio no tiene las inscripciones abiertas." };
                }
            }
            return new AuthReturn { Success = false, Message = "Alguno de los datos ingresados es incorrecto, vuelve a intentarlo." };
        }

        public async Task<AuthReturn> LoginAsync(LoginReq request)
        {
            var user = await _userManager.FindByNameAsync(request.EmailUsername);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(request.EmailUsername);
            }

            var userValido = await ValidarUserAsync(request.EmailUsername, request.Password);

            var rolUser = await _userManager.GetRolesAsync(user);

            if (userValido && !rolUser.Contains("SuperAdmin") && !rolUser.Contains("Admin"))
            {
                if (user.TenantId != request.tenantId)
                {
                    return new AuthReturn { Success = false, Message = "Este usuario no pertenece a este Sitio" };
                }
                else if(user.Status == false)
                {
                    return new AuthReturn { Success = false, Message = "El administrador del sitio aún no ha habilitado tu usuario" };
                }

                var token = GenerateJwtToken(user);

                return new AuthReturn { Success = true, Token = token, TenantId = user.TenantId, IdUser = user.Id };
            }

            return new AuthReturn { Success = false, Message = "El usuario y/o la contraseña son incorrectos" };
        }


        public async Task<AuthReturn> GoogleLoginAsync(string returnUrl = null, string externalError = null, string tenantId = null)
        {
            if (externalError != null)
            {
                return new AuthReturn { Success = false, Message = $"Ha habido un error con el proveedor externo : {externalError}" };

            }

            var infoGoogleUser = await _signInManager.GetExternalLoginInfoAsync();

            if (infoGoogleUser == null)
            {
                return new AuthReturn { Success = false, Message ="Hemos tenido un eror al cargar la iformacion de tu cuenta de Google" };

            }

            var resultadoLogin = await _signInManager.ExternalLoginSignInAsync(infoGoogleUser.LoginProvider,
                infoGoogleUser.ProviderKey, isPersistent: false, bypassTwoFactor: false);

            //User tiene cuenta interna, y entra con la de Google
            if (resultadoLogin.Succeeded)
            {
                return new AuthReturn {Success=true, Message="Login exitoso. Bienvenido!"};
            }
            else
            { 
                var email = infoGoogleUser.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);

                    var tiporesgistroSitio = await _context.Sitios.FirstOrDefaultAsync(s => s.TenantId == tenantId);


                    //User no tiene cuenta interna
                    if (user == null && tiporesgistroSitio.TipoRegistro != "Cerrado" && tiporesgistroSitio.TipoRegistro != "Invitacion Link")
                    {
                        if (tiporesgistroSitio.TipoRegistro == "Abierto")
                        {
                            //Creo nuevo user sin contraseña
                            user = new AppUser
                            {
                                Name = infoGoogleUser.Principal.FindFirstValue(ClaimTypes.Name).Replace(" ", ""),
                                UserName = infoGoogleUser.Principal.FindFirstValue(ClaimTypes.Name).Replace(" ", ""),
                                Email = infoGoogleUser.Principal.FindFirstValue(ClaimTypes.Email),
                                Tmstmp = DateTime.Now,
                                TenantId = tenantId,
                                Status = true
                            };

                        }else if(tiporesgistroSitio.TipoRegistro == "Con aprobacion")
                        {
                            //Creo nuevo user sin contraseña
                            user = new AppUser
                            {
                                Name = infoGoogleUser.Principal.FindFirstValue(ClaimTypes.Name).Replace(" ", ""),
                                UserName = infoGoogleUser.Principal.FindFirstValue(ClaimTypes.Name).Replace(" ", ""),
                                Email = infoGoogleUser.Principal.FindFirstValue(ClaimTypes.Email),
                                Tmstmp = DateTime.Now,
                                TenantId = tenantId,
                                Status = false
                            };
                        }
                        var userCreado = await _userManager.CreateAsync(user);

                        if (!userCreado.Succeeded)
                        {
                            return new AuthReturn { Success = false, Message = $"Error al crear el usuario: {userCreado.Errors}" };
                        }

                    }else if(user == null && tiporesgistroSitio.TipoRegistro == "Cerrado")
                    {
                        return new AuthReturn { Success = false, Message = "Este sitio no admite inscripciones" };

                    }else if(user == null && tiporesgistroSitio.TipoRegistro == "Invitacion Link")
                    {
                        return new AuthReturn { Success = false, Message = "Este sitio solo admite inscripciones mediante link" };
                    }

                    //Agrego sesión a DB (AppNetUsersLogins)
                    var loginAdded = await _userManager.AddLoginAsync(user, infoGoogleUser);
                    if (!loginAdded.Succeeded)
                    {
                        return new AuthReturn { Success = false, Message = "Error al crear la sesion en DB" };
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return new AuthReturn { Success = true, Message = "Login exitoso. Bienvenido!" };
                }
                return new AuthReturn { Success = false, Message = "No hemos podido procesar su cuenta de Google" };

            }
        }

        public async Task<AuthReturn> MauiGoogleLoginAsync(GoogleLoginReq googleUserMaui)
        {
            var userGoogleEnDb = await _userManager.FindByEmailAsync(googleUserMaui.Email);

            if(userGoogleEnDb != null)
            {
                if(userGoogleEnDb.TenantId != googleUserMaui.tenantId)
                {
                    return new AuthReturn { Success = false, Message = "El usuario no pertenece a este sitio" };
                }
            }

            var resultadoLogin = await _signInManager.ExternalLoginSignInAsync("Google",
                googleUserMaui.GoogleId, isPersistent: false, bypassTwoFactor: false);

            //User ya tiene cuenta de Google
            if (resultadoLogin.Succeeded)
            {
                var token = GenerateJwtToken(userGoogleEnDb);
                return new AuthReturn { Success = true, Message = "User guardado con exito", IdUser = userGoogleEnDb.Id, Token = token };
            }
            else
            {
                if (googleUserMaui.Email != null)
                {
                    var tiporesgistroSitio = await _context.Sitios.FirstOrDefaultAsync(s => s.TenantId == googleUserMaui.tenantId);

                    //User no tiene cuenta interna
                    if (userGoogleEnDb == null && tiporesgistroSitio.TipoRegistro != "Cerrado" && tiporesgistroSitio.TipoRegistro != "Invitacion Link")
                    {
                        if (tiporesgistroSitio.TipoRegistro == "Abierto")
                        {

                            //Creo nuevo user sin contraseña
                            userGoogleEnDb = new AppUser
                            {
                                Name = googleUserMaui.Nombre,
                                UserName = googleUserMaui.UserName,
                                Email = googleUserMaui.Email,
                                TenantId = googleUserMaui.tenantId,
                                Status = true,
                                Tmstmp = DateTime.Now
                            };
                        }
                        else if (tiporesgistroSitio.TipoRegistro == "Con aprobacion")
                        {
                            //Creo nuevo user sin contraseña
                            userGoogleEnDb = new AppUser
                            {
                                Name = googleUserMaui.Nombre,
                                UserName = googleUserMaui.UserName,
                                Email = googleUserMaui.Email,
                                TenantId = googleUserMaui.tenantId,
                                Status = false,
                                Tmstmp = DateTime.Now
                            };
                        }

                        var userCreado = await _userManager.CreateAsync(userGoogleEnDb);

                        if (!userCreado.Succeeded)
                        {
                            return new AuthReturn { Success = false, Message = $"Error al crear el usuario: {userCreado.Errors}" };
                        }
                    }
                    else if (userGoogleEnDb == null && tiporesgistroSitio.TipoRegistro == "Cerrado")
                    {
                        return new AuthReturn { Success = false, Message = "Este sitio no admite inscripciones" };

                    }
                    else if (userGoogleEnDb == null && tiporesgistroSitio.TipoRegistro == "Invitacion Link")
                    {
                        return new AuthReturn { Success = false, Message = "Este sitio solo admite inscripciones mediante link" };
                    }

                    //Agrego sesión a DB (AppNetUsersLogins)
                    UserLoginInfo userLoginDB = new UserLoginInfo("Google", googleUserMaui.GoogleId, googleUserMaui.UserName);

                    var loginAdded = await _userManager.AddLoginAsync(userGoogleEnDb, userLoginDB);

                    if (!loginAdded.Succeeded)
                    {
                        return new AuthReturn { Success = false, Message = "Error al crear la sesion en DB" };
                    }

                    await _signInManager.SignInAsync(userGoogleEnDb, isPersistent: false);
                    var token = GenerateJwtToken(userGoogleEnDb);

                    return new AuthReturn { Success = true, Message = "User guardado con exito", IdUser = userGoogleEnDb.Id, Token = token };
                }
                return new AuthReturn { Success = false, Message = "No hemos podido procesar su cuenta de Google" };

            }
        }


        private string GenerateJwtToken(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                //issuer: _configuration["Jwt:Issuer"],
                //audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
