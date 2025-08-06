using Dapper;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Security;
using RMD.Interface.Vidal;
using RMD.Shared.Models.Vidal.Tools;
using RMD.Shared.Models.Vidal.Tools.Alertas;
using System.Text;
using System.Text.RegularExpressions;

namespace RMD.Service.Vidal.Tools
{

    public class ToolsService : IToolsService
    {
        private readonly string _country;
        private readonly string _token;
        private readonly VidalDBContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private bool? _alertServiceAvailable;
        private bool? _docsServiceAvailable;
        private readonly IDapperService _dapperService;

        public ToolsService(IConfiguration configuration,
            ICatalogoNotificacionService catalogoNotificacionService,
            IDapperService dapperService,
            VidalDBContext context)
        {

            _country = configuration["Country"]?.ToLowerInvariant() ?? "mx";
            _token = _country == "mx"
                ? "e20f9d213e7cd76772de1c1794145ba0"
                : "e04ab2608bf655024f85524f1ddb9b16";

            _dapperService = dapperService;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _catalogoNotificacionService = catalogoNotificacionService ?? throw new ArgumentNullException(nameof(catalogoNotificacionService));
        }
        public async Task<ResponseFromService<IEnumerable<DocumentOption>>> GetDocumentListAsync(FichaHtmlRequest peticion)
        {
            try
            {
                if (!await IsVidalServiceDocsActiveAsync())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("VIDALS", "VIDAL_INACTIVO");
                    return ResponseFromService<IEnumerable<DocumentOption>>.Failure(notif);
                }

                var idVmp = await GetIdVmpAsync(peticion.MedicamentoType, peticion.MedicamentoId);
                if (idVmp is null)
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("VIDALS", "IDVMP_NO_ENCONTRADO");
                    return ResponseFromService<IEnumerable<DocumentOption>>.Failure(notif);
                }

                var host = _country == "es" ? "es" : "mx";
                var url = $"https://{host}.vidal-consult.com/get_docs_tool/getDocsList?commonNameGroupId={idVmp}&format=html&tk={_token}";

                using var client = new HttpClient();
                var html = await client.GetStringAsync(url);

                var result = new List<DocumentOption>();
                var liPattern = @"<li><span[^>]*?open_window\('([^']+)'\)[^>]*?>([^<]+)</li>";
                var matches = Regex.Matches(html, liPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);


