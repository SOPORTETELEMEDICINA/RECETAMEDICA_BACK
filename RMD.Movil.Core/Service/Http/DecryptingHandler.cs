
using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Utils.Encryption;
using System.Text.Json;

namespace RMD.Movil.Core.Service.Http
{
    public class DecryptingHandler : DelegatingHandler
    {
        private readonly byte[] _key;

        public DecryptingHandler()
        {
            _key = Convert.FromBase64String("Zjk3KOXcE5PGoOA5FmpG67olfRR7QoBeKjczYGe8bB8=");
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
         {
            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return response;

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            try
            {
                var parsed = JsonSerializer.Deserialize<ResponseFromService<JsonElement>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (parsed != null && parsed.Data.ValueKind == JsonValueKind.String)
                {
                    string encrypted = parsed.Data.GetString()?.Trim() ?? "";

                    if (EsTextoCifradoPosible(encrypted))
                    {
                        var decrypted = DecryptionHelper.DecryptData(encrypted, _key);
                        var parsedJson = JsonDocument.Parse(decrypted).RootElement;
                        parsed.Data = parsedJson;

                        var newJson = JsonSerializer.Serialize(parsed);
                        response.Content = new StringContent(newJson, System.Text.Encoding.UTF8, "application/json");
                    }
                }

                // Si no es string, no hace nada (ejemplo: login con Data = objeto)
            }
            catch
            {
                // Puedes loguear si lo deseas
            }

            return response;
        }

        private static bool EsTextoCifradoPosible(string texto)
        {
            texto = texto.Trim();

            if (texto.StartsWith("<") || texto.StartsWith("&lt;"))
                return false;

            if (texto.StartsWith("{") || texto.StartsWith("["))
                return false;

            if (texto.Length < 16)
                return false;

            if (texto.Contains("<html") || texto.Contains("<div") || texto.Contains("<?xml") || texto.Contains("DOCTYPE"))
                return false;

            return true;
        }
    }
}
