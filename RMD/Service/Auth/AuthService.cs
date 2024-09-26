using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Auth;
using RMD.Interface.Usuarios;
using RMD.Models.Login;
using RMD.Models.Usuarios;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RMD.Service.Auth
{
    public class AuthService(IUsuarioService usuarioService, UsuariosDBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, CifradoHelper cifradoHelper) : IAuthService
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly UsuariosDBContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly CifradoHelper _cifradoHelper = cifradoHelper;

        // Cambia IActionResult por string (o ajusta la interfaz según lo que necesites)
        public async Task<string> LoginAsync(UserCredentials credentials)
        {
            // Validación de credenciales
            var isValid = await _usuarioService.ValidateUserCredentialsAsync(credentials.Usr, credentials.Password);
            if (!isValid)
            {
                return "Credenciales inválidas"; // O lanza una excepción personalizada si lo prefieres
            }

            // Obtener información del usuario
            var usuarioDetalle = await _usuarioService.GetUsuarioByUsernameAsync(credentials.Usr);
            if (usuarioDetalle == null)
            {
                return "Usuario no encontrado"; // O lanza una excepción personalizada
            }

            // Generar token
            var token = GenerateToken(usuarioDetalle);

            return token; // Devuelve el token como string
        }

        public async Task LogoutAsync(string token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:Key is missing in configuration");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };

            var principal = jwtTokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtToken)
            {
                var expiration = jwtToken.ValidTo;
                _context.BlacklistedTokens.Add(new BlacklistedToken
                {
                    Token = token,
                    ExpirationDate = expiration
                });
                await _context.SaveChangesAsync(); // Added 'await' to eliminate warning
            }
        }

        public async Task<string> RenewTokenAsync()
        {
            var username = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "unknown";

            // Obtén el detalle del usuario nuevamente si es necesario
            var usuarioDetalle = await _usuarioService.GetUsuarioByUsernameAsync(username);
            if (usuarioDetalle == null)
            {
                return string.Empty; // Ajustado para que nunca devuelva null
            }

            return GenerateToken(usuarioDetalle);
        }

        private string GenerateToken(UsuarioDetalle usuarioDetalle)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Obtener la clave y otros parámetros del token desde appsettings.json
            var key = _configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:Key is missing in configuration");
            var audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:Audience is missing in configuration");
            var issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:Issuer is missing in configuration");

            // Obtener el tiempo de expiración del token desde appsettings.json (en horas)
            var tokenExpirationHours = int.Parse(_configuration["Jwt:TokenExpirationHours"] ?? "24"); // Por defecto, 24 horas si no está configurado

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, usuarioDetalle.Usr),
                    new Claim(ClaimTypes.Role, usuarioDetalle.TipoUsuario), // Asegúrate de que TipoUsuario es el nombre correcto
                    new Claim("GEMP", usuarioDetalle.IdGEMP?.ToString() ?? string.Empty),  // Convertir Guid? a string
                    new Claim("IdSucursal", usuarioDetalle.IdSucursal?.ToString() ?? string.Empty), // Convertir Guid? a string
                    new Claim("IdUsuario", usuarioDetalle.IdUsuario.ToString()),  // Convertir Guid a string
                    new Claim("IdRol", usuarioDetalle.IdTipoUsuario.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(tokenExpirationHours), // Usar el tiempo configurado en appsettings.json
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public async Task<string> GeneratePasswordResetTokenAsync(Guid userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:Key is missing in configuration");

            // Tiempo de expiración configurable
            var tokenExpirationMinutes = int.Parse(_configuration["Jwt:ResetTokenExpirationMinutes"] ?? "20");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("IdUsuario", userId.ToString()), // Incluir el IdUsuario en el token
                new Claim("GeneratedAt", DateTime.UtcNow.ToString()) // Agregar la fecha de generación
            }),
                Expires = DateTime.UtcNow.AddMinutes(tokenExpirationMinutes), // Token válido por N minutos
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<(bool IsValid, string ErrorMessage, Guid UserId)> ValidateResetTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:Key is missing in configuration");

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true // Esto asegura que el token no haya expirado
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken && jwtToken.ValidTo < DateTime.UtcNow)
                {
                    return (false, "El token ha expirado.", Guid.Empty);
                }

                // Verificar si el token está en la lista negra
                var isTokenBlacklisted = await _context.BlacklistedTokens.AnyAsync(t => t.Token == token);
                if (isTokenBlacklisted)
                {
                    return (false, "El token ya ha sido utilizado.", Guid.Empty);
                }

                // Obtener el IdUsuario del token
                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "IdUsuario");
                if (userIdClaim == null)
                {
                    return (false, "Token inválido.", Guid.Empty);
                }

                return (true, string.Empty, Guid.Parse(userIdClaim.Value));
            }
            catch (SecurityTokenException)
            {
                return (false, "Token inválido o expirado.", Guid.Empty);
            }
        }


        // Método para actualizar la contraseña
        public async Task<bool> UpdatePasswordAsync(Guid userId, string newPassword)
        {
            try
            {
                // Cifrar la nueva contraseña usando BCrypt
                var hashedPassword = _cifradoHelper.HashPassword(newPassword);

                // Llamar al SP que actualiza la contraseña en la base de datos
                await _context.Database.ExecuteSqlRawAsync("EXEC Usuarios_UpdateUserPassword @p0, @p1", userId, hashedPassword);
                return true;
            }
            catch (Exception ex)
            {
                // Loguear el error
                // logger.LogError(ex, "Error al actualizar la contraseña para el usuario {UserId}", userId);
                return false;
            }
        }

        // Método para verificar si un token está activo
        public async Task<bool> IsTokenActiveAsync(string token)
        {
            // Verificar si el token es válido, puedes hacer validaciones adicionales aquí
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
            {
                return false;
            }

            // Aquí puedes agregar lógica adicional para verificar el estado del token
            // Por ejemplo, revisar una lista de tokens revocados en tu base de datos
            // o asegurarte de que no ha expirado
            var expirationDate = jwtToken.ValidTo;
            return expirationDate > DateTime.UtcNow;
        }

        public async Task RevokeTokenAsync(string token, DateTime expirationDate)
        {
            // Añadir el token a la lista negra de tokens (BlacklistedTokens)
            _context.BlacklistedTokens.Add(new BlacklistedToken
            {
                Id = Guid.NewGuid(),  // Genera un nuevo ID único
                Token = token,
                ExpirationDate = expirationDate
            });

            await _context.SaveChangesAsync();
        }
    }
}
