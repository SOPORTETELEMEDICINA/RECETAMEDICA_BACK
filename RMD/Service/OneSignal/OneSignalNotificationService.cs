using RMD.Interface.OneSignal;
using RMD.Interface.Security; 
using RMD.Shared.Models.OneSignal.Request;
using RMD.Shared.Models.OneSignal.Response;
using System.Data;
using System.Text;

namespace RMD.Service.OneSignal
{
    public sealed class OneSignalNotificationService : IOneSignalNotificationService
    {
        private readonly HttpClient _http;
        private readonly string _appId;
        private readonly string _restApiKey;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;

        public OneSignalNotificationService(
            HttpClient http,
            IConfiguration configuration,
            IDapperService dapperService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _http = http;
            _appId = configuration.GetValue<string>("OneSignal:AppId")!;
            _restApiKey = configuration.GetValue<string>("OneSignal:RestApiKey")!;
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;
        }

        public async Task<ResponseFromService<bool>> ProgramarRecordatorioTomaAsync(
            List<ProgramarTomaRequest> req, Guid idPaciente,
            CancellationToken ct = default)
        {
            try
            {
                if (idPaciente == Guid.Empty)
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "DATOS_INVALIDOS");
                    return ResponseFromService<bool>.Failure(notif);
                }

                var respuestas = new List<ProgramarTomaResponse>(req.Count);
                int ok = 0, fail = 0;

                foreach (var item in req)
                {
                    var resItem = new ProgramarTomaResponse
                    {
                        IdAlertaToma = item.IdAlertaToma,
                        TipoAlerta = item.TipoAlerta
                    };

                    try
                    {
                        if (string.IsNullOrWhiteSpace(item.Medicamento))
                            throw new ArgumentException("Medicamento inválido.");
                        if (item.TipoAlerta is not (1 or 2))
                            throw new ArgumentException("TipoAlerta debe ser 1 (normal) o 2 (manual).");

                        var sendAtUtc = item.FechaHoraTomaUtc.AddMinutes(-item.MinutosAntes);
                        resItem.SendAfterUtc = sendAtUtc;

                        using var msg = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications");
                        msg.Headers.TryAddWithoutValidation("Authorization", $"Basic {_restApiKey}");

                        var contenido = $"En {item.MinutosAntes} minutos debes tomarte el {item.Medicamento}.";

                        var payload = new
                        {
                            app_id = _appId,
                            include_external_user_ids = new[] { idPaciente.ToString() }, // external_id = IdPaciente
                            target_channel = "push",
                            headings = new { es = "Recordatorio de medicación" },
                            contents = new { es = contenido },
                            send_after = sendAtUtc.ToString("o"), // ISO-8601 (UTC)
                            data = new
                            {
                                idAlertaToma = item.IdAlertaToma,
                                tipoAlerta = item.TipoAlerta, // 1=normal, 2=manual
                                medicamento = item.Medicamento
                            }
                        };

                        var json = JsonSerializer.Serialize(payload);
                        msg.Content = new StringContent(json, Encoding.UTF8, "application/json");

                        var resp = await _http.SendAsync(msg, ct);
                        resItem.HttpStatus = (int)resp.StatusCode;

                        var body = await resp.Content.ReadAsStringAsync(ct);

                        if (!resp.IsSuccessStatusCode)
                        {
                            resItem.Exito = false;
                            resItem.Error = string.IsNullOrWhiteSpace(body)
                                ? $"HTTP {(int)resp.StatusCode}"
                                : $"HTTP {(int)resp.StatusCode}: {body}";
                            fail++;
                        }
                        else
                        {
                            try
                            {
                                using var doc = JsonDocument.Parse(body);
                                if (doc.RootElement.TryGetProperty("id", out var idProp))
                                    resItem.OneSignalNotificationId = idProp.GetString();
                            }
                            catch { /* tolerante */ }

                            resItem.Exito = true;
                            ok++;
                        }
                    }
                    catch (Exception exItem)
                    {
                        resItem.Exito = false;
                        resItem.Error = exItem.Message;
                        resItem.HttpStatus = (int)HttpStatusCode.BadRequest;
                        fail++;
                    }

                    respuestas.Add(resItem);
                }

                await GuardarRespuestasEnBdAsync(respuestas);

                if (ok == 0)
                {
                    var notifError = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "TOMA_NO_PROGRAMADA");
                    notifError.Mensaje = $"{notifError.Mensaje} | Fallidas: {fail}/{req.Count}";
                    return ResponseFromService<bool>.Failure(notifError);
                }

                if (ok == req.Count)
                {
                    var notifOk = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "TOMA_PROGRAMADA");
                    return ResponseFromService<bool>.Success(true, notifOk);
                }
                else
                {
                    var notifParcial = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "TOMA_PROGRAMADA_PARCIAL");
                    notifParcial.Mensaje = $"{notifParcial.Mensaje} | Exitosas: {ok}/{req.Count}, Fallidas: {fail}";
                    return new ResponseFromService<bool>
                    {
                        Code = 200,
                        Message = notifParcial.Mensaje,
                        Toast = ToastType.INFO.ToString(),
                        Data = true
                    };
                }
            }
            catch (Exception ex)
            {
                var notifEx = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXCEPCION");
                notifEx.Mensaje = $"{notifEx.Mensaje} | {ex.Message}";
                return ResponseFromService<bool>.Failure(notifEx);
            }
        }

        private async Task GuardarRespuestasEnBdAsync(List<ProgramarTomaResponse> respuestas)
        {
            if (respuestas.Count == 0) return;

            var rows = respuestas.Select(r => new
            {
                r.IdAlertaToma,
                TipoAlerta = (byte)r.TipoAlerta,
                r.OneSignalNotificationId,
                FechaEnvioProgramadoUtc = r.SendAfterUtc?.UtcDateTime,
                r.HttpStatus,
                r.Error
            });

            string json = JsonSerializer.Serialize(rows);

            try
            {
                await _dapperService.ExecuteAsync(
                    "[Receta].UpdateAlertaProgramadaUpdateOneSignal",
                    new { Json = json },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }


    }
}
