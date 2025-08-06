using System.Net.Http.Headers;
using System.Text.Json;
using RMD.Shared.Models.Login;

public class AuthHeaderHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var json = Preferences.Default.Get<string>("UsuarioGuardado", null!);

            if (!string.IsNullOrEmpty(json))
            {
                var loginResult = JsonSerializer.Deserialize<LoginResult>(json);

                if (!string.IsNullOrWhiteSpace(loginResult?.TokenEncrypted))
                {

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.TokenEncrypted);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUTH HEADER] Excepción al cargar token: {ex.Message}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}