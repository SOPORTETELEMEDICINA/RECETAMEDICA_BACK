using RMD.Interface.Auth;
using RMD.Interface.Security;
using RMD.Interface.Usuarios;
using RMD.Shared.Models.Login;
using RMD.Shared.Utils.Interface;

namespace RMD.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IEncryptionService _encryptionService;

        public AuthController(
            IAuthService authService,
            IUsuarioService usuarioService,
            IConfiguration configuration,
            IEmailService emailService,
            ICatalogoNotificacionService catalogoNotificacionService,
            IEncryptionService encryptionService)
        {
            _authService = authService;
            _usuarioService = usuarioService;
            _configuration = configuration;
            _emailService = emailService;
            _catalogoNotificacionService = catalogoNotificacionService;
            _encryptionService = encryptionService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
        {
            if (!ModelState.IsValid)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "MODELO_INVALIDO");
                return BadRequest(ResponseFromService<object>.Failure(notif));
            }

            if (!ValidationHelper.IsValidPassword(credentials.Password))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "PASSWORD_INVALIDO");
                return BadRequest(ResponseFromService<string>.Failure(notif));
            }

            var loginResult = await _authService.LoginAsync(credentials);
            if (loginResult.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(loginResult);

            var userResult = await _usuarioService.GetUsuarioByUsernameAsync(credentials.Usr);
            if (userResult.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(userResult);

            // Usamos el servicio ya inyectado
            var tokenPlain = loginResult.Data;
            var userPlain = JsonSerializer.Serialize(userResult.Data);

            var tokenEncrypted = _encryptionService.EncryptString(tokenPlain);
            var userEncrypted = _encryptionService.EncryptString(userPlain);

            var successNotif = await _catalogoNotificacionService
                .GetNotificationByTipoAndFuncionAsync("AUTHC", "LOGIN_EXITOSO");

            var response = new
            {
                token = ParseEncryptedString(tokenEncrypted),
                user = ParseEncryptedString(userEncrypted)
            };

            return Ok(ResponseFromService<object>.Success(response, successNotif));
        }

        // Utilidad local para separar el IV y el texto cifrado
        private static object ParseEncryptedString(string encrypted)
        {
            var parts = encrypted.Split(':');
            return new
            {
                iv = parts[0],
                cipherText = parts[1]
            };
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // 1) Extraer token de la cabecera
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            // 2) Ejecutar el logout en el service
            var logoutResult = await _authService.LogoutAsync(token);
            if (logoutResult.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
                return BadRequest(logoutResult);

            // 3) Obtener notificación de éxito AuthC/LOGOUT_EXITOSO (20001)
            var notif = await _catalogoNotificacionService
                .GetNotificationByTipoAndFuncionAsync("AUTHC", "LOGOUT_EXITOSO");

            // 4) Devolver ResponseFromService<bool>
            return Ok(ResponseFromService<bool>.Success(true, notif));
        }

        [HttpPost("renew")]
        public async Task<IActionResult> RenewToken()
        {
            // 1) Intentar renovar el token
            var renewResult = await _authService.RenewTokenAsync();

            // 2) Si hubo error, devolvemos 401 con el ResponseFromService ya formateado
            if (renewResult.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(renewResult);

            // 3) Éxito: buscamos notificación AuthC/RENEW_EXITOSO (20002)
            var successNotif = await _catalogoNotificacionService
                .GetNotificationByTipoAndFuncionAsync("AUTHC", "RENEW_EXITOSO");
            
            // 4) Devolvemos el nuevo token en un ResponseFromService<string>
            return Ok(ResponseFromService<string>.Success(_encryptionService.EncryptString(renewResult.Data), successNotif));
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            // 1) Validación de modelo y formato de email
            if (!ModelState.IsValid)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "MODELO_INVALIDO");
                return BadRequest(ResponseFromService<object>.Failure(notif));
            }
            if (!ValidationHelper.IsValidEmail(request.Email))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EMAIL_INVALIDO");
                return BadRequest(ResponseFromService<object>.Failure(notif));
            }

            // 2) Buscar usuario por email
            var userResult = await _usuarioService.GetUsuarioByEmailAsync(request.Email);
            if (userResult.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
                return NotFound(userResult);

            // 3) Generar token de reseteo
            var tokenResult = await _authService.GeneratePasswordResetTokenAsync(userResult.Data.IdUsuario);
            if (tokenResult.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
                return BadRequest(tokenResult);

            // 4) Validar configuración de URL de front
            var environment = _configuration["Environment"];
            var frontendUrl = _configuration[$"AppSettings:FrontendUrl{environment}"];
            if (string.IsNullOrEmpty(frontendUrl))
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                return BadRequest(ResponseFromService<object>.Failure(notif));
            }

            // 5) Enviar correo
            var resetLink = $"{frontendUrl}#/authentication/reset/{tokenResult.Data}";
            await _emailService.SendEmailAsync(
                userResult.Data.Email,
                "SOLICITUD PARA RESTABLECER CONTRASEÑA",
                resetLink
            );

            // 6) Éxito: AuthC/FORGOT_PASSWORD_OK = 20003
            var successNotif = await _catalogoNotificacionService
                .GetNotificationByTipoAndFuncionAsync("AUTHC", "FORGOT_PASSWORD_OK");
            return Ok(ResponseFromService<string>.Success(resetLink, successNotif));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            // 1) Validación de modelo
            if (!ModelState.IsValid)
            {
                var m = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "MODELO_INVALIDO");
                return BadRequest(ResponseFromService<object>.Failure(m));
            }

            // 2) Complejidad de la nueva contraseña
            if (!ValidationHelper.IsValidPassword(request.NewPassword))
            {
                var m = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "PASSWORD_INVALIDO");
                return BadRequest(ResponseFromService<object>.Failure(m));
            }

            // 3) Confirmación de contraseña
            if (request.NewPassword != request.ConfirmPassword)
            {
                var m = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("AUTHC", "PASSWORDS_NO_COINCIDEN");
                return BadRequest(ResponseFromService<object>.Failure(m));
            }

            // 4) Validar token de reseteo
            var validate = await _authService.ValidateResetTokenAsync(request.Token);
            if (validate.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
                return BadRequest(validate);

            // 5) Actualizar la contraseña
            var update = await _authService.UpdatePasswordAsync(validate.Data.UserId, request.NewPassword);
            if (update.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
                return StatusCode(500, update);

            // 6) Revocar el token usado
            await _authService.RevokeTokenAsync(request.Token, DateTime.Now.AddMinutes(-1));

            // 7) Respuesta de éxito AuthC/RESET_PASSWORD_OK = 20004
            var success = await _catalogoNotificacionService
                .GetNotificationByTipoAndFuncionAsync("AUTHC", "RESET_PASSWORD_OK");
            return Ok(ResponseFromService<string>.Success(string.Empty, success));
        }
        // AuthController.cs (añadir endpoints)
        [HttpPost("change-sucursal")]
        public async Task<IActionResult> ChangeSucursal([FromBody] Guid idSucursal)
        {
            var result = await _authService.ChangeSucursalAsync(idSucursal);
            if (result.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(result);
            var success = await _catalogoNotificacionService
                .GetNotificationByCodeAsync(result.Code);

            // 4) Devolvemos el nuevo token en un ResponseFromService<string>
            return Ok(ResponseFromService<string>.Success(_encryptionService.EncryptString(result.Data), success));
        }

        [HttpPost("change-gemp")]
        public async Task<IActionResult> ChangeGemp([FromBody] Guid idGemp)
        {
            var result = await _authService.ChangeGempAsync(idGemp);
            if (result.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(result);
            var success = await _catalogoNotificacionService
                .GetNotificationByCodeAsync(result.Code);

            // 4) Devolvemos el nuevo token en un ResponseFromService<string>
            return Ok(ResponseFromService<string>.Success(_encryptionService.EncryptString(result.Data), success));        
        }

        //// 3) Añade en AuthController.cs
        //[HttpPost("send-2fa")]
        //public async Task<IActionResult> Send2FACode()
        //{
        //    var result = await _authService.Send2FACodeAsync();
        //    if (result.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
        //        return BadRequest(result);
        //    return Ok(result);
        //}

        //[HttpPost("validate-2fa")]
        //public async Task<IActionResult> Validate2FACode([FromBody] string code)
        //{
        //    var result = await _authService.Validate2FACodeAsync(code);
        //    if (result.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
        //        return BadRequest(result);
        //    return Ok(result);
        //}

        // Models/EncryptedPayload.cs
        

    }
}
