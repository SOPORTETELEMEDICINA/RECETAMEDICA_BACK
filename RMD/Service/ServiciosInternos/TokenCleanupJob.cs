using Microsoft.EntityFrameworkCore;
using RMD.Data;

namespace RMD.Service.ServiciosInternos
{
    public class TokenCleanupJob
    {
        private readonly IServiceProvider _serviceProvider;
        public TokenCleanupJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        //public async Task CleanupAsync()
        //{
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var context = scope.ServiceProvider.GetRequiredService<UsuariosDBContext>();

        //        var tokensExpirados = await context.BlacklistedTokens
        //            .Where(t => t.ExpirationDate != null && t.ExpirationDate <= DateTime.UtcNow)
        //            .ToListAsync();
        //        if (tokensExpirados.Any())
        //        {
        //            context.BlacklistedTokens.RemoveRange(tokensExpirados);
        //            await context.SaveChangesAsync();
        //        }
        //    }
        //}
        public async Task CleanupAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UsuariosDBContext>();

            var now = DateTime.UtcNow;
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
