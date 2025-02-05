using Microsoft.IdentityModel.Tokens;
using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Auth;
using RMD.Interface.Usuarios;
using RMD.Models.Login;
using RMD.Models.Usuarios;
using System.IdentityModel.Tokens.Jwt;
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

        public async Task<string> LoginAsync(UserCredentials credentials)
        {
            // Validar credenciales del usuario
            var isValid = await _usuarioService.ValidateUserCredentialsAsync(credentials.Usr, credentials.Password);
            if (!isValid)
            {
                return "Credenciales inválidas"; // O lanzar una excepción personalizada
            }

            // Obtener información del usuario
            var usuarioDetalle = await _usuarioService.GetUsuarioByUsernameAsync(credentials.Usr);
            if (usuarioDetalle == null)
            {
                return "Usuario no encontrado"; // O lanzar una excepción personalizada
            }

            // Si el usuario no es SuperAdmin, validar que no tenga sesiones activas
            if (usuarioDetalle.TipoUsuario != "Super Admin")
            {
                var tokenActivo = await _context.BlacklistedTokens
                     .FirstOrDefaultAsync(t => t.IdUsuario == usuarioDetalle.IdUsuario && t.ExpirationDate > DateTime.UtcNow);

                if (tokenActivo != null)
                {
                    // Invalidar el token previo (actualizar su fecha de expiración)
                    tokenActivo.ExpirationDate = DateTime.Now; // Marca el token como inactivo
                    await _context.SaveChangesAsync();
                }
            }

            // Generar un nuevo token
            var nuevoToken = GenerateToken(usuarioDetalle);

            return nuevoToken; // Retornar el nuevo token
        }



        public async Task LogoutAsync(string token)
        {
            // Buscar el token en la tabla
            var tokenEnTabla = await _context.BlacklistedTokens
                .FirstOrDefaultAsync(t => t.Token == token);

            if (tokenEnTabla != null)
            {
                // Eliminar el token de la tabla
                _context.BlacklistedTokens.Remove(tokenEnTabla);
                await _context.SaveChangesAsync();
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
            var tokenExpirationHours = int.Parse(_configuration["Jwt:TokenExpirationHours"] ?? "24");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, usuarioDetalle.Usr),
                    new Claim(ClaimTypes.Role, usuarioDetalle.TipoUsuario),
                    new Claim("GEMP", usuarioDetalle.IdGEMP?.ToString() ?? string.Empty),
                    new Claim("IdSucursal", usuarioDetalle.IdSucursal?.ToString() ?? string.Empty),
                    new Claim("IdUsuario", usuarioDetalle.IdUsuario.ToString()),
                    new Claim("IdRol", usuarioDetalle.IdTipoUsuario.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(tokenExpirationHours),
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            // Crear y escribir el token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Guardar el token en la tabla BlacklistedTokens
            _context.BlacklistedTokens.Add(new BlacklistedToken
            {
                Id = Guid.NewGuid(),
                IdUsuario = usuarioDetalle.IdUsuario, // Relaciona el token con el usuario
                Token = tokenString,
                ExpirationDate = tokenDescriptor.Expires ?? DateTime.Now.AddHours(tokenExpirationHours)
            });

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message;
                throw new Exception($"Error al guardar el token: {ex.Message}. Detalle interno: {innerException}");
            }

            return tokenString;
        }


        public Task<string> GeneratePasswordResetTokenAsync(Guid userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:Key is missing in configuration");

            // Tiempo de expiración configurable
            var tokenExpirationMinutes = int.Parse(_configuration["Jwt:ResetTokenExpirationMinutes"] ?? "20");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                new Claim("IdUsuario", userId.ToString()), // Incluir el IdUsuario en el token
                new Claim("GeneratedAt", DateTime.UtcNow.ToString()) // Agregar la fecha de generación
            ]),
                Expires = DateTime.UtcNow.AddMinutes(tokenExpirationMinutes), // Token válido por N minutos
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token));
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

        public async Task<bool> IsTokenActiveAsync(string token)
        {
            // Buscar el token en la tabla BlacklistedTokens
            var tokenEnTabla = await _context.BlacklistedTokens
                .FirstOrDefaultAsync(t => t.Token == token && t.ExpirationDate > DateTime.UtcNow);

            // Retorna true si el token existe y no ha expirado
            return tokenEnTabla != null;
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

        public async Task LimpiarTokensExpiradosAsync()
        {
            // Buscar todos los tokens cuya fecha de expiración ya ha pasado
            var tokensExpirados = await _context.BlacklistedTokens
                .Where(t => t.ExpirationDate <= DateTime.UtcNow)
                .ToListAsync();

            // Eliminar los tokens encontrados
            if (tokensExpirados.Any())
            {
                _context.BlacklistedTokens.RemoveRange(tokensExpirados);
                await _context.SaveChangesAsync();
            }
        }

    }
}
