using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tupencauy.Data;
using System.Security.Claims;
using tupencauy.Models;
using tupencauy.ViewModels;
using System.Data;

namespace tupencauy.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;

    // Constructor con inyección de dependencias
    public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, AppDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var model = new LoginUserVM();
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginUserVM model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Username!);

            if (user == null)
            {
                // El usuario no está registrado en ningún sitio
                ModelState.AddModelError(string.Empty, "El usuario no está registrado en ningún sitio.");
                return View(model);
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(user);
                //recupero el tenantId que guarde en la session cuando paso por el middleware
                var tenantId = HttpContext.Session.GetString("TenantId");
                if (roles.Contains("SuperAdmin") || user.TenantId == tenantId)
                {
                    // El usuario es SuperAdmin o el TenantId coincide, proceder con el login
                    var result = await _signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        // Redirigir basado en el rol del usuario
                        if (roles.Contains("SuperAdmin"))
                        {
                            return RedirectToAction("Index", "SuperAdmin");
                        }
                        else if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Intento de inicio de sesión inválido.");
                    }
                }
                else
                {
                    // El TenantId no coincide
                    ModelState.AddModelError(string.Empty, "El usuario no está registrado en el sitio que seleccionaste.");
                    return View(model);
                }
            }


        }

        // Si el modelo no es válido, agregar un mensaje de error genérico
        ModelState.AddModelError(string.Empty, "Algo falló al mostrar el formulario.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register(string tenantId)
    {
        var model = new RegisterUserVM();
        model.TenantId = tenantId ?? HttpContext.Session.GetString("TenantId");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserVM model)
    {
        if (ModelState.IsValid)
        {
            var tenantId = model.TenantId ?? HttpContext.Session.GetString("TenantId");

            if (string.IsNullOrEmpty(tenantId))
            {
                ModelState.AddModelError("", "Tenant ID is required.");
                return View(model);
            }

            var user = new AppUser
            {
                Name = model.Nombre,
                UserName = model.Username,
                Email = model.Email,
                TenantId = tenantId,
                Status = true,
                Tmstmp = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim("tenantId", tenantId));
                await _userManager.AddToRoleAsync(user, "Usuario");
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new EditUserVM
        {
            Id = user.Id,
            Nombre = user.Name,
            Email = user.Email,
            UserName = user.UserName
    };

        return View("~/Views/Account/Edit.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserVM model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = model.Nombre;
            user.Email = model.Email;
            user.UserName = model.UserName;
            // Actualiza otras propiedades según sea necesario

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Usuarios", "SuperAdmin");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View("~/Views/Account/Delete.cshtml", user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction("Usuarios", "SuperAdmin");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        
        // En caso de error, redirige a la vista de confirmación de eliminación con el usuario
        return NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View("~/Views/Account/Details.cshtml", user);
    }


    //[HttpPost]
    //[AllowAnonymous]
    //public IActionResult LoginGoogle(string prov, string urlRetorno)
    //{
    //    var redirigir = Url.Action("ResponseGoogle", "Account", new { ReturnUrl = urlRetorno });

    //    var props = _signInManager.ConfigureExternalAuthenticationProperties(prov, redirigir);

    //    return new ChallengeResult(prov, props);
    //}

    //public async Task<IActionResult> ResponseGoogle(string returnUrl = null, string externalError = null)
    //{
    //    returnUrl = returnUrl ?? Url.Content("~/");
        
    //    if (externalError != null)
    //    {
    //        ModelState.AddModelError(string.Empty, $"Ha habido un error con el proveedor externo: {externalError}");

    //        return View("Login");

    //    }

    //    var infoGoogleUser = await _signInManager.GetExternalLoginInfoAsync();
    //    if (infoGoogleUser == null)
    //    {
    //        ModelState.AddModelError(string.Empty, "Error al cargar la informacion");

    //        return View("Login");
    //    }

    //    var resultadoLogin = await _signInManager.ExternalLoginSignInAsync(infoGoogleUser.LoginProvider,
    //        infoGoogleUser.ProviderKey, isPersistent: false, bypassTwoFactor: false);

    //    //User tiene cuenta interna, y entra con la de Google
    //    if (resultadoLogin.Succeeded)
    //    {
    //        return LocalRedirect(returnUrl);
    //    }

    //    else
    //    {
    //        var email = infoGoogleUser.Principal.FindFirstValue(ClaimTypes.Email);

    //        if (email != null)
    //        {
    //            var user = await _userManager.FindByEmailAsync(email);

    //            //User no tiene cuenta interna
    //            if (user == null)
    //            {
    //                //Creo nuevo user sin contraseña
    //                user = new AppUser
    //                {
    //                    Name = infoGoogleUser.Principal.FindFirstValue(ClaimTypes.Name).Replace(" ", ""),
    //                    UserName = infoGoogleUser.Principal.FindFirstValue(ClaimTypes.Name).Replace(" ", ""),
    //                    Email = infoGoogleUser.Principal.FindFirstValue(ClaimTypes.Email),
    //                    Tmstmp = DateTime.Now
    //                };

    //                await _userManager.CreateAsync(user);
    //            }

    //            //Agrego sesión a DB (AppNetUsersLogins)
    //            var addUser = await _userManager.AddLoginAsync(user, infoGoogleUser);

    //            await _signInManager.SignInAsync(user, isPersistent: false);

    //            return LocalRedirect(returnUrl);
    //        }

    //        ViewBag.ErrorTitle = $"Email claim no fue recibido de: {infoGoogleUser.LoginProvider}";
    //        ViewBag.ErrorMessage = "Contactese a santiagotutzo@gmail.com";

    //        return View("Error");
    //    }
    //}

}

