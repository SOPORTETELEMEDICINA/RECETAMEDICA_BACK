using RMD.Extensions;
using RMD.Interface.Pacientes;
using RMD.Interface.Usuarios;
using RMD.Models.Responses;
using RMD.Models.Usuarios;

namespace RMD.Controllers.Usuarios
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class UsuariosController(IUsuarioService usuarioService
        //, IPacienteService pacienteService
        ) : ControllerBase
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        //private readonly IPacienteService _pacienteService = pacienteService;

        [HttpPost("crear")]
        public async Task<IActionResult> CrearUsuario([FromBody] UsuarioCreate usuario)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.UsuariosController.EndpointRolesUsuariosController["CrearUsuario"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }
            if (usuario == null)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Datos inválidos."));
            }
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, errores));
            }

            var idRol = User.FindFirstValue("IdRol");
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
        public async Task<IActionResult> UpdateUsuario([FromBody] Usuario usuario)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.UsuariosController.EndpointRolesUsuariosController["UpdateUsuario"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }
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

        [HttpGet("gemp/{idGEMP}")]
        public async Task<ActionResult<IEnumerable<RequestUsuario>>> GetUsuariosByGEMP(Guid idGEMP)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.UsuariosController.EndpointRolesUsuariosController["GetUsuariosByGEMP"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }
            try
            {
                // Llamar al servicio para obtener los usuarios
                var usuarios = await _usuarioService.GetUsuariosByGEMPAsync(idGEMP);

                // Verificar si no se encontraron resultados
                if (usuarios == null || !usuarios.Any())
                {
                    return NotFound(ResponseFromService<IEnumerable<RequestUsuario>>.Failure(HttpStatusCode.NotFound, "No se encontraron usuarios para el GEMP especificado."));
                }

                return Ok(ResponseFromService<IEnumerable<RequestUsuario>>.Success(usuarios, "Usuarios obtenidos con éxito."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Error interno del servidor."));
            }
        }

        [HttpGet("sucursal/{idSucursal}")]
        public async Task<ActionResult<IEnumerable<RequestUsuario>>> GetUsuariosBySucursal(Guid idSucursal)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.UsuariosController.EndpointRolesUsuariosController["GetUsuariosBySucursal"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }
            try
            {
                // Llamar al servicio para obtener los usuarios
                var usuarios = await _usuarioService.GetUsuariosBySucursalAsync(idSucursal);

                // Verificar si no se encontraron resultados
                if (usuarios == null || !usuarios.Any())
                {
                    return NotFound(ResponseFromService<IEnumerable<RequestUsuario>>.Failure(HttpStatusCode.NotFound, "No se encontraron usuarios para la sucursal especificada."));
                }

                return Ok(ResponseFromService<IEnumerable<RequestUsuario>>.Success(usuarios, "Usuarios obtenidos con éxito."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Error interno del servidor."));
            }
        }

        [HttpPost("obtener-usuarios")]
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
        public async Task<IActionResult> EliminarUsuario(Guid idUsuario)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            if (!RolesPermissions.UsuariosController.EndpointRolesUsuariosController["EliminarUsuario"].Contains(rol))
            {
                return Forbid("No tiene permisos para acceder a este recurso.");
            }
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

        [HttpPost("imagen-firma")]
        public async Task<IActionResult> CrearActualizarImagenFirma([FromBody] UsuarioImagenRequest request)
        {
            if (request == null)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Datos inválidos."));
            }

            var idUsuario = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuario, out var parsedIdUsuario))
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "IdUsuario no válido en el token."));
            }

            // Establecer el IdUsuario desde el token
            request.IdUsuario = parsedIdUsuario;

            var mensaje = await _usuarioService.CrearActualizarImagenFirmaAsync(request);

            return Ok(ResponseFromService<string>.Success(mensaje, "Operación realizada con éxito."));
        }


        [HttpGet("firma")]
        public async Task<IActionResult> ObtenerFirma()
        {
            var idUsuario = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuario, out var parsedIdUsuario))
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "IdUsuario no válido en el token."));
            }

            var firma = await _usuarioService.ObtenerFirmaPorIdUsuarioAsync(parsedIdUsuario);
            if (string.IsNullOrEmpty(firma))
            {
                return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "Firma no encontrada."));
            }

            return Ok(ResponseFromService<string>.Success(firma, "Firma obtenida con éxito."));
        }


        [HttpDelete("firma")]
        public async Task<IActionResult> EliminarFirma()
        {
            var idUsuario = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuario, out var parsedIdUsuario))
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "IdUsuario no válido en el token."));
            }

            var mensaje = await _usuarioService.EliminarFirmaAsync(parsedIdUsuario);
            return Ok(ResponseFromService<string>.Success(mensaje, "Firma eliminada con éxito."));
        }

        [HttpGet("imagen")]
        public async Task<IActionResult> ObtenerImagen()
        {
            var idUsuario = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuario, out var parsedIdUsuario))
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "IdUsuario no válido en el token."));
            }

            var imagen = await _usuarioService.ObtenerImagenPorIdUsuarioAsync(parsedIdUsuario);
            if (string.IsNullOrEmpty(imagen))
            {
                return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "Imagen no encontrada."));
            }

            return Ok(ResponseFromService<string>.Success(imagen, "Imagen obtenida con éxito."));
        }

        [HttpDelete("imagen")]
        public async Task<IActionResult> EliminarImagen()
        {
            var idUsuario = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuario, out var parsedIdUsuario))
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "IdUsuario no válido en el token."));
            }

            var mensaje = await _usuarioService.EliminarImagenAsync(parsedIdUsuario);
            return Ok(ResponseFromService<string>.Success(mensaje, "Imagen eliminada con éxito."));
        }


    }
}
