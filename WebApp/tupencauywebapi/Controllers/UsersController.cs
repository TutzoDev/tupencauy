using Microsoft.AspNetCore.Mvc;
using tupencauy.Data;
using tupencauy.Models;
using tupencauywebapi.DTOs;
using tupencauywebapi.Models;
using tupencauywebapi.Services;

namespace tupencauy.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IServicioUser _servicioUser;

        public UsersController(AppDbContext context, IServicioUser servicioUser)
        {
            _context = context;
            _servicioUser = servicioUser;
        }

        [HttpGet("{id}/GetInfoUser")]
        public async Task<ActionResult<AppUserDTO>> GetUserById(string id)
        {
            var usuarioReturn = await _servicioUser.GetUsuario(id);

            if (usuarioReturn == null)
            {
                return NotFound();
            }

            return Ok(usuarioReturn);
        }

        [HttpPost("EditUser")]
        public async Task<IActionResult> EditUser(EditUserReq userEdit)
        {
            var usuarioEdit = await _servicioUser.EditarUsuario(userEdit);

            if (!usuarioEdit.Success)
            {
                return BadRequest();
            }
            return Ok(usuarioEdit);
        }
        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit([FromForm] DepositDTO depositDto)
        {
            if (depositDto.paymentReceipt == null || depositDto.paymentReceipt.Length == 0)
                return BadRequest("Comprobante de pago no subido.");

            var user = await _context.Usuarios.FindAsync(depositDto.idUser);
            if (user == null)
                return NotFound("Usuario no encontrado.");

            // Guardar el archivo en el servidor
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, depositDto.paymentReceipt.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await depositDto.paymentReceipt.CopyToAsync(fileStream);
            }

            // Crear una nueva entidad Recarga
            var recarga = new Recarga
            {
                IdUsuario = depositDto.idUser,
                Carga = depositDto.depositAmount,
                ComprobantePath = $"{depositDto.paymentReceipt.FileName}",
                Aprobado = null,
                Tmstmp = DateTime.Now
            };

            _context.Recargas.Add(recarga);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Depósito registrado exitosamente" });
        }

        [HttpGet("{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var image = System.IO.File.OpenRead(filePath);
            return File(image, "image/jpeg");
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetUsers()
        //{
        //    var usuarios = await _context.Usuarios.ToListAsync();
        //    var usuariosFiltrados = new List<AppUserDTO>();

        //    foreach (var usuario in usuarios)
        //    {
        //        var roles = await _userManager.GetRolesAsync(usuario);
        //        if (!roles.Contains("SuperAdmin"))
        //        {

        //            var usuarioFiltrado = new AppUserDTO
        //            {
        //                Id = usuario.Id,
        //                Nombre = usuario.Name,
        //                UserName = usuario.UserName,
        //                Email = usuario.Email
        //            };
        //            usuariosFiltrados.Add(usuarioFiltrado);
        //        }
        //    }

        //    return Ok(usuariosFiltrados);
        //}
    }
}