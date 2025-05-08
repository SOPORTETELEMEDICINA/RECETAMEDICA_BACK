using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using RMD.Data;
using RMD.Data.Models;
using RMD.Interface.Auth;
using RMD.Interface.Notificaciones;
using RMD.Interface.Usuarios;
using RMD.Models.Login;
using RMD.Models.Responses;
using RMD.Models.Usuarios;

namespace RMD.Service.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioService _usuarioService;
        private readonly UsuariosDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CifradoHelper _cifradoHelper;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;

        public AuthService(
            IUsuarioService usuarioService,
            UsuariosDBContext context,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            CifradoHelper cifradoHelper,
            ICatalogoNotificacionService catalogoNotificacionService,
            IMemoryCache cache,
            IHttpClientFactory httpClientFactory)
        {
            _usuarioService = usuarioService;
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _cifradoHelper = cifradoHelper;
            _catalogoNotificacionService = catalogoNotificacionService;
            _cache = cache;
            _httpClient = httpClientFactory.CreateClient("WhatsAppBusiness");
        }

        // Inicia sesión y retorna un token nuevo envuelto en ResponseFromService<string>
        public async Task<ResponseFromService<string>> LoginAsync(UserCredentials credentials)
        {
            try
            {
                // Validar las credenciales del usuario.

                var validateResponse = await _usuarioService.ValidateUserCredentialsAsync(credentials.Usr, credentials.Password);
                if (validateResponse == null || !validateResponse.Data)
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                    return ResponseFromService<string>.Failure(error);
                }
                ResponseFromService<UsuarioDetalle> usuarioResponse = new();
                if (credentials.Plataform == "MOVIL") 
                {
                    usuarioResponse = await _usuarioService.GetUsuarioPacienteByUsernameAsync(credentials.Usr);
                    if (usuarioResponse == null || usuarioResponse.Data == null)
                    {
                        var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("USUARIOSSP", "USUARIONOEXISTE");
                        return ResponseFromService<string>.Failure(error);
                    }
                }
                else if(credentials.Plataform == "WEB")
                {
                    // Obtener los detalles del usuario.
                    usuarioResponse = await _usuarioService.GetUsuarioByUsernameAsync(credentials.Usr);
                    if (usuarioResponse == null || usuarioResponse.Data == null)
                    {
                        var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("USUARIOSSP", "USUARIONOEXISTE");
                        return ResponseFromService<string>.Failure(error);
                    }
                }
                

                //// Si el usuario no es Super Admin, invalidar cualquier token activo existente.
                //if (usuarioResponse.Data.TipoUsuario != "Super Admin")
                //{
                //    var tokenActivo = await _context.BlacklistedTokens
                //        .FirstOrDefaultAsync(t => t.IdUsuario == usuarioResponse.Data.IdUsuario && t.ExpirationDate > DateTime.UtcNow);
                //    if (tokenActivo != null)
                //    {
                //        tokenActivo.ExpirationDate = DateTime.UtcNow;
                //        await _context.SaveChangesAsync();
                //    }
                //}
                // Si el usuario no es Super Admin, revocar cualquier token activo existente.
                if (usuarioResponse.Data.TipoUsuario != "Super Admin")
                {
                    var now = DateTime.UtcNow;
                    var activo = await _context.AuthTokens
                        .Where(t => t.IdUsuario == usuarioResponse.Data.IdUsuario
                                    && t.ExpiresAt > now
                                    && t.RevokedAt == null)
                        .FirstOrDefaultAsync();

                    if (activo != null)
                    {
                        activo.RevokedAt = now;
                        await _context.SaveChangesAsync();
                    }
                }

                // Generar un nuevo token usando el método que ya retorna ResponseFromService<string>.
                var tokenResponse = await GenerateTokenAsync(usuarioResponse.Data);
               
                return tokenResponse;
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        // Cierra la sesión invalidando el token recibido.
        public async Task<ResponseFromService<bool>> LogoutAsync(string token)
        {
            try
            {
                var now = DateTime.UtcNow;

                // 1) Buscar el token en auth.AuthTokens
                var tokenEnTabla = await _context.AuthTokens
                    .FirstOrDefaultAsync(t => t.Token == token);

                // 2) Si existe, marcarlo como revocado
                if (tokenEnTabla != null)
                {
                    tokenEnTabla.RevokedAt = now;
                    await _context.SaveChangesAsync();
                }

                // 3) Notificación de éxito AuthC/LOGOUT_EXITOSO
                var success = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("AUTHC", "LOGOUT_EXITOSO");

                return ResponseFromService<bool>.Success(true, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<string>> RenewTokenAsync()
        {
            try
            {
                // 1) Extraer usuario del token actual
                var username = _httpContextAccessor.HttpContext?.User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    var err = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
                    return ResponseFromService<string>.Failure(err);
                }

                // 2) Obtener UsuarioDetalle completo desde BD
                var usuarioResponse = await _usuarioService.GetUsuarioByUsernameAsync(username);
                if (usuarioResponse?.Data == null)
                {
                    var err = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("USUARIOSSP", "USUARIONOEXISTE");
                    return ResponseFromService<string>.Failure(err);
                }

                // 3) (Opcional) Revocar el token viejo
                var currentToken = _httpContextAccessor.HttpContext.Request
                    .Headers["Authorization"].ToString().Replace("Bearer ", "");
                var viejo = await _context.AuthTokens
                    .FirstOrDefaultAsync(t => t.Token == currentToken && t.RevokedAt == null);
                if (viejo != null)
                {
                    viejo.RevokedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                // 4) Generar y devolver un nuevo JWT
                return await GenerateTokenAsync(usuarioResponse.Data);
            }
            catch (Exception ex)
            {
                var err = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, err);
            }
        }

        // Actualiza la contraseña; en caso de error, se retorna el fallo estandarizado.
        public async Task<ResponseFromService<bool>> UpdatePasswordAsync(Guid userId, string newPassword)
        {
            try
            {
                var hashedPassword = _cifradoHelper.HashPassword(newPassword);
                await _context.Database.ExecuteSqlRawAsync("EXEC Usuarios_UpdateUserPassword @p0, @p1", userId, hashedPassword);
                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<bool>.Success(true, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }

        // Verifica si el token está activo (no expirado y no revocado).
        public async Task<ResponseFromService<bool>> IsTokenActiveAsync(string token)
        {
            try
            {
                var now = DateTime.UtcNow;
                var tokenEnTabla = await _context.AuthTokens
                    .FirstOrDefaultAsync(t =>
                        t.Token == token &&
                        t.ExpiresAt > now &&
                        t.RevokedAt == null);

                var success = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");

                return ResponseFromService<bool>.Success(tokenEnTabla != null, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }

        // Invalida (revoca) un token estableciendo su fecha de expiración.
        public async Task<ResponseFromService<bool>> RevokeTokenAsync(string token, DateTime expirationDate)
        {
            try
            {
                // Buscar el token en auth.AuthTokens
                var tokenEntry = await _context.AuthTokens
                    .FirstOrDefaultAsync(t => t.Token == token);

                if (tokenEntry != null)
                {
                    // Marcar como revocado
                    tokenEntry.RevokedAt = DateTime.UtcNow;
                }
                else
                {
                    // Si no existe (caso raro), lo insertamos con RevokedAt
                    _context.AuthTokens.Add(new AuthToken
                    {
                        IdToken = Guid.NewGuid(),
                        Token = token,
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = expirationDate,
                        RevokedAt = DateTime.UtcNow,
                        IdUsuario = Guid.Empty,    // o el IdUsuario que tengas a mano
                        LastAuth2F = null
                    });
                }

                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");

                return ResponseFromService<bool>.Success(true, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }

        // Limpia los tokens expirados y retorna true en caso de éxito.
        public async Task<ResponseFromService<bool>> LimpiarTokensExpiradosAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                var expirados = await _context.AuthTokens
                    .Where(t => t.ExpiresAt <= now || t.RevokedAt != null)
                    .ToListAsync();

                if (expirados.Any())
                {
                    _context.AuthTokens.RemoveRange(expirados);
                    await _context.SaveChangesAsync();
                }

                var success = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXITOSA");

                return ResponseFromService<bool>.Success(true, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }

        private async Task<ResponseFromService<string>> GenerateTokenAsync(UsuarioDetalle usuarioDetalle)
        {
            try
            {
                // 1) Generar JWT con todos los claims
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
                var issuer = _configuration["Jwt:Issuer"]!;
                var aud = _configuration["Jwt:Audience"]!;
                var horas = int.Parse(_configuration["Jwt:TokenExpirationHours"] ?? "1");

                var desc = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, usuarioDetalle.Usr),
                        new Claim(ClaimTypes.Role, usuarioDetalle.TipoUsuario),
                        new Claim("GEMP", usuarioDetalle.IdGEMP?.ToString() ?? ""),
                        new Claim("IdSucursal", usuarioDetalle.IdSucursal?.ToString() ?? ""),
                        new Claim("IdUsuario", usuarioDetalle.IdUsuario.ToString()),
                        new Claim("IdRol", usuarioDetalle.IdTipoUsuario.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(horas),
                    Issuer = issuer,
                    Audience = aud,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                };

                var token = handler.CreateToken(desc);
                var tokenString = handler.WriteToken(token);

                // 2) (Opcional) Blacklist
                _context.AuthTokens.Add(new AuthToken
                {
                    IdToken = Guid.NewGuid(),
                    IdUsuario = usuarioDetalle.IdUsuario,
                    Token = tokenString,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = desc.Expires!.Value,
                    RevokedAt = null,
                    LastAuth2F = null
                });
                await _context.SaveChangesAsync();

                // 3) Notificación de éxito AUTHC/LOGIN_EXITOSO
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("AUTHC", "LOGIN_EXITOSO");

                // 4) Sólo retornamos el token
                return ResponseFromService<string>.Success(tokenString, notif);
            }
            catch (Exception ex)
            {
                var err = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, err);
            }
        }

        public async Task<ResponseFromService<string>> GeneratePasswordResetTokenAsync(Guid userId)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = _configuration["Jwt:Key"] 
                    ?? throw new ArgumentNullException(nameof(_configuration), "JWT:KEY is missing in configuration");

                var minutos = int.Parse(_configuration["Jwt:ResetTokenExpirationMinutes"] ?? "20");

                var desc = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("IdUsuario",  userId.ToString()),
                        new Claim("GeneratedAt", DateTime.UtcNow.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(minutos),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = handler.CreateToken(desc);
                var tokenString = handler.WriteToken(token);

                // ← Aquí guardamos el reset-token en auth.AuthTokens
                _context.AuthTokens.Add(new AuthToken
                {
                    IdToken = Guid.NewGuid(),
                    IdUsuario = userId,
                    Token = tokenString,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = desc.Expires!.Value,
                    RevokedAt = null,
                    LastAuth2F = null
                });
                await _context.SaveChangesAsync();

                var success = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXITOSA");
                return ResponseFromService<string>.Success(tokenString, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }

        public async Task<ResponseFromService<ResetTokenValidationResult>> ValidateResetTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = _configuration["Jwt:Key"]
                          ?? throw new ArgumentNullException(nameof(_configuration), "JWT:KEY is missing in configuration");

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // 1) Validar firma, expiración, etc.
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // 2) Revisar en la tabla auth.AuthTokens si está revocado
                bool isRevoked = await _context.AuthTokens
                    .AnyAsync(t => t.Token == token && t.RevokedAt != null);
                if (isRevoked)
                {
                    var err = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                    return ResponseFromService<ResetTokenValidationResult>.Failure(err);
                }

                // 3) Extraer claim IdUsuario
                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "IdUsuario");
                if (userIdClaim == null)
                {
                    var err = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                    return ResponseFromService<ResetTokenValidationResult>.Failure(err);
                }

                var validationResult = new ResetTokenValidationResult
                {
                    IsValid = true,
                    ErrorMessage = string.Empty,
                    UserId = Guid.Parse(userIdClaim.Value)
                };

                var success = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXITOSA");
                return ResponseFromService<ResetTokenValidationResult>.Success(validationResult, success);
            }
            catch (SecurityTokenExpiredException)
            {
                var err = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<ResetTokenValidationResult>.Failure(err);
            }
            catch (SecurityTokenException ex)
            {
                var err = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<ResetTokenValidationResult>.Exeption(ex, err);
            }
            catch (Exception ex)
            {
                var err = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<ResetTokenValidationResult>.Exeption(ex, err);
            }
        }

        // AuthService.cs (añadir métodos al final de la clase)
        public async Task<ResponseFromService<string>> ChangeSucursalAsync(Guid newSucursalId)
        {
            try
            {
                // 1) Extraer token actual
                var http = _httpContextAccessor.HttpContext!;
                var oldToken = http.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(oldToken))
                    return ResponseFromService<string>.Failure(
                        await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO"));

                // 2) Validar token y extraer claims
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(oldToken);
                var claims = jwt.Claims
                    .Where(c => c.Type != "IdSucursal")
                    .ToList();
                claims.Add(new Claim("IdSucursal", newSucursalId.ToString()));

                // 3) Generar nuevo JWT con mismas configuraciones
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
                var issuer = _configuration["Jwt:Issuer"]!;
                var aud = _configuration["Jwt:Audience"]!;
                var horas = int.Parse(_configuration["Jwt:TokenExpirationHours"] ?? "1");
                var desc = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(horas),
                    Issuer = issuer,
                    Audience = aud,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                };
                var newToken = handler.WriteToken(handler.CreateToken(desc));

                // 4) Actualizar la tabla auth.AuthTokens
                var record = await _context.AuthTokens.FirstOrDefaultAsync(t => t.Token == oldToken);
                if (record != null)
                {
                    record.Token = newToken;
                    record.ExpiresAt = desc.Expires!.Value;
                    record.RevokedAt = null;
                    await _context.SaveChangesAsync();
                }

                // 5) Retornar nuevo token
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("AUTHC", "RENEW_EXITOSO");
                return ResponseFromService<string>.Success(newToken, notif);
            }
            catch (Exception ex)
            {
                var err = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, err);
            }
        }

        public async Task<ResponseFromService<string>> ChangeGempAsync(Guid newGempId)
        {
            try
            {
                var http = _httpContextAccessor.HttpContext!;
                var oldToken = http.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (string.IsNullOrEmpty(oldToken))
                    return ResponseFromService<string>.Failure(
                        await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO"));

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(oldToken);
                var claims = jwt.Claims
                    .Where(c => c.Type != "GEMP")
                    .ToList();
                claims.Add(new Claim("GEMP", newGempId.ToString()));

                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
                var issuer = _configuration["Jwt:Issuer"]!;
                var aud = _configuration["Jwt:Audience"]!;
                var horas = int.Parse(_configuration["Jwt:TokenExpirationHours"] ?? "1");
                var desc = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(horas),
                    Issuer = issuer,
                    Audience = aud,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                };
                var newToken = handler.WriteToken(handler.CreateToken(desc));

                var record = await _context.AuthTokens.FirstOrDefaultAsync(t => t.Token == oldToken);
                if (record != null)
                {
                    record.Token = newToken;
                    record.ExpiresAt = desc.Expires!.Value;
                    record.RevokedAt = null;
                    await _context.SaveChangesAsync();
                }

                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("AUTHS", "RENEW_EXITOSO");
                return ResponseFromService<string>.Success(newToken, notif);
            }
            catch (Exception ex)
            {
                var err = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, err);
            }
        }

        // AuthService.cs (añadir métodos)

        //public async Task<ResponseFromService<bool>> Send2FACodeAsync()
        //{
        //    try
        //    {
        //        // 1) Extraer IdUsuario y teléfono
        //        var uidClaim = _httpContextAccessor.HttpContext!.User.FindFirst("IdUsuario")?.Value;
        //        if (!Guid.TryParse(uidClaim, out var userId))
        //            throw new Exception("Token inválido");

        //        var userResp = await _usuarioService.GetUsuarioByIdAsync(userId);
        //        var phone = userResp.Data?.Telefono;
        //        if (string.IsNullOrWhiteSpace(phone))
        //            throw new Exception("Teléfono no disponible");

        //        // 2) Generar código
        //        var code = new Random().Next(100_000, 999_999).ToString();
        //        _cache.Set($"2fa_{userId}", code, TimeSpan.FromMinutes(5));

        //        // 3) Llamar WhatsApp Business API
        //        var waPayload = new
        //        {
        //            messaging_product = "whatsapp",
        //            to = phone,
        //            type = "text",
        //            text = new { body = $"Tu código de autenticación es: {code}" }
        //        };
        //        var response = await _httpClient.PostAsJsonAsync("/v14.0/" +
        //            _configuration["WhatsApp:PhoneNumberId"] + "/messages", waPayload);

        //        response.EnsureSuccessStatusCode();

        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
        //        return ResponseFromService<bool>.Success(true, notif);
        //    }
        //    catch (Exception ex)
        //    {
        //        var err = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
        //        return ResponseFromService<bool>.Exeption(ex, err);
        //    }
        //}

        //public async Task<ResponseFromService<bool>> Validate2FACodeAsync(string code)
        //{
        //    try
        //    {
        //        // 1) Extraer IdUsuario
        //        var uidClaim = _httpContextAccessor.HttpContext!.User.FindFirst("IdUsuario")?.Value;
        //        if (!Guid.TryParse(uidClaim, out var userId))
        //            throw new Exception("Token inválido");

        //        // 2) Recuperar código de cache
        //        if (!_cache.TryGetValue($"2fa_{userId}", out string? expected) || expected != code)
        //        {
        //            var err = await _catalogoNotificacionService
        //                .GetNotificationByTipoAndFuncionAsync("GENERAL", "PASSWORD_INVALIDO");
        //            return ResponseFromService<bool>.Failure(err);
        //        }

        //        // 3) Actualizar LastAuth2F en AuthTokens
        //        var jwt = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //        var record = await _context.AuthTokens.FirstOrDefaultAsync(t => t.Token == jwt);
        //        if (record != null)
        //        {
        //            record.LastAuth2F = DateTime.UtcNow;
        //            await _context.SaveChangesAsync();
        //        }

        //        _cache.Remove($"2fa_{userId}");

        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
        //        return ResponseFromService<bool>.Success(true, notif);
        //    }
        //    catch (Exception ex)
        //    {
        //        var err = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
        //        return ResponseFromService<bool>.Exeption(ex, err);
        //    }
        //}
    }

    public class ResetTokenValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}






