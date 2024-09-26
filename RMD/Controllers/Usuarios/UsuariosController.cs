using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Pacientes;
using RMD.Interface.Usuarios;
using RMD.Models.Responses;
using RMD.Models.Usuarios;
using System.Net;
using System.Security.Claims;

namespace RMD.Controllers.Usuarios
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController(IUsuarioService usuarioService, IPacienteService pacienteService) : ControllerBase
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly IPacienteService _pacienteService = pacienteService;

        [HttpPost("crear")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> CrearUsuario([FromBody] UsuarioCreate usuario)
        {
            if (usuario == null)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Datos inválidos."));
            }

            var idRol = User.FindFirstValue("IdRol");
            var rol = User.FindFirstValue(ClaimTypes.Role);
            var idGemp = User.FindFirstValue("GEMP");
            var idSucursal = User.FindFirstValue("IdSucursal");

            if (string.IsNullOrEmpty(idRol) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(idGemp) || string.IsNullOrEmpty(idSucursal))
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Faltan datos en el token."));
            }

            if (!Guid.TryParse(idRol, out Guid idRolGuid) || !Guid.TryParse(idGemp, out Guid idGempGuid) || !Guid.TryParse(idSucursal, out Guid idSucursalGuid))
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Los valores de GUID no son válidos."));
            }

            var (mensaje, idUsuario) = await _usuarioService.AddUsuarioAsync(usuario, idRolGuid, rol, idGempGuid, idSucursalGuid);

            if (idUsuario == Guid.Empty)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, mensaje));
            }

            var responseData = new { Mensaje = mensaje, IdUsuario = idUsuario };
            return Ok(ResponseFromService<object>.Success(responseData, "Usuario creado con éxito."));
        }

        [HttpPut("actualizar")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> UpdateUsuario([FromBody] Usuario usuario)
        {
            if (usuario == null)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Datos inválidos."));
            }

            var idUsuarioSolicitante = User.FindFirstValue("IdUsuario");

            if (!Guid.TryParse(idUsuarioSolicitante, out var parsedIdUsuarioSolicitante))
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Id del usuario solicitante no válido."));
            }

            var (mensaje, exito) = await _usuarioService.UpdateUsuarioAsync(usuario, parsedIdUsuarioSolicitante);

            if (!exito)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, mensaje));
            }

            return Ok(ResponseFromService<string>.Success(null, mensaje));
        }

        [HttpGet("tipo/{idTipoUsuario}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosByTipoUsuario(Guid idTipoUsuario)
        {
            var usuarios = await _usuarioService.GetUsuariosByTipoUsuarioAsync(idTipoUsuario);
            return Ok(ResponseFromService<IEnumerable<Usuario>>.Success(usuarios ?? new List<Usuario>(), usuarios == null || !usuarios.Any() ? "No se encontraron usuarios para el tipo especificado." : ""));
        }

        [HttpGet("gemp/{idGEMP}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosByGEMP(Guid idGEMP)
        {
            var usuarios = await _usuarioService.GetUsuariosByGEMPAsync(idGEMP);
            return Ok(ResponseFromService<IEnumerable<Usuario>>.Success(usuarios ?? new List<Usuario>(), usuarios == null || !usuarios.Any() ? "No se encontraron usuarios para el GEMP especificado." : ""));
        }

        [HttpGet("sucursal/{idSucursal}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosBySucursal(Guid idSucursal)
        {
            var usuarios = await _usuarioService.GetUsuariosBySucursalAsync(idSucursal);
            return Ok(ResponseFromService<IEnumerable<Usuario>>.Success(usuarios ?? new List<Usuario>(), usuarios == null || !usuarios.Any() ? "No se encontraron usuarios para la sucursal especificada." : ""));
        }

        [HttpGet("asentamiento/{idAsentamiento}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuariosByAsentamiento(int idAsentamiento)
        {
            var usuarios = await _usuarioService.GetUsuariosByAsentamientoAsync(idAsentamiento);
            return Ok(ResponseFromService<IEnumerable<Usuario>>.Success(usuarios ?? new List<Usuario>(), usuarios == null || !usuarios.Any() ? "No se encontraron usuarios para el asentamiento especificado." : ""));
        }

        [HttpPost("obtener-usuarios")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var idUsuario = User.FindFirstValue("IdUsuario");
            var idRol = User.FindFirstValue("IdRol");

            if (Guid.TryParse(idUsuario, out var parsedIdUsuario) && Guid.TryParse(idRol, out var parsedIdRol))
            {
                var usuarios = await _usuarioService.ObtenerUsuariosPorIdUsuarioYRolAsync(parsedIdUsuario, parsedIdRol);

                return Ok(ResponseFromService<IEnumerable<UsuarioDetalle>>.Success(usuarios ?? new List<UsuarioDetalle>(), usuarios == null || !usuarios.Any() ? "No se encontraron usuarios." : ""));
            }

            return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "No se pudo obtener el IdUsuario o el IdRol del token."));
        }

        [HttpPost("eliminar-usuario/{idUsuario}")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> EliminarUsuario(Guid idUsuario)
        {
            var idUsuarioSolicitante = User.FindFirstValue("IdUsuario");

            if (!Guid.TryParse(idUsuarioSolicitante, out var parsedIdUsuarioSolicitante))
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Id del usuario solicitante no válido."));
            }

            var (mensaje, exito) = await _usuarioService.InactivarUsuarioAsync(idUsuario, parsedIdUsuarioSolicitante);

            if (!exito)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, mensaje));
            }

            return Ok(ResponseFromService<string>.Success(null, mensaje));
        }

        [HttpPost("cambiar-password")]
        [Authorize]
        [ServiceFilter(typeof(ValidateTokenFilter))]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.NuevaPassword) || string.IsNullOrEmpty(request.ConfirmacionPassword))
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Datos inválidos."));
            }

            if (request.NuevaPassword != request.ConfirmacionPassword)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Las contraseñas no coinciden."));
            }

            var idUsuario = User.FindFirstValue("IdUsuario");

            if (!Guid.TryParse(idUsuario, out var parsedIdUsuario))
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Id del usuario no válido."));
            }

            var (mensaje, exito) = await _usuarioService.CambiarPasswordAsync(parsedIdUsuario, request.NuevaPassword);

            if (!exito)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, mensaje));
            }

            return Ok(ResponseFromService<string>.Success(null, mensaje));
        }
    }
}
