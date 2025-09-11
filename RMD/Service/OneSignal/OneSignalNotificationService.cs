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

        // NUEVO: país y zona horaria resuelta a partir de appsettings: Country = "MX" | "ES"
        private readonly TimeZoneInfo _tz;

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

            string country = (configuration["Country"] ?? "MX").Trim().ToUpperInvariant();

            // Si quieres permitir override explícito:
            // var tzOverride = configuration["TimeZoneId"]; // opcional
            // _tz = !string.IsNullOrWhiteSpace(tzOverride) ? ResolveTzById(tzOverride) : ResolveTzByCountry(_country);
            _tz = ResolveTzByCountry(country);
        }

        // Resuelve zona horaria por país con fallback Windows/IANA
        private static TimeZoneInfo ResolveTzByCountry(string country)
        {
            // MX → America/Mexico_City (IANA) | Central Standard Time (Mexico) (Windows)
            // ES → Europe/Madrid (IANA)       | Romance Standard Time (Windows)
            return country switch
            {
                "ES" => FirstTzOrThrow(new[]
                {
                    "Europe/Madrid",           // IANA (Linux/Mac)
                    "Romance Standard Time"    // Windows
                }),
                _ => FirstTzOrThrow(new[]
                {
                    "America/Mexico_City",                 // IANA (Linux/Mac)
                    "Central Standard Time (Mexico)",      // Windows (preferida)
                    "Central Standard Time"                // Windows (fallback)
                })
            };
        }

        private static TimeZoneInfo ResolveTzById(string tzId)
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(tzId); }
            catch { throw new TimeZoneNotFoundException($"No se encontró la zona horaria '{tzId}' en el sistema."); }
        }

        private static TimeZoneInfo FirstTzOrThrow(string[] ids)
        {
            foreach (var id in ids)
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
                catch { /* intentar el siguiente */ }
            }
            throw new TimeZoneNotFoundException("No fue posible resolver la zona horaria para la configuración actual.");
        }

        public async Task<ResponseFromService<bool>> ProgramarRecordatorioTomaAsync(
            List<ProgramarTomaRequest> req, CancellationToken ct = default)
        {
            try
            {
                var respuestas = new List<ProgramarTomaResponse>(req.Count);
                int ok = 0, fail = 0;

                foreach (var item in req)
                {
                    var resItem = new ProgramarTomaResponse
                    {
                        IdAlertaTomaProgramada = item.IdAlertaTomaProgramada,
                        TipoAlerta = item.TipoAlerta
                    };

                    try
                    {
                        if (string.IsNullOrWhiteSpace(item.Medicamento))
                            throw new ArgumentException("Medicamento inválido.");
                        if (item.TipoAlerta is not (1 or 2))
                            throw new ArgumentException("TipoAlerta debe ser 1 (normal) o 2 (manual).");

                        // 🔁 LOCAL -> UTC según Country (MX/ES)
                        // Aseguramos Kind=Unspecified para que ConvertTimeToUtc no asuma "Local" del servidor
                        var local = DateTime.SpecifyKind(item.FechaHoraTomaLocal, DateTimeKind.Unspecified);
                        var dtUtc = TimeZoneInfo.ConvertTimeToUtc(local, _tz);
                        var sendAtUtc = new DateTimeOffset(dtUtc).AddMinutes(-item.MinutosAntes);

                        // Clamp mínimo (por si el horario local ya pasó unos segundos)
                        var minUtc = DateTimeOffset.UtcNow.AddSeconds(5);
                        if (sendAtUtc < minUtc) sendAtUtc = minUtc;

                        resItem.SendAfterUtc = sendAtUtc;

                        using var msg = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications");
                        msg.Headers.TryAddWithoutValidation("Authorization", $"Basic {_restApiKey}");

                        var contenido = $"En {item.MinutosAntes} minutos debes tomarte el {item.Medicamento}.";

                        var payload = new
                        {
                            app_id = _appId,
                            include_external_user_ids = new[] { item.IdPaciente.ToString() }, // external_id = IdPaciente
                            target_channel = "push",
                            headings = new { es = "Recordatorio de medicación" },
                            contents = new { es = contenido },
                            send_after = sendAtUtc.ToString("o"), // ISO-8601 (UTC)
                            data = new
                            {
                                idAlertaToma = item.IdAlertaTomaProgramada,
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
                r.IdAlertaTomaProgramada,
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

        public async Task<ResponseFromService<bool>> CancelarProgramadasAsync(
    List<CancelarNotificacionRequest> porCancelar, CancellationToken ct = default)
        {
            try
            {
                if (porCancelar.Count == 0)
                {
                    var notifVacio = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "SIN_NOTIFICACIONES_A_CANCELAR");
                    return ResponseFromService<bool>.Success(true, notifVacio);
                }

                var resultados = new List<(CancelarNotificacionRequest item, int status, string? body, bool exito)>(porCancelar.Count);
                int ok = 0, fail = 0;

                foreach (var item in porCancelar)
                {
                    // Si vino sin ID de OneSignal, no se puede cancelar
                    if (string.IsNullOrWhiteSpace(item.OneSignalNotificationId))
                    {
                        resultados.Add((item, (int)HttpStatusCode.BadRequest, "Falta OneSignalNotificationId", false));
                        fail++;
                        continue;
                    }

                    try
                    {
                        // DELETE /api/v1/notifications/{id}?app_id=...
                        var url = $"https://onesignal.com/api/v1/notifications/{Uri.EscapeDataString(item.OneSignalNotificationId)}?app_id={_appId}";
                        using var reqMsg = new HttpRequestMessage(HttpMethod.Delete, url);
                        reqMsg.Headers.TryAddWithoutValidation("Authorization", $"Basic {_restApiKey}");

                        var resp = await _http.SendAsync(reqMsg, ct);
                        var body = await resp.Content.ReadAsStringAsync(ct);

                        // OneSignal suele responder 200/204 al cancelar; si ya no existe puede ser 404.
                        // Tomamos 200/204 como éxito; 404 lo tratamos como "ya no existe" => éxito suave.
                        bool exito = resp.StatusCode switch
                        {
                            HttpStatusCode.OK => true,
                            HttpStatusCode.NoContent => true,
                            HttpStatusCode.NotFound => true, // ya no existe en OS: objetivo cumplido
                            _ => false
                        };

                        if (exito) ok++; else fail++;
                        resultados.Add((item, (int)resp.StatusCode, string.IsNullOrWhiteSpace(body) ? null : body, exito));
                    }
                    catch (Exception exDel)
                    {
                        resultados.Add((item, (int)HttpStatusCode.InternalServerError, exDel.Message, false));
                        fail++;
                    }
                }

                // (Opcional) Persistir bitácora de cancelaciones en BD
                // Descomenta si tienes un SP tipo [Receta].LogCancelacionOneSignal
                /*
                try
                {
                    var rows = resultados.Select(r => new {
                        r.item.IdAlertaTomaProgramada,
                        r.item.OneSignalNotificationId,
                        r.item.IdPaciente,
                        r.status,
                        Error = r.exito ? null : (r.body ?? "Error al cancelar")
                    });
                    string json = JsonSerializer.Serialize(rows);
                    await _dapperService.ExecuteAsync(
                        "[Receta].LogCancelacionOneSignal",
                        new { Json = json },
                        commandType: CommandType.StoredProcedure
                    );
                }
                catch (Exception e) { Console.Error.WriteLine(e); }
                */

                if (ok == 0)
                {
                    var notifError = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "TOMA_NO_CANCELADA");
                    notifError.Mensaje = $"{notifError.Mensaje} | Fallidas: {fail}/{porCancelar.Count}";
                    return ResponseFromService<bool>.Failure(notifError);
                }

                if (ok == porCancelar.Count)
                {
                    var notifOk = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "TOMA_CANCELADA");
                    return ResponseFromService<bool>.Success(true, notifOk);
                }
                else
                {
                    var notifParcial = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "TOMA_CANCELADA_PARCIAL");
                    notifParcial.Mensaje = $"{notifParcial.Mensaje} | Exitosas: {ok}/{porCancelar.Count}, Fallidas: {fail}";
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

        public async Task<ResponseFromService<bool>> PosponerNotificacionAsync(PosponerNotificacionRequest req)
        {
            try
            {
                // 1) Validaciones básicas
                if (req.SnoozeMinutos <= 0)
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "TOMA_NO_PROGRAMADA");
                    notif.Mensaje = $"{notif.Mensaje} | SnoozeMinutos debe ser > 0.";
                    return ResponseFromService<bool>.Failure(notif);
                }
                if (string.IsNullOrWhiteSpace(req.Medicamento))
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "TOMA_NO_PROGRAMADA");
                    notif.Mensaje = $"{notif.Mensaje} | Medicamento inválido.";
                    return ResponseFromService<bool>.Failure(notif);
                }
                if (req.TipoAlerta is not (1 or 2))
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "TOMA_NO_PROGRAMADA");
                    notif.Mensaje = $"{notif.Mensaje} | TipoAlerta debe ser 1 (normal) o 2 (manual).";
                    return ResponseFromService<bool>.Failure(notif);
                }

                // 2) (Suave) cancelar la notificación anterior si se proporcionó el OS Id
                if (!string.IsNullOrWhiteSpace(req.OneSignalNotificationId))
                {
                    try
                    {
                        // Reutilizamos tu método actual (ignorar resultado si falla)
                        await CancelarProgramadasAsync(new List<CancelarNotificacionRequest>
                {
                    new CancelarNotificacionRequest
                    {
                        IdAlertaTomaProgramada = req.IdAlertaTomaProgramadaBase,
                        OneSignalNotificationId = req.OneSignalNotificationId!,
                        IdPaciente = req.IdPaciente
                    }
                });
                    }
                    catch { /* best-effort: continuamos incluso si falla */ }
                }

                // 3) Calcular el nuevo horario: ahora (local) + SnoozeMinutos
                //    Convertimos a UTC para OneSignal
                var nowUtc = DateTimeOffset.UtcNow;
                var nowLocal = TimeZoneInfo.ConvertTime(nowUtc, _tz).DateTime; // hora local para persistir en columna local
                var fechaTomaLocalNueva = nowLocal.AddMinutes(req.SnoozeMinutos);

                var tomaUnspec = DateTime.SpecifyKind(fechaTomaLocalNueva, DateTimeKind.Unspecified);
                var tomaUtc = TimeZoneInfo.ConvertTimeToUtc(tomaUnspec, _tz);
                var sendAfterUtc = new DateTimeOffset(tomaUtc); // En snooze no restamos MinutosAntes (es “en N minutos”)

                // Pequeño clamp por seguridad
                var minUtc = DateTimeOffset.UtcNow.AddSeconds(5);
                if (sendAfterUtc < minUtc) sendAfterUtc = minUtc;

                // 4) Programar en OneSignal
                using var msg = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications");
                msg.Headers.TryAddWithoutValidation("Authorization", $"Basic {_restApiKey}");

                var contenido = $"Te recordaremos en {req.SnoozeMinutos} minuto(s) tomar {req.Medicamento}.";
                var payload = new
                {
                    app_id = _appId,
                    include_external_user_ids = new[] { req.IdPaciente.ToString() },
                    target_channel = "push",
                    headings = new { es = "Recordatorio de medicación (pospuesto)" },
                    contents = new { es = contenido },
                    send_after = sendAfterUtc.ToString("o"),
                    data = new
                    {
                        idAlertaTomaBase = req.IdAlertaTomaProgramadaBase,
                        tipoAlerta = req.TipoAlerta,
                        medicamento = req.Medicamento,
                        esSnooze = true,
                        snoozeMinutos = req.SnoozeMinutos
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                msg.Content = new StringContent(json, Encoding.UTF8, "application/json");

                string? osId = null;
                int httpStatus;
                string? httpBody;

                using (var resp = await _http.SendAsync(msg))
                {
                    httpStatus = (int)resp.StatusCode;
                    httpBody = await resp.Content.ReadAsStringAsync();

                    if (resp.IsSuccessStatusCode)
                    {
                        try
                        {
                            using var doc = JsonDocument.Parse(httpBody);
                            if (doc.RootElement.TryGetProperty("id", out var idProp))
                                osId = idProp.GetString();
                        }
                        catch { /* tolerante */ }
                    }
                }

                // 5) Persistir la NUEVA fila (snooze) en BD
                //    SP esperado: Receta.InsertSnoozeAlertaProgramada
                //    Parámetros sugeridos:
                //      @IdAlertaTomaProgramadaBase UNIQUEIDENTIFIER,
                //      @IdPaciente UNIQUEIDENTIFIER,
                //      @TipoAlerta TINYINT,
                //      @FechaHoraTomaLocal DATETIME,          -- fecha local nueva
                //      @SnoozeMinutos SMALLINT,
                //      @OneSignalNotificationId NVARCHAR(64) = NULL,
                //      @FechaEnvioProgramadoUtc DATETIMEOFFSET = NULL,
                //      @HttpStatus INT = NULL,
                //      @ErrorMensaje NVARCHAR(MAX) = NULL
                //    (El SP:
                //       - calcula Root/ReprogramadaDe y SnoozeNivel,
                //       - inserta fila con EsSnooze=1, y guarda OS Id/HTTP/Error)
                await _dapperService.ExecuteAsync(
                    "[Receta].[InsertSnoozeAlertaProgramada]",
                    new
                    {
                        req.IdAlertaTomaProgramadaBase,
                        req.IdPaciente,
                        TipoAlerta = (byte)req.TipoAlerta,
                        FechaHoraTomaLocal = fechaTomaLocalNueva,
                        req.SnoozeMinutos,
                        OneSignalNotificationId = osId,
                        FechaEnvioProgramadoUtc = sendAfterUtc.UtcDateTime,
                        HttpStatus = httpStatus,
                        ErrorMensaje = osId is null ? (httpBody) : null
                    },
                    commandType: CommandType.StoredProcedure
                );

                // 6) Armar respuesta con tu catálogo
                if (osId is null)
                {
                    var notifError = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "TOMA_NO_PROGRAMADA");
                    notifError.Mensaje = $"{notifError.Mensaje} | HTTP {httpStatus}: {httpBody}";
                    return ResponseFromService<bool>.Failure(notifError);
                }
                else
                {
                    var notifOk = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "TOMA_PROGRAMADA");
                    notifOk.Mensaje = $"{notifOk.Mensaje} | Pospuesta {req.SnoozeMinutos} min.";
                    return ResponseFromService<bool>.Success(true, notifOk);
                }
            }
            catch (Exception ex)
            {
                var notifEx = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                notifEx.Mensaje = $"{notifEx.Mensaje} | {ex.Message}";
                return ResponseFromService<bool>.Failure(notifEx);
            }
        }
    }
}
