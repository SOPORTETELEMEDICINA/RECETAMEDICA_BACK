using RMD.Shared.Models.Login;
using System.Security.Cryptography;
using System.Text;

namespace RMD.Extensions.System
{
    public class DecryptJwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtKeyHolder _jwtKeyHolder;

        public DecryptJwtMiddleware(RequestDelegate next, JwtKeyHolder jwtKeyHolder)
        {
            _next = next;
            _jwtKeyHolder = jwtKeyHolder;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out var hdr))
            {
                var bearer = hdr.ToString();
                if (bearer.StartsWith("Bearer "))
                {
                    var tokenEnc = bearer["Bearer ".Length..];
                    var parts = tokenEnc.Split(':', 2);
                    if (parts.Length == 2)
                    {
                        try
                        {
                            var ivBytes = Convert.FromBase64String(parts[0]);
                            var cipherBytes = Convert.FromBase64String(parts[1]);

                            using var aes = Aes.Create();
                            aes.Key = _jwtKeyHolder.ResponseKey;
                            aes.IV = ivBytes;

                            using var decryptor = aes.CreateDecryptor();
                            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                            var jwt = Encoding.UTF8.GetString(plainBytes);

                            context.Request.Headers["Authorization"] = "Bearer " + jwt;
                        }
                        catch
                        {
                            // Si falla la desencriptación, no se reemplaza el header y sigue igual
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}