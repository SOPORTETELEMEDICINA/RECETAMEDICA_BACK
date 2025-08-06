using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RMD.Data;
using RMD.Interface.Security;

namespace RMD.Service.ServiciosInternos
{
    public class ConfiguracionGlobalService : IConfiguracionGlobalService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly object _lock = new();
        private const string CacheKey = "ConfiguracionGlobal:Data";
        private const string CacheDateKey = "ConfiguracionGlobal:LastUpdated";

        public ConfiguracionGlobalService(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<string> GetValorAsync(string clave)
        {
            await ReloadIfUpdatedAsync();

            if (_cache.TryGetValue<Dictionary<string, string>>(CacheKey, out var config) &&
                config.TryGetValue(clave, out var valor))
            {
                return valor;
            }

            return null!;
        }

        public async Task<int?> GetValorIntAsync(string clave)
        {
            var val = await GetValorAsync(clave);
            return int.TryParse(val, out var result) ? result : null;
        }

        public async Task<bool?> GetValorBoolAsync(string clave)
        {
            var val = await GetValorAsync(clave);
            return bool.TryParse(val, out var result) ? result : null;
        }

        public async Task ReloadIfUpdatedAsync()
        {
            var lastDbUpdate = await _context.ConfiguracionGlobal
                .Where(c => c.Activo == true)
                .MaxAsync(c => c.FechaActualizacion ?? c.FechaCreacion);

            if (!_cache.TryGetValue<DateTime?>(CacheDateKey, out var lastCachedUpdate) || lastDbUpdate > lastCachedUpdate)
            {
                lock (_lock)
                {
                    if (lastDbUpdate > lastCachedUpdate)
                    {
                        var config = _context.ConfiguracionGlobal
                            .Where(c => c.Activo)
                            .ToDictionary(c => c.Clave, c => c.Valor);

                        _cache.Set(CacheKey, config);
                        _cache.Set(CacheDateKey, lastDbUpdate);
                    }
                }
            }
        }
    }

}
