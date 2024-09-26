using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RMD.Interface.Auth;
using RMD.Interface.Usuarios;
using RMD.Models.Login;
using RMD.Models.Responses;
using System.Net;

namespace RMD.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService, IUsuarioService usuarioService, IConfiguration configuration, IEmailService emailService, ILogger<AuthController> logger) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly IConfiguration _configuration = configuration;
        private readonly IEmailService _emailService = emailService;
        private readonly ILogger<AuthController> _logger = logger;

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
        {
            var token = await _authService.LoginAsync(credentials);
            if (token == null || token == "Credenciales inválidas")
            {
                return Unauthorized(ResponseFromService<string>.Failure(HttpStatusCode.Unauthorized, "Invalid username or password."));
            }

            var userDetails = await _usuarioService.GetUsuarioByUsernameAsync(credentials.Usr);
            if (userDetails == null)
            {
                return Unauthorized(ResponseFromService<string>.Failure(HttpStatusCode.Unauthorized, "User not found."));
            }

            var response = ResponseFromService<object>.Success(new { Token = token, UserDetails = userDetails }, "Login successful.");
            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _authService.LogoutAsync(token);

            var response = ResponseFromService<string>.Success(null, "Logged out successfully.");
            return Ok(response);
        }

        [HttpPost("renew")]
        public async Task<IActionResult> RenewToken()
        {
            var token = await _authService.RenewTokenAsync();
            if (token == null)
            {
                return Unauthorized(ResponseFromService<string>.Failure(HttpStatusCode.Unauthorized, "Unable to renew token."));
            }

            var response = ResponseFromService<string>.Success(token, "Token renewed successfully.");
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] Models.Login.ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Formato de correo inválido."));
            }

            // Validar si el usuario con ese email existe
            var user = await _usuarioService.GetUsuarioByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("No se encontró ningún usuario con el correo: {Email}", request.Email);
                return NotFound(ResponseFromService<string>.Failure(HttpStatusCode.NotFound, "No se encontró ningún usuario con ese correo electrónico."));
            }
            try
            {
                // Generar el token de recuperación de contraseña
                var resetToken = await _authService.GeneratePasswordResetTokenAsync(user.IdUsuario);

                // Diccionario para mapear los entornos con sus URLs correspondientes
                var environmentUrls = new Dictionary<string, string>
                {
                    { "DEV", _configuration["AppSettings:FrontendUrlDEV"] },
                    { "QA", _configuration["AppSettings:FrontendUrlQA"] },
                    { "PROD", _configuration["AppSettings:FrontendUrlPROD"] }
                };

                // Obtener el entorno actual desde appsettings
                string environment = _configuration["Environment"];

                // Validar que el entorno esté configurado correctamente
                if (!environmentUrls.TryGetValue(environment, out string frontendUrl))
                {
                    _logger.LogError("Entorno inválido: {Environment}", environment);
                    return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "Configuración de entorno inválida. Por favor, contacte con el administrador."));
                }

                // Crear el link para la recuperación de contraseña
                var resetLink = $"{frontendUrl}#/authentication/reset/{resetToken}";

                // Enviar el link por correo al usuario
                await _emailService.SendEmailAsync(user.Email, "Solicitud para restablecer la contraseña", resetLink);

                _logger.LogInformation("Se envió un enlace para restablecer la contraseña a: {Email}", user.Email);

                // Respuesta estandarizada en caso de éxito
                var response = ResponseFromService<string>.Success(resetLink, "Se ha enviado un enlace para restablecer su contraseña a su correo electrónico.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la solicitud de recuperación de contraseña para el usuario {Email}", request.Email);
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Ocurrió un error al procesar la solicitud."));
            }         
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] RMD.Models.Login.ResetPasswordRequest request)
        {
            // Validar que la nueva contraseña y la confirmación sean iguales
            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, "La nueva contraseña y la confirmación de contraseña no coinciden."));
            }

            // Verificar si el token es válido y no ha expirado
            var validationResult = await _authService.ValidateResetTokenAsync(request.Token);
            if (!validationResult.IsValid)
            {
                return BadRequest(ResponseFromService<string>.Failure(HttpStatusCode.BadRequest, validationResult.ErrorMessage));
            }

            // Actualizar la contraseña
            var result = await _authService.UpdatePasswordAsync(validationResult.UserId, request.NewPassword);
            if (!result)
            {
                return StatusCode(500, ResponseFromService<string>.Failure(HttpStatusCode.InternalServerError, "Error al actualizar la contraseña."));
            }

            // Invalida el token después de la actualización exitosa de la contraseña
            await _authService.RevokeTokenAsync(request.Token, DateTime.UtcNow.AddMinutes(-1)); // Marca el token como expirado

            // Respuesta exitosa
            return Ok(ResponseFromService<string>.Success(null, "La contraseña ha sido actualizada exitosamente."));
        }

    }
}
