using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tupencauy.Data;
using tupencauy.Models;
using tupencauywebapi.DTOs;
using tupencauywebapi.Models;

namespace tupencauywebapi.Services
{
    public class ServicioUser : IServicioUser
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ServicioUser(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<AppUserDTO> GetUsuario(string id)
        {
            AppUser usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            
            if(usuario == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            if (!roles.Contains("SuperAdmin") && !roles.Contains("Admin"))
            {
                var usuarioReturn = new AppUserDTO
                {
                    Nombre = usuario.Name,
                    UserName = usuario.UserName,
                    Email = usuario.Email,
                    Saldo = usuario.Saldo
                };

                return usuarioReturn;
            }
            return null;
        }

        public async Task<AuthReturn> EditarUsuario(EditUserReq user)
        {
            AppUser usuarioModificado = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == user.Id);

            if(usuarioModificado != null && user != null && user.Nombre != "" && user.UserName != "")
            {
                if(usuarioModificado.Name != user.Nombre)
                    usuarioModificado.Name = user.Nombre;
                if (usuarioModificado.UserName != user.UserName)
                {
                    await _userManager.SetUserNameAsync(usuarioModificado, user.UserName);
                    await _userManager.UpdateNormalizedUserNameAsync(usuarioModificado);
                }

                if (user.CurrentPassword != user.NewPassword)
                {
                    var passwordActualValida = await _userManager.CheckPasswordAsync(usuarioModificado, user.CurrentPassword);

                    if(!passwordActualValida)
                    {
                        return new AuthReturn { Success = false, Message = "Tu contraseña actual no es esa, vuelve a intentarlo." };
                    }
                }
                
                await _userManager.ChangePasswordAsync(usuarioModificado, user.CurrentPassword, user.NewPassword);

                _context.Update(usuarioModificado);
                _context.SaveChanges();

                return new AuthReturn { Success = true, Message = "Usuario modificado correctamente!" };
            }

            return new AuthReturn { Success = false, Message = "Algo ha salido mal!" };
        }

    }
}
