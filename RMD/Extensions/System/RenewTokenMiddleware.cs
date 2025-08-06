using Microsoft.EntityFrameworkCore;
using RMD.Data;

namespace RMD.Extensions.System
{

    public class RenewTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public RenewTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task Invoke(HttpContext context, UsuariosDBContext dbContext)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString().Replace("Bearer ", "");
                var now = DateTime.UtcNow;

                // 1) Buscar el registro en auth.AuthTokens
                var record = await dbContext.AuthTokens
                    .FirstOrDefaultAsync(t => t.Token == token);

                // 2) Si existe y aún no ha expirado, extender el ExpiresAt
                if (record != null && record.ExpiresAt > now)
                {
                    record.ExpiresAt = now.AddHours(1);
                    await dbContext.SaveChangesAsync();
                }
            }

            await _next(context);
        }
    }
}