//// Cierra la sesión invalidando el token recibido.
//public async Task<ResponseFromService<bool>> LogoutAsync(string token)
//{
//    try
//    {
//        var tokenEnTabla = await _context.BlacklistedTokens
//            .FirstOrDefaultAsync(t => t.Token == token);
//        if (tokenEnTabla != null)
//        {
//            _context.BlacklistedTokens.Remove(tokenEnTabla);
//            await _context.SaveChangesAsync();
//        }
//        var success = await _catalogoNotificacionService
//            .GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
//        return ResponseFromService<bool>.Success(true, success);
//    }
//    catch (Exception ex)
//    {
//        var error = await _catalogoNotificacionService
//            .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
//        return ResponseFromService<bool>.Exeption(ex, error);
//    }
//}


// Renueva el token para el usuario autenticado
//public async Task<ResponseFromService<string>> RenewTokenAsync()
//{
//    try
//    {
//        var username = _httpContextAccessor.HttpContext?.User.Identity?.Name;
//        if (string.IsNullOrEmpty(username))
//        {
//            var error = await _catalogoNotificacionService
//                .GetNotificationByTipoAndFuncionAsync("GENERAL", "TOKEN_INVALIDO");
//            return ResponseFromService<string>.Failure(error);
//        }

//        var usuarioResponse = await _usuarioService.GetUsuarioByUsernameAsync(username);
//        if (usuarioResponse == null || usuarioResponse.Data == null)
//        {
//            var error = await _catalogoNotificacionService
//                .GetNotificationByTipoAndFuncionAsync("USUARIOSSP", "USUARIONOEXISTE");
//            return ResponseFromService<string>.Failure(error);
//        }

