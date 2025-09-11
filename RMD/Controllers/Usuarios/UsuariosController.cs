using RMD.Interface.Usuarios;
using RMD.Shared.Models.Usuarios;

namespace RMD.Controllers.Usuarios
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class UsuariosController(IUsuarioService usuarioService, ICatalogoNotificacionService catalogoNotificacionService) : ControllerBase
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService = catalogoNotificacionService;

        [HttpPost("crear")]
        public async Task<IActionResult> CrearUsuario([FromBody] UsuarioCreate usuario)
        {
            // Verificar permiso
            if (!HasPermission("CrearUsuario"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            // Validar el modelo
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("USUARIOSC", "DATOS_INVALIDOS");
                notificacion.Mensaje = errores;

                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            if (!ValidationHelper.IsValidEmail(usuario.Email))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EMAIL_INVALIDO");
                return BadRequest(ResponseFromService<object>.Failure(notificacion));
            }

            if (!ValidationHelper.IsValidPassword(usuario.Password))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "PASSWORD_INVALIDO");
                return BadRequest(ResponseFromService<object>.Failure(notificacion));
            }

            // Extraer datos del token: IdRol, GEMP e IdSucursal
            var idRol = User.FindFirstValue("IdRol");
            var idGemp = User.FindFirstValue("GEMP");
            var idSucursal = User.FindFirstValue("IdSucursal");


            if (!Guid.TryParse(idRol, out Guid idRolGuid) ||
                !Guid.TryParse(idGemp, out Guid idGempGuid) ||
                !Guid.TryParse(idSucursal, out Guid idSucursalGuid))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO"); // TOKEN INCOMPLETO
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            // Crear el usuario
            var crear_usuario = await _usuarioService.AddUsuarioAsync(usuario, idRolGuid, User.FindFirstValue(ClaimTypes.Role), idGempGuid, idSucursalGuid);

            if (crear_usuario.Toast != "success" && crear_usuario.Toast != "info")
            {
                return BadRequest(crear_usuario);
            }
            return Ok(crear_usuario);
        }

        [HttpPut("actualizar")]
        public async Task<IActionResult> UpdateUsuario([FromBody] Usuario usuario)
        {
            if (!HasPermission("UpdateUsuario"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                notificacion.Mensaje = errores;

                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }
            if (!ValidationHelper.IsValidEmail(usuario.Email))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EMAIL_INVALIDO");
                return BadRequest(ResponseFromService<object>.Failure(notificacion));
            }

            if (!string.IsNullOrEmpty(usuario.Password) && !ValidationHelper.IsValidPassword(usuario.Password))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "PASSWORD_INVALIDO");
                return BadRequest(ResponseFromService<object>.Failure(notificacion));
            }
            

            var idUsuarioSolicitante = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioSolicitante, out var parsedIdUsuarioSolicitante))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var actualizar_usuario = await _usuarioService.UpdateUsuarioAsync(usuario, parsedIdUsuarioSolicitante);

            if (actualizar_usuario.Toast != "success" && actualizar_usuario.Toast != "info")
            {
                return BadRequest(actualizar_usuario);
            }

            return Ok(actualizar_usuario);
        }

        [HttpGet("gemp/{idGEMP}")]
        public async Task<IActionResult> GetUsuariosByGEMP(Guid idGEMP)
        {
            if (!HasPermission("GetUsuariosByGEMP"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var usuarios = await _usuarioService.GetUsuariosByGEMPAsync(idGEMP);

            if (usuarios.Toast != "success" && usuarios.Toast != "info")
            {
                return BadRequest(usuarios);
            }

            return Ok(usuarios);
        }

        [HttpGet("sucursal/{idSucursal}")]
        public async Task<IActionResult> GetUsuariosBySucursal(Guid idSucursal)
        {
            if (!HasPermission("GetUsuariosBySucursal"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var usuarios = await _usuarioService.GetUsuariosBySucursalAsync(idSucursal);

            if (usuarios.Toast != "success" && usuarios.Toast != "info")
            {
                return BadRequest(usuarios);
            }

            return Ok(usuarios);
        }


        [HttpPost("obtener-usuarios")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            if (!HasPermission("ObtenerUsuarios"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var idUsuario = User.FindFirstValue("IdUsuario");
            var idRol = User.FindFirstValue("IdRol");

            if (!Guid.TryParse(idUsuario, out var parsedIdUsuario) || !Guid.TryParse(idRol, out var parsedIdRol))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var usuarios = await _usuarioService.ObtenerUsuariosPorIdUsuarioYRolAsync(parsedIdUsuario, parsedIdRol);
            return (usuarios.Toast == "success" || usuarios.Toast == "info") ? Ok(usuarios) : BadRequest(usuarios);
        }

        [HttpPost("eliminar-usuario/{idUsuario}")]
        public async Task<IActionResult> EliminarUsuario(Guid idUsuario)
        {
            if (!HasPermission("EliminarUsuario"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var idUsuarioSolicitante = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuarioSolicitante, out var parsedIdUsuarioSolicitante))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var resultado = await _usuarioService.InactivarUsuarioAsync(idUsuario, parsedIdUsuarioSolicitante);
            return (resultado.Toast == "success" || resultado.Toast == "info") ? Ok(resultado) : BadRequest(resultado);
        }

        [HttpPost("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(request.NuevaPassword) || string.IsNullOrEmpty(request.ConfirmacionPassword))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            if (!ValidationHelper.IsValidPassword(request.NuevaPassword))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "PASSWORD_INVALIDO");
                return BadRequest(ResponseFromService<object>.Failure(notificacion));
            }

            if (request.NuevaPassword != request.ConfirmacionPassword)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("USUARIOSC", "PASSWORD_NO_COINCIDE");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var idUsuario = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuario, out var parsedIdUsuario))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var resultado = await _usuarioService.CambiarPasswordAsync(parsedIdUsuario, request.NuevaPassword);
            return (resultado.Toast == "success" || resultado.Toast == "info") ? Ok(resultado) : BadRequest(resultado);
        }

        [HttpPost("imagen-firma")]
        public async Task<IActionResult> CrearActualizarImagenFirma([FromBody] UsuarioImagenRequest request)
        {
            if (!HasPermission("CrearActualizarImagenFirma"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            if (!ModelState.IsValid)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var resultado = await _usuarioService.CrearActualizarImagenFirmaAsync(request);
            return (resultado.Toast == "success" || resultado.Toast == "info") ? Ok(resultado) : BadRequest(resultado);
        }

        [HttpGet("firma")]
        public async Task<IActionResult> ObtenerFirma()
        {
            if (!HasPermission("ObtenerFirma"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var idUsuario = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuario, out var parsedIdUsuario))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var resultado = await _usuarioService.ObtenerFirmaPorIdUsuarioAsync(parsedIdUsuario);
            return (resultado.Toast == "success" || resultado.Toast == "info") ? Ok(resultado) : BadRequest(resultado);
        }

        [HttpDelete("firma")]
        public async Task<IActionResult> EliminarFirma()
        {
            if (!HasPermission("EliminarFirma"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var idUsuario = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuario, out var parsedIdUsuario))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var resultado = await _usuarioService.EliminarFirmaAsync(parsedIdUsuario);
            return (resultado.Toast == "success" || resultado.Toast == "info") ? Ok(resultado) : BadRequest(resultado);
        }

        [HttpGet("imagen")]
        public async Task<IActionResult> ObtenerImagen()
        {
            if (!HasPermission("ObtenerImagen"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var idUsuario = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuario, out var parsedIdUsuario))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var resultado = await _usuarioService.ObtenerImagenPorIdUsuarioAsync(parsedIdUsuario);
            return (resultado.Toast == "success" || resultado.Toast == "info") ? Ok(resultado) : BadRequest(resultado);
        }

        [HttpDelete("imagen")]
        public async Task<IActionResult> EliminarImagen()
        {
            if (!HasPermission("EliminarImagen"))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "NOPERMISOS");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var idUsuario = User.FindFirstValue("IdUsuario");
            if (!Guid.TryParse(idUsuario, out var parsedIdUsuario))
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notificacion));
            }

            var resultado = await _usuarioService.EliminarImagenAsync(parsedIdUsuario);
            return (resultado.Toast == "success" || resultado.Toast == "info") ? Ok(resultado) : BadRequest(resultado);
        }

        private bool HasPermission(string endpointName)
        {
            var rol = User.FindFirstValue(ClaimTypes.Role);
            return RolesPermissions.UsuariosController.EndpointRolesUsuariosController[endpointName].Contains(rol);
        }
    }
}
