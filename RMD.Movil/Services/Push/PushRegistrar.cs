using System.Text;
using System.Text.Json;

namespace RMD.Movil.Services.Push
{
    /// <summary>
    /// Cliente para registrar el token del dispositivo en tu API
    /// y vincular/desvincular el externalId (idPaciente) en OneSignal vía backend.
    /// </summary>
    public static class PushRegistrar
    {
        static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Upsert genérico. Reutilizado por iOS/Android, login/logout.
        /// </summary>
        public static async Task UpsertAsync(string platform, string? token = null, string? externalId = null, CancellationToken ct = default)
        {
            try
            {
                var http = App.Services.GetRequiredService<HttpClient>();

                var body = new
                {
                    platform,  // "ios" | "android"
                    token,     // APNs o FCM; puede ir null si solo quieres vincular/desvincular
                    externalId // idPaciente; null -> backend interpreta desvincular
                };

                var json = JsonSerializer.Serialize(body, JsonOpts);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Ajusta la ruta si tu API usa otro path
                var resp = await http.PostAsync("api/push/upsert", content, ct);
                resp.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Push Upsert error: {ex}");
            }
        }

        // ===== Conveniencias =====

        // iOS: guardar/vincular el token APNs
        public static Task RegisterIosTokenAsync(string apnsToken, CancellationToken ct = default)
            => UpsertAsync("ios", token: apnsToken, externalId: null, ct);

        // Vincular externalId tras login
        public static Task LinkExternalIdAsync(string externalId, CancellationToken ct = default)
        {
#if IOS
            var token = Preferences.Default.Get("APNsToken", string.Empty);
            token = string.IsNullOrWhiteSpace(token) ? null : token;
            return UpsertAsync("ios", token, externalId, ct);
#elif ANDROID
            var token = Preferences.Default.Get("FCMToken", string.Empty);
            token = string.IsNullOrWhiteSpace(token) ? null : token;
            return UpsertAsync("android", token, externalId, ct);
#else
            return UpsertAsync("ios", null, externalId, ct);
#endif
        }

        // Desvincular externalId (logout) — enviamos también el token del device
        public static Task UnlinkExternalIdAsync(string externalId, CancellationToken ct = default)
        {
#if IOS
            var token = Preferences.Default.Get("APNsToken", string.Empty);
            token = string.IsNullOrWhiteSpace(token) ? null : token;
            return UpsertAsync("ios", token: token, externalId: null, ct);
#elif ANDROID
            var token = Preferences.Default.Get("FCMToken", string.Empty);
            token = string.IsNullOrWhiteSpace(token) ? null : token;
            return UpsertAsync("android", token: token, externalId: null, ct);
#else
            return UpsertAsync("ios", token: null, externalId: null, ct);
#endif
        }
    }
}