//        // Generar un nuevo token usando el método que retorna ResponseFromService<string>.
//        var tokenResponse = await GenerateTokenAsync(usuarioResponse.Data);
//        // Se retorna directamente el tokenResponse, que ya contiene éxito o fallo según corresponda.
//        return tokenResponse;
//    }
//    catch (Exception ex)
//    {
//        var error = await _catalogoNotificacionService
//            .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
//        return ResponseFromService<string>.Exeption(ex, error);
//    }
//}


//public async Task<ResponseFromService<bool>> IsTokenActiveAsync(string token)
//{
//    try
//    {
//        var tokenEnTabla = await _context.BlacklistedTokens
//            .FirstOrDefaultAsync(t => t.Token == token && t.ExpirationDate > DateTime.UtcNow);
//        var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
//        return ResponseFromService<bool>.Success(tokenEnTabla != null, success);
//    }
//    catch (Exception ex)
//    {
//        var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
//        return ResponseFromService<bool>.Exeption(ex, error);
//    }
//}


//public async Task<ResponseFromService<bool>> RevokeTokenAsync(string token, DateTime expirationDate)
//{
//    try
//    {
//        _context.BlacklistedTokens.Add(new BlacklistedToken
//        {
//            Id = Guid.NewGuid(),
//            Token = token,
//            ExpirationDate = expirationDate
//        });
//        await _context.SaveChangesAsync();
//        var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
//        return ResponseFromService<bool>.Success(true, success);
//    }
//    catch (Exception ex)
//    {
//        var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
//        return ResponseFromService<bool>.Exeption(ex, error);
//    }
//}


