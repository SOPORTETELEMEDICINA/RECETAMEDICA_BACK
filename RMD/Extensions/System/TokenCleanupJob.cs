using Microsoft.EntityFrameworkCore;
using RMD.Data;

namespace RMD.Extensions.System
{
    public class TokenCleanupJob
    {
        private readonly IServiceProvider _serviceProvider;
        public TokenCleanupJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task CleanupAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UsuariosDBContext>();

            var now = DateTime.Now;
            var tokensParaLimpiar = await context.AuthTokens
                .Where(t => t.ExpiresAt <= now || t.RevokedAt != null)
                .ToListAsync();

            if (tokensParaLimpiar.Any())
            {
                context.AuthTokens.RemoveRange(tokensParaLimpiar);
                await context.SaveChangesAsync();
            }
        }

    }
}
