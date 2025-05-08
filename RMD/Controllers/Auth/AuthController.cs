using System.Security.Cryptography;
using System.Text;
using RMD.Interface.Auth;
using RMD.Interface.Notificaciones;
using RMD.Interface.Usuarios;
using RMD.Models.Login;
using RMD.Models.Responses;

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

        public AuthController(
            IAuthService authService,
            IUsuarioService usuarioService,
            IConfiguration configuration,
            IEmailService emailService,
            ILogger<AuthController> logger,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _authService = authService;
            _usuarioService = usuarioService;
            _configuration = configuration;
            _emailService = emailService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
        {
            // 1) Validación de modelo y contraseña
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

            // 2) Intentar login
            var loginResult = await _authService.LoginAsync(credentials);
            if (loginResult.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(loginResult);

            // 3) Cargar detalles de usuario
            var userResult = await _usuarioService.GetUsuarioByUsernameAsync(credentials.Usr);
            if (userResult.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(userResult);

            // 4) Preparamos las dos cargas a encriptar por separado
            var tokenPlain = loginResult.Data;                // tu JWT en claro
            var userPlain = System.Text.Json.JsonSerializer.Serialize(userResult.Data);

            // 5) Función auxiliar para encriptar un string y devolver { iv, cipherText }
            (string iv, string cipher) Encrypt(string plain)
            {
                var keyBytes = Convert.FromBase64String(_configuration["Encryption:Key"]!.Trim());
                using var aes = Aes.Create();
                aes.Key = keyBytes;
                aes.GenerateIV();
                using var encryptor = aes.CreateEncryptor();
                var plainBytes = Encoding.UTF8.GetBytes(plain);
                var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                return (
                    iv: Convert.ToBase64String(aes.IV),
                    cipher: Convert.ToBase64String(cipherBytes)
                );
            }

            var tokenEncrypted = Encrypt(tokenPlain);
            var userEncrypted = Encrypt(userPlain);

            // 6) Notificación de éxito
            var successNotif = await _catalogoNotificacionService
                .GetNotificationByTipoAndFuncionAsync("AUTHC", "LOGIN_EXITOSO");

            // 7) Devolvemos un objeto con ambos encriptados por separado
            var response = new
            {
                token = new
                {
                    iv = tokenEncrypted.iv,
                    cipherText = tokenEncrypted.cipher
                },
                user = new
                {
                    iv = userEncrypted.iv,
                    cipherText = userEncrypted.cipher
                }
            };

            return Ok(ResponseFromService<object>.Success(response, successNotif));
        }


        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
        //{
        //    // 1) Validación de modelo y contraseña
        //    if (!ModelState.IsValid)
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "MODELO_INVALIDO");
        //        return BadRequest(ResponseFromService<object>.Failure(notif));
        //    }
        //    if (!ValidationHelper.IsValidPassword(credentials.Password))
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "PASSWORD_INVALIDO");
        //        return BadRequest(ResponseFromService<string>.Failure(notif));
        //    }

        //    // 2) Intentar login
        //    var loginResult = await _authService.LoginAsync(credentials);
        //    if (loginResult.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
        //        return Unauthorized(loginResult);

        //    // 3) Cargar detalles de usuario
        //    var userDetails = await _usuarioService.GetUsuarioByUsernameAsync(credentials.Usr);
        //    if (userDetails.Toast.Equals("error", StringComparison.OrdinalIgnoreCase))
        //        return Unauthorized(userDetails);

        //    // 4) Preparar payload
        //    var payloadObj = new
        //    {
        //        token =  loginResult.Data,
        //        user = userDetails.Data
        //    };
        //    var plain = System.Text.Json.JsonSerializer.Serialize(payloadObj);

        //    // 5) Encriptar con AES
        //    var keyBytes = Convert.FromBase64String(_configuration["Encryption:Key"]); // 32 bytes base64
        //    using var aes = System.Security.Cryptography.Aes.Create();
        //    aes.Key = keyBytes;
        //    aes.GenerateIV();
        //    using var encryptor = aes.CreateEncryptor();
        //    var plainBytes = Encoding.UTF8.GetBytes(plain);
        //    var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        //    var response = new
        //    {
        //        iv = Convert.ToBase64String(aes.IV),
        //        cipherText = Convert.ToBase64String(cipherBytes)
        //    };

        //    // 6) Notif de éxito
        //    var successNotif = await _catalogoNotificacionService
        //        .GetNotificationByTipoAndFuncionAsync("AUTHC", "LOGIN_EXITOSO");

        //    return Ok(ResponseFromService<object>.Success(response, successNotif));
        //}

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
            return Ok(ResponseFromService<string>.Success(EncryptString(renewResult.Data), successNotif));
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
            if (userResult.Toast.Equals("error", StringComparison.OrdinalIgnoreCase) || userResult.Data == null)
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
            await _authService.RevokeTokenAsync(request.Token, DateTime.UtcNow.AddMinutes(-1));

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
            return Ok(ResponseFromService<string>.Success(EncryptString(result.Data), success));
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
            return Ok(ResponseFromService<string>.Success(EncryptString(result.Data), success));        
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


        //[HttpPost("testsetmail")]
        //public async Task<IActionResult> TestSenderMail(string email)
        //{
        //    await _emailService.SendEmailAsync(email, "Solicitud para restablecer la contraseña", "https:\\www.apiqa.recetamedica.com");
        //    return Ok();

        //}
        private string EncryptString(string plainText)
        {
            var keyBytes = Convert.FromBase64String(_configuration["Encryption:Key"]!.Trim());
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = keyBytes;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // Devolvemos IV + ':' + cipherText, ambos en Base64
            return $"{Convert.ToBase64String(aes.IV)}:{Convert.ToBase64String(cipherBytes)}";
        }

        // Models/EncryptedPayload.cs
        public class EncryptedPayload
        {
            public string iv { get; set; } = null!;
            public string cipherText { get; set; } = null!;
        }
        
        [HttpPost("decrypt-data")]
        [AllowAnonymous]
        public IActionResult DecryptData([FromBody] ResponseFromService<string> encryptedResponse)
        {
            // 1) Separa IV y cipherText
            var parts = encryptedResponse.Data.Split(':', 2);
            var ivBytes = Convert.FromBase64String(parts[0]);
            var cipherBytes = Convert.FromBase64String(parts[1]);

            // 2) Obtén la key desde la configuración
            var keyBase64 = _configuration["Encryption:Key"]!.Trim();
            var keyBytes = Convert.FromBase64String(keyBase64);

            // 3) Desencripta con AES
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            var json = Encoding.UTF8.GetString(plainBytes);
            // 1) opcional: si json representa un objeto, conviértelo en dinámico
            var obj = System.Text.Json.JsonSerializer.Deserialize<object>(json);
            // 2) devuelve ese objeto
            return Ok(obj);
        }

        /// <summary>
        /// Recibe iv y cipherText, desencripta el payload y devuelve el objeto original.
        /// </summary>
        [HttpPost("decrypt-token")]
        [AllowAnonymous]
        public async Task<IActionResult> Decrypt([FromBody] EncryptedPayload payload)
        {
            try
            {
                // 1) Cargar y validar key de configuración
                var keyBase64 = _configuration["Encryption:Key"]?.Trim()
                    ?? throw new ArgumentNullException("Encryption:Key", "Encryption key is missing");
                var keyBytes = Convert.FromBase64String(keyBase64);

                // 2) Decodificar IV y cipherText desde Base64
                var ivBytes = Convert.FromBase64String(payload.iv);
                var cipherBytes = Convert.FromBase64String(payload.cipherText);

                // 3) Desencriptar con AES
                using var aes = Aes.Create();
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                using var decryptor = aes.CreateDecryptor();
                var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                // 4) Obtienes el JWT completo como string
                var jwtString = Encoding.UTF8.GetString(plainBytes);

                // 5) Devuelves un JSON { token: "eyJhbGciOi…" }
                return Ok(new { token = jwtString });
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return BadRequest(ResponseFromService<object>.Exeption(ex, error!));
            }
        }

        [HttpPost("decrypt-json")]
        [AllowAnonymous]
        public IActionResult DecryptJson([FromBody] EncryptedPayload payload)
        {
            try
            {
                // 1) Leer y validar key
                var keyBase64 = _configuration["Encryption:Key"]?.Trim()
                    ?? throw new ArgumentNullException("Encryption:Key", "Encryption key is missing");
                var keyBytes = Convert.FromBase64String(keyBase64);

                // 2) Decodificar IV y cipherText
                var ivBytes = Convert.FromBase64String(payload.iv);
                var cipherBytes = Convert.FromBase64String(payload.cipherText);

                // 3) Desencriptar con AES
                using var aes = Aes.Create();
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                using var decryptor = aes.CreateDecryptor();
                var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                // 4) Obtener el JSON plano
                var json = Encoding.UTF8.GetString(plainBytes);

                // 5) Deserializar a objeto dinámico
                var obj = System.Text.Json.JsonSerializer.Deserialize<object>(json);

                // 6) Devolver directamente el JSON deserializado
                return Ok(obj);
            }
            catch (Exception ex)
            {
                // aquí podrías reutilizar tu catálogo de notificaciones
                return BadRequest(new
                {
                    error = "No se pudo desencriptar/deserializar",
                    details = ex.Message
                });
            }
        }

    }
}