                foreach (Match match in matches)
                {
                    var fullUrl = match.Groups[1].Value;
                    var name = match.Groups[2].Value;

                    fullUrl = WebUtility.HtmlDecode(fullUrl);
                    var cleanUrl = Regex.Replace(fullUrl, @"[&?]tk=[^&]+", "");
                    var pathOnly = Regex.Replace(cleanUrl, $"^https://{host}\\.vidal-consult\\.com/get_docs_tool/", "", RegexOptions.IgnoreCase);

                    result.Add(new DocumentOption
                    {
                        DocumentName = name.Trim(),
                        Url = pathOnly.Trim()
                    });
                }


                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<IEnumerable<DocumentOption>>.Success(result, success);
            }
            catch (Exception ex)
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<DocumentOption>>.Exeption(ex, notif);
            }
        }

        public async Task<ResponseFromService<List<AlertaJsonHtmlModel>>> GetTipoAlertaJsonAsync(int type)
        {
            try
            {
                if (!await IsVidalServiceAlertsActiveAsync())
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("VIDALS", "VIDAL_NO_DISPONIBLE");
                    return ResponseFromService<List<AlertaJsonHtmlModel>>.Failure(error);
                }

                var alert = await GetTiposDeAlertasAsync();
                var alertname = alert.Data.FirstOrDefault(a => a.Type == type)?.Name;

                if (string.IsNullOrWhiteSpace(alertname))
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("VIDALS", "TIPO_ALERTA_NO_ENCONTRADO");
                    return ResponseFromService<List<AlertaJsonHtmlModel>>.Failure(notif);
                }

                var parametros = new DynamicParameters();
                parametros.Add("@AlertType", alertname);

                var data = await _dapperService.QueryAsync<AlertaJsonHtmlModel>(
                    "Vidal.GetAlertaByIdTipo",
                    parametros
                );

                var listaProcesada = new List<AlertaJsonHtmlModel>();

                foreach (var item in data)
                {
                    using var originalDoc = JsonDocument.Parse(item.Json);
                    using var stream = new MemoryStream();
                    using (var writer = new Utf8JsonWriter(stream))
                    {
                        ModifyEnlaces(originalDoc.RootElement, writer);
                    }

                    var jsonModificado = Encoding.UTF8.GetString(stream.ToArray());

                    listaProcesada.Add(new AlertaJsonHtmlModel
                    {
                        Json = jsonModificado//,
                        //Html = item.Html
                    });
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<List<AlertaJsonHtmlModel>>.Success(listaProcesada, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<List<AlertaJsonHtmlModel>>.Exeption(ex, error);
            }
        }

        private void ModifyEnlaces(JsonElement element, Utf8JsonWriter writer)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                writer.WriteStartObject();
                foreach (var prop in element.EnumerateObject())
                {
                    if ((prop.Name == "enlace" || prop.Name == "enlace_json") && prop.Value.ValueKind == JsonValueKind.String)
                    {
                        var raw = prop.Value.GetString();
                        var cleaned = ExtraerPathDesdeUrl(raw);
                        writer.WriteString(prop.Name, cleaned);
                    }
                    else
                    {
                        writer.WritePropertyName(prop.Name);
                        ModifyEnlaces(prop.Value, writer);
                    }
                }
                writer.WriteEndObject();
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                writer.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                {
                    ModifyEnlaces(item, writer);
                }
                writer.WriteEndArray();
            }
            else
            {
                element.WriteTo(writer);
            }
        }
        private string ExtraerPathDesdeUrl(string url)
        {
            try
            {
                var indexCom = url.IndexOf(".com/", StringComparison.OrdinalIgnoreCase);
                if (indexCom == -1) return url;

                var path = url.Substring(indexCom + 5); // quitar ".com/"

                var indexTk = path.IndexOf("&tk=", StringComparison.OrdinalIgnoreCase);
                if (indexTk == -1)
                    indexTk = path.IndexOf("?tk=", StringComparison.OrdinalIgnoreCase);

                return indexTk >= 0 ? path.Substring(0, indexTk) : path;
            }
            catch
            {
                return url;
            }
        }

        public async Task<ResponseFromService<string>> GetFichaHtmlAsync(string path)
        {
            try
            {
                if (!await IsVidalServiceDocsActiveAsync())
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("VIDALS", "VIDAL_NO_DISPONIBLE");
                    return ResponseFromService<string>.Failure(error);
                }

                var html = await GetFichaHtmlFromPathAsync(path);
                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<string>.Success(html, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
        private async Task<string> GetFichaHtmlFromPathAsync(string urlPath)
        {
            var host = _country == "es" ? "es" : "mx";
            var url = $"https://{host}.vidal-consult.com/get_docs_tool/{urlPath}&tk={_token}";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        public async Task<ResponseFromService<string>> GetContenidoDesdeRutaAsync(string ruta)
        {
            try
            {
                if (!await IsVidalServiceAlertsActiveAsync())
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("VIDALS", "VIDAL_NO_DISPONIBLE");
                    return ResponseFromService<string>.Failure(error);
                }

                var host = _country == "es" ? "es" : "mx";
                var separador = ruta.Contains('?') ? "&" : "?";

                var url = $"https://{host}.vidal-consult.com/{ruta}{separador}tk={_token}";

                using var client = new HttpClient();
                var content = await client.GetStringAsync(url);

                // Validar si es HTML (respuesta inicia con <html o <!DOCTYPE)
                if (content.TrimStart().StartsWith("<", StringComparison.OrdinalIgnoreCase))
                {
                    // Es HTML, se retorna como string plano
                    var notifHtml = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                    return ResponseFromService<string>.Success(content, notifHtml);
                }

                // Si parece ser JSON, validamos que realmente lo sea
                try
                {
                    JsonDocument.Parse(content); // Validación básica
                }
                catch
                {
                    var notifFail = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "RESPUESTA_NO_VALIDA");
                    return ResponseFromService<string>.Failure(notifFail);
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<string>.Success(content, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
        
        public async Task<ResponseFromService<IEnumerable<AlertaOption>>> GetTiposDeAlertasAsync()
        {
            try
            {
                if (!await IsVidalServiceAlertsActiveAsync())
                {
                    var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("VIDALS", "VIDAL_INACTIVO");
                    return ResponseFromService<IEnumerable<AlertaOption>>.Failure(notif);
                }

                var host = _country == "es" ? "es" : "mx";
                var url = $"https://{host}.vidal-consult.com/get_alerts_tool/listAlerts?format=html&tk={_token}";

                using var client = new HttpClient();
                var html = await client.GetStringAsync(url);

                var alertas = new List<AlertaOption>();

                var itemPattern = @"<li>(.*?)</li>";
                var spanPattern = @"<span>\s*(.*?)\s*:\s*</span>\s*<span>\s*(.*?)\s*</span>";

                var items = Regex.Matches(html, itemPattern, RegexOptions.Singleline);

                foreach (Match item in items)
                {
                    var content = item.Groups[1].Value;
                    var spans = Regex.Matches(content, spanPattern);

                    var data = spans.ToDictionary(
                        s => s.Groups[1].Value.Trim().ToLower(),
                        s => s.Groups[2].Value.Trim()
                    );

                    alertas.Add(new AlertaOption
                    {
                        Name = data.GetValueOrDefault("name", ""),
                        Type = int.TryParse(data.GetValueOrDefault("type", "0"), out int type) ? type : 0,
                        Description = data.GetValueOrDefault("description", ""),
                        Active = data.GetValueOrDefault("active", "0") == "1",
                        Provider = data.GetValueOrDefault("provider", "")
                    });
                }

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<IEnumerable<AlertaOption>>.Success(alertas, success);
            }
            catch (Exception ex)
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<AlertaOption>>.Exeption(ex, notif);
            }
        }
       
        private async Task<int?> GetIdVmpAsync(string MedicamentoType, int MedicamentoId)
        {
            switch (MedicamentoType.ToUpper())
            {
                case "VMP":
                    return MedicamentoId;

                case "PRODUCT":
                    return await _context.Productos
                        .Where(p => p.IdProduct == MedicamentoId)
                        .Select(p => (int?)p.IdVmp)
                        .FirstOrDefaultAsync();

                case "PACKAGE":
                    return await (
                        from pack in _context.Packages
                        join prod in _context.Productos on pack.ProductId equals prod.IdProduct
                        where pack.IdPackage == MedicamentoId
                        select (int?)prod.IdVmp
                    ).FirstOrDefaultAsync();

                case "UDCV":
                    return await (
                        from pack in _context.Packages
                        join prod in _context.Productos on pack.ProductId equals prod.IdProduct
                        where pack.UcdvId == MedicamentoId.ToString()
                        select (int?)prod.IdVmp
                    ).FirstOrDefaultAsync();

                default:
                    throw new ArgumentException("MedicamentoType no válido");
            }
        }

        private async Task<bool> IsVidalServiceAlertsActiveAsync()
        {
            if (_alertServiceAvailable.HasValue)
                return _alertServiceAvailable.Value;

            var host = _country == "es" ? "es" : "mx";
            var url = $"https://{host}.vidal-consult.com/get_alerts_tool/service_status?tk={_token}&format=json";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _alertServiceAvailable = false;
                return false;
            }

            var json = await response.Content.ReadAsStringAsync();
            _alertServiceAvailable = json.Contains("\"status\":\"Ok\"", StringComparison.OrdinalIgnoreCase);

            return _alertServiceAvailable.Value;
        }

        private async Task<bool> IsVidalServiceDocsActiveAsync()
        {
            if (_docsServiceAvailable.HasValue)
                return _docsServiceAvailable.Value;

            var host = _country == "es" ? "es" : "mx";
            var url = $"https://{host}.vidal-consult.com/get_docs_tool/service_status?tk={_token}&format=json";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _docsServiceAvailable = false;
                return false;
            }

            var json = await response.Content.ReadAsStringAsync();
            _docsServiceAvailable = json.Contains("\"status\":\"Ok\"", StringComparison.OrdinalIgnoreCase);

            return _docsServiceAvailable.Value;
        }

        public async Task<ResponseFromService<string>> GetNoticiaHtmlById(string type, int idNoticia)
        {
            try
            {
                if (!await IsVidalServiceAlertsActiveAsync())
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("VIDALS", "VIDAL_NO_DISPONIBLE");
                    return ResponseFromService<string>.Failure(error);
                }

                var host = _country == "es" ? "es" : "mx";
                var url = $"https://{host}.vidal-consult.com/get_alerts_tool/getAlerts?type={type}&id={idNoticia}&format=html&tk={_token}";

                using var client = new HttpClient();
                var html = await client.GetStringAsync(url);

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<string>.Success(html, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
        public async Task<ResponseFromService<string>> GetNoticiaRelatedHtmlById(int idNoticia)
        {
            try
            {
                if (!await IsVidalServiceAlertsActiveAsync())
                {
                    var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("VIDALS", "VIDAL_NO_DISPONIBLE");
                    return ResponseFromService<string>.Failure(error);
                }

                var host = _country == "es" ? "es" : "mx";
                var url = $"https://{host}.vidal-consult.com/get_alerts_tool/getRelatedAlerts?id={idNoticia}&format=html&tk={_token}";
               
                using var client = new HttpClient();
                var html = await client.GetStringAsync(url);

                var success = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<string>.Success(html, success);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, error);
            }
        }
     
        public async Task GuardarTodasLasAlertasDesdeVidalAsync()
        {
            var host = _country == "es" ? "es" : "mx";
            if (host == "mx")
                return;

            try
            {
                var tiposAlerta = await GetTiposDeAlertasAsync();
                var alertas = tiposAlerta.Data.Where(x => x.Type != 0).ToList();
                var alertasAcumuladas = new List<AlertaModel>();
                using var http = new HttpClient();
               
                var token = _token;

                foreach (var alertaTipo in alertas)
                {
                    var alertname = alertaTipo.Name;
                    var page = 1;
                    var totalPages = 1;

                    do
                    {
                        var urlJson = $"https://{host}.vidal-consult.com/get_alerts_tool/getAlerts?observ=1&active=1&resolve=0&type={alertname}&format=json&page={page}&size=20&tk={token}";
                        var urlHtml = $"https://{host}.vidal-consult.com/get_alerts_tool/getAlerts?observ=1&active=1&resolve=0&type={alertname}&format=html&page={page}&size=20&tk={token}";

                        var jsonRaw = await http.GetStringAsync(urlJson);
                        var htmlRaw = await http.GetStringAsync(urlHtml);

                        var htmlItems = ExtraerLisDesdeHtml(htmlRaw);

                        using var doc = JsonDocument.Parse(jsonRaw);
                        var root = doc.RootElement;
                        var resultados = root.GetProperty("resultados").EnumerateArray();

                        if (page == 1 && root.TryGetProperty("totalPaginas", out var total))
                            totalPages = total.GetInt32();
                        switch (alertname)
                        {
                            case "pSuministros":
                                alertasAcumuladas.AddRange( GuardarAlertasSuministro(resultados, htmlItems, alertname));
                                break;
                            case "nSeguridad":
                                alertasAcumuladas.AddRange( ProcesarAlertasNSeguridad(resultados, htmlItems, alertname));
                                break;
                            case "vad_noticias":
                            case "vad_alertas":
                            case "vad_pactivo":
                                alertasAcumuladas.AddRange(ProcesarAlertasPorTitulo(resultados, htmlItems, alertname));
                                break;
                                
                        }
                      

                        page++;
                    }
                    while (page <= totalPages);
                }

                var jsonFinal = JsonSerializer.Serialize(alertasAcumuladas);

                var parametros = new DynamicParameters();
                parametros.Add("@JsonAlertas", jsonFinal);

                await _dapperService.ExecuteAsync("Vidal.[InsertAlertas]", parametros);
            }
            catch (Exception ex)
            {
                // Si tienes un logger, úsalo
                //_logger.LogError(ex, "Error en GuardarTodasLasAlertasDesdeVidalAsync");

                throw new Exception("Error al guardar las alertas desde Vidal", ex);
            }

        }
        private List<AlertaModel> GuardarAlertasSuministro(IEnumerable<JsonElement> resultados, List<string> htmlItems, string alertname)
        {
            var lista = new List<AlertaModel>();

            foreach (var alerta in resultados)
            {
                if (!alerta.TryGetProperty("codigoNacional", out var codigoProp) ||
                    !alerta.TryGetProperty("productid", out var productidProp))
                    continue;

                var jsonStr = alerta.GetRawText();
                var codigo = codigoProp.GetString();
                var productid = productidProp.GetString();
                var claveHtml = $"psum_{codigo}_{productid}";

                var li = htmlItems.FirstOrDefault(x => x.Contains($"id='{claveHtml}'")) ?? "";

                lista.Add(new AlertaModel
                {
                    AlertType = alertname,
                    Json = jsonStr,
                    Html = li,
                    ProductId = int.TryParse(productid, out var pid) ? pid : null,
                    CodigoNacional = int.TryParse(codigo, out var cn) ? cn : null
                });
            }

            return lista;
        }
        private List<AlertaModel> ProcesarAlertasNSeguridad(IEnumerable<JsonElement> resultados, List<string> htmlItems, string alertname)
        {
            var lista = new List<AlertaModel>();

            foreach (var alerta in resultados)
            {
                if (!alerta.TryGetProperty("enlace", out var enlaceProp))
                    continue;

                var jsonStr = alerta.GetRawText();
                var enlaceCompleto = enlaceProp.GetString() ?? "";

                // Extraemos solo la URL final
                var urlEsperada = enlaceCompleto.Replace("https://es.vidal-consult.com/get_alerts_tool/getNSecHTML?url=", "");

                var claveHtml = $"id='secnote_{urlEsperada}'";
                var li = htmlItems.FirstOrDefault(x => x.Contains(claveHtml)) ?? "";

                lista.Add(new AlertaModel
                {
                    AlertType = alertname,
                    Json = jsonStr,
                    Html = li
                });
            }

            return lista;
        }
        private List<AlertaModel> ProcesarAlertasPorTitulo(IEnumerable<JsonElement> resultados, List<string> htmlItems, string alertname)
        {
            var lista = new List<AlertaModel>();

            foreach (var alerta in resultados)
            {
                if (!alerta.TryGetProperty("titulo", out var tituloProp))
                    continue;

                var jsonStr = alerta.GetRawText();
                var titulo = WebUtility.HtmlDecode(tituloProp.GetString() ?? "").Trim();

                var li = htmlItems.FirstOrDefault(x =>
                    x.Contains("<div class='til'>") &&
                    x.Contains(titulo, StringComparison.OrdinalIgnoreCase)
                ) ?? "";

                lista.Add(new AlertaModel
                {
                    AlertType = alertname,
                    Json = jsonStr,
                    Html = li
                });
            }

            return lista;
        }



        private List<string> ExtraerLisDesdeHtml(string html)
        {
            var resultado = new List<string>();
            var startTag = "<li";
            var endTag = "</li>";

            int index = 0;
            while ((index = html.IndexOf(startTag, index, StringComparison.Ordinal)) != -1)
            {
                int end = html.IndexOf(endTag, index, StringComparison.Ordinal);
                if (end == -1) break;

                var li = html.Substring(index, end - index + endTag.Length);
                resultado.Add(li);
                index = end + endTag.Length;
            }

            return resultado;
        }


    }
}
