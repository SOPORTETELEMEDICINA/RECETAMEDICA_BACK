using Microsoft.IdentityModel.Tokens;
using RMD.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RMD.Middleware
{

    public class RenewTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public RenewTokenMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, UsuariosDBContext dbContext)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                try
                {
                    var principal = jwtTokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                    if (validatedToken is JwtSecurityToken jwtToken)
                    {
                        var expiration = jwtToken.ValidTo;
                        var currentTime = DateTime.UtcNow;

                        if (currentTime > expiration.AddMinutes(-5))
                        {
                            var identity = principal.Identity as ClaimsIdentity;
                            var newExpirationTime = DateTime.UtcNow.AddMinutes(10);

                            var newToken = new JwtSecurityToken(
                                issuer: _configuration["Jwt:Issuer"],
                                audience: _configuration["Jwt:Audience"],
                                claims: identity.Claims,
                                expires: newExpirationTime,
                                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                            );

                            var newTokenString = jwtTokenHandler.WriteToken(newToken);
                            context.Response.Headers.Append("Authorization", "Bearer " + newTokenString);
                        }
                    }
                }
                catch (SecurityTokenException)
                {
                    // Token no válido o expirado
                }
            }

            await _next(context);
        }
    }
}