//public async Task<ResponseFromService<bool>> LimpiarTokensExpiradosAsync()
//{
//    try
//    {
//        var tokensExpirados = await _context.BlacklistedTokens
//            .Where(t => t.ExpirationDate <= DateTime.UtcNow)
//            .ToListAsync();
//        if (tokensExpirados.Any())
//        {
//            _context.BlacklistedTokens.RemoveRange(tokensExpirados);
//            await _context.SaveChangesAsync();
//        }
//        var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
//        return ResponseFromService<bool>.Success(true, success);
//    }
//    catch (Exception ex)
//    {
//        var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
//        return ResponseFromService<bool>.Exeption(ex, error);
//    }
//}


// Método privado estandarizado para generar el token y guardar el token en la BD.
//private async Task<ResponseFromService<string>> GenerateTokenAsync(UsuarioDetalle usuarioDetalle)
//{
//    try
//    {
//        var tokenHandler = new JwtSecurityTokenHandler();
//        var key = _configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:Key is missing in configuration");
//        var audience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:Audience is missing in configuration");
//        var issuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(_configuration), "Jwt:Issuer is missing in configuration");
//        var tokenExpirationHours = int.Parse(_configuration["Jwt:TokenExpirationHours"] ?? "1");

//        var tokenDescriptor = new SecurityTokenDescriptor
//        {
//            Subject = new ClaimsIdentity(new[]
//            {
//                new Claim(ClaimTypes.Name, usuarioDetalle.Usr),
//                new Claim(ClaimTypes.Role, usuarioDetalle.TipoUsuario),
//                new Claim("GEMP", usuarioDetalle.IdGEMP?.ToString() ?? string.Empty),
//                new Claim("IdSucursal", usuarioDetalle.IdSucursal?.ToString() ?? string.Empty),
//                new Claim("IdUsuario", usuarioDetalle.IdUsuario.ToString()),
//                new Claim("IdRol", usuarioDetalle.IdTipoUsuario.ToString())
//            }),
//            Expires = DateTime.UtcNow.AddHours(tokenExpirationHours),
//            Audience = audience,
//            Issuer = issuer,
//            SigningCredentials = new SigningCredentials(
//                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
//                SecurityAlgorithms.HmacSha256Signature)
//        };

