using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Login;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace RMD.Movil.Services.Http;

public class RenewableAuthHandler : DelegatingHandler
{
    private readonly IServiceProvider _services;
    private readonly string[] rutasPublicas =
    {
        "/api/auth/login",
        "/api/auth/renew",
        "/api/auth/forgot-password",
        "/api/auth/reset-password"
    };

    public RenewableAuthHandler(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            // No hacer nada en rutas públicas
            if (rutasPublicas.Any(r => request.RequestUri!.AbsolutePath.ToLower().Contains(r)))
                return await base.SendAsync(request, cancellationToken);

            var json = Preferences.Default.Get<string>("UsuarioGuardado", null);
            if (string.IsNullOrEmpty(json))
                return await HandleUnauthorizedAsync();

            var usuario = JsonSerializer.Deserialize<LoginResult>(json);
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.TokenPlain))
                return await HandleUnauthorizedAsync();

            // Validar si va a expirar
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(usuario.TokenPlain);
            var tiempoRestante = (jwt.ValidTo.ToUniversalTime() - DateTime.UtcNow).TotalMinutes;

            if (tiempoRestante <= 10)
            {
                Console.WriteLine("[AUTH] Token por expirar. Renovando...");

                var authService = _services.GetRequiredService<IAuthControllerService>();
                var renew = await authService.RenewTokenAsync();

                if (renew.Toast?.ToLower() == "success" && renew.Data != null)
                {
                    usuario.TokenEncrypted = renew.Data.TokenEncrypted;
                    usuario.TokenPlain = renew.Data.TokenPlain;
                    Preferences.Default.Set("UsuarioGuardado", JsonSerializer.Serialize(usuario));
                }
                else
                {
                    return await HandleUnauthorizedAsync();
                }
            }

            // Usar el token actualizado
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", usuario.TokenEncrypted);

            return await base.SendAsync(request, cancellationToken);
        }
        catch
        {
            return await HandleUnauthorizedAsync();
        }
    }

    private static Task<HttpResponseMessage> HandleUnauthorizedAsync()
    {
        Console.WriteLine("[AUTH] Sesión no válida. Cerrando sesión.");
        Preferences.Default.Remove("UsuarioGuardado");

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.GoToAsync("///LoginPage");
        });

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized));
    }
}
