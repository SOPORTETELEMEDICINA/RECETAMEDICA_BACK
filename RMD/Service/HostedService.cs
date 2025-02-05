using RMD.Data;

namespace RMD.Service
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public TokenCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<UsuariosDBContext>();
                        var tokensExpirados = await context.BlacklistedTokens
                            .Where(t => t.ExpirationDate != null && t.ExpirationDate <= DateTime.Now)
                            .ToListAsync();

                        if (tokensExpirados.Any())
                        {
                            context.BlacklistedTokens.RemoveRange(tokensExpirados);
                            await context.SaveChangesAsync();
                        }
                    }

                    // Ejecutar cada 24 horas
                    await Task.Delay(TimeSpan.FromHours(3), stoppingToken);
                }
                catch (Exception ex)
                {
                    // Log del error para depuración
                    Console.WriteLine($"Error en TokenCleanupService: {ex.Message}");
                }
            }
        }

    }

}