//        var token = tokenHandler.CreateToken(tokenDescriptor);
//        var tokenString = tokenHandler.WriteToken(token);

//        _context.BlacklistedTokens.Add(new BlacklistedToken
//        {
//            Id = Guid.NewGuid(),
//            IdUsuario = usuarioDetalle.IdUsuario,
//            Token = tokenString,
//            ExpirationDate = tokenDescriptor.Expires ?? DateTime.UtcNow.AddHours(tokenExpirationHours)
//        });

//        await _context.SaveChangesAsync();

//        var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
//        return ResponseFromService<string>.Success(tokenString, success);
//    }
//    catch (Exception ex)
//    {
//        var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
//        return ResponseFromService<string>.Exeption(ex, error);
//    }
//}

// Retorna un token de reseteo de contraseña estandarizado.
//public async Task<ResponseFromService<string>> GeneratePasswordResetTokenAsync(Guid userId)
//{
//    try
//    {
//        var tokenHandler = new JwtSecurityTokenHandler();
//        var key = _configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(_configuration), "JWT:KEY is missing in configuration");
//        var tokenExpirationMinutes = int.Parse(_configuration["Jwt:ResetTokenExpirationMinutes"] ?? "20");

//        var tokenDescriptor = new SecurityTokenDescriptor
//        {
//            Subject = new ClaimsIdentity(new[]
//            {
//                new Claim("IdUsuario", userId.ToString()),
//                new Claim("GeneratedAt", DateTime.UtcNow.ToString())
//            }),
//            Expires = DateTime.Now.AddMinutes(tokenExpirationMinutes),
//            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
//        };

//        var token = tokenHandler.CreateToken(tokenDescriptor);
//        var tokenString = tokenHandler.WriteToken(token);

//        var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
//        return ResponseFromService<string>.Success(tokenString, success);
//    }
//    catch (Exception ex)
//    {
//        var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
//        return ResponseFromService<string>.Exeption(ex, error);
//    }
//}

// Valida el token de reseteo y devuelve un objeto ResetTokenValidationResult estandarizado.
//public async Task<ResponseFromService<ResetTokenValidationResult>> ValidateResetTokenAsync(string token)
//{
//    try
//    {
//        var tokenHandler = new JwtSecurityTokenHandler();
//        var key = _configuration["Jwt:Key"] ?? throw new ArgumentNullException(nameof(_configuration), "JWT:KEY is missing in configuration");

//        var tokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true
//        };

//        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

//        // Si el token ya expiró
//        if (validatedToken is JwtSecurityToken jwtToken && jwtToken.ValidTo < DateTime.UtcNow)
//        {
//            var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
//            var result = new ResetTokenValidationResult { IsValid = false, ErrorMessage = error.Mensaje, UserId = Guid.Empty };
//            return ResponseFromService<ResetTokenValidationResult>.Failure(error);
//        }

//        // Si el token ya está en la lista negra
//        bool isTokenBlacklisted = await _context.BlacklistedTokens.AnyAsync(t => t.Token == token);
//        if (isTokenBlacklisted)
//        {
//            var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
//            var result = new ResetTokenValidationResult { IsValid = false, ErrorMessage = error.Mensaje, UserId = Guid.Empty };
//            return ResponseFromService<ResetTokenValidationResult>.Failure(error);
//        }

//        var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "IdUsuario");
//        if (userIdClaim == null)
//        {
//            var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
//            var result = new ResetTokenValidationResult { IsValid = false, ErrorMessage = error.Mensaje, UserId = Guid.Empty };
//            return ResponseFromService<ResetTokenValidationResult>.Failure(error);
//        }

//        var validationResult = new ResetTokenValidationResult
//        {
//            IsValid = true,
//            ErrorMessage = string.Empty,
//            UserId = Guid.Parse(userIdClaim.Value)
//        };

//        var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
//        return ResponseFromService<ResetTokenValidationResult>.Success(validationResult, success);
//    }
//    catch (SecurityTokenException ste)
//    {
//        var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
//        return ResponseFromService<ResetTokenValidationResult>.Exeption(ste, error);
//    }
//    catch (Exception ex)
//    {
//        var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
//        return ResponseFromService<ResetTokenValidationResult>.Exeption(ex, error);
//    }
//}
