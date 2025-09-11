using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RMD.OneSignal.Models;
using RMD.OneSignal.Service.Interfaces;

namespace RMD.OneSignal.Service.Implementations;

public sealed class NotificationsService : INotificationsService
{
    private readonly OneSignalOptions _opt;
    private readonly HttpClient _http;

    public NotificationsService(OneSignalOptions options, HttpClient httpClient)
    {
        _opt = options;
        _http = httpClient;

        if (!_opt.UseBackendProxy && !string.IsNullOrWhiteSpace(_opt.RestApiKey))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _opt.RestApiKey);
        }
    }

    public Task SendNowAsync(NotificationRequest req)
        => SendInternalAsync(req, schedule: null);

    public async Task ScheduleAsync(NotificationRequest req, ScheduleOptions schedule)
    {
        if (schedule.MultipleTimes != null && schedule.MultipleTimes.Any())
        {
            foreach (var at in schedule.MultipleTimes)
            {
                var sc = new ScheduleOptions { SendAfter = at, TimeZone = schedule.TimeZone };
                await SendInternalAsync(req, sc);
            }
        }
        else
        {
            await SendInternalAsync(req, schedule);
        }
    }

    public async Task SendDailyPackageAsync(IEnumerable<string> externalUserIds,
                                            IEnumerable<(string title, string body, DateTimeOffset at)> alerts,
                                            string? timeZone = null)
    {
        foreach (var uid in externalUserIds)
        {
            foreach (var (title, body, at) in alerts)
            {
                var req = new NotificationRequest
                {
                    ExternalUserId = uid,
                    Title = title,
                    Body = body
                };

                await SendInternalAsync(req, new ScheduleOptions { SendAfter = at, TimeZone = timeZone ?? _opt.DefaultTimeZone });
            }
        }
    }

    // --------------------
    // Private
    // --------------------
    private async Task SendInternalAsync(NotificationRequest req, ScheduleOptions? schedule)
    {
        if (_opt.UseBackendProxy)
        {
            // Modo seguro: llama a tu backend RMD (endpoint que envía a OneSignal)
            if (string.IsNullOrWhiteSpace(_opt.BackendBaseUrl))
                throw new InvalidOperationException("BackendBaseUrl no configurada.");

            var url = $"{_opt.BackendBaseUrl.TrimEnd('/')}/onesignal/send";
            var backendPayload = BuildBackendPayload(req, schedule);
            await PostJsonAsync(url, backendPayload);
            return;
        }

        // Modo DEV: llamada directa a OneSignal
        if (string.IsNullOrWhiteSpace(_opt.RestApiKey))
            throw new InvalidOperationException("RestApiKey no configurada para modo directo.");

        var payload = BuildOneSignalPayload(req, schedule);
        await PostJsonAsync("https://onesignal.com/api/v1/notifications", payload);
    }

    private object BuildBackendPayload(NotificationRequest req, ScheduleOptions? sch)
    {
        return new
        {
            app_id = _opt.AppId,
            to = new[] { req.ExternalUserId },
            title = req.Title,
            body = req.Body,
            withSound = req.WithSound,
            withButtons = req.WithActionButtons,
            send_after = sch?.SendAfter?.ToString("o"),
            timezone = sch?.TimeZone ?? _opt.DefaultTimeZone,
            data = req.Data
        };
    }

    private object BuildOneSignalPayload(NotificationRequest req, ScheduleOptions? sch)
    {
        var payload = new Dictionary<string, object?>
        {
            ["app_id"] = _opt.AppId,
            ["include_external_user_ids"] = new[] { req.ExternalUserId },
            ["channel_for_external_user_ids"] = "push",
            ["headings"] = new { en = req.Title, es = req.Title },
            ["contents"] = new { en = req.Body, es = req.Body },
            ["data"] = req.Data
        };

        if (req.WithSound)
        {
            payload["android_sound"] = "alarma";
            payload["ios_sound"] = "alarma.wav";
        }

        if (req.WithActionButtons)
        {
            payload["action_buttons"] = new[]
            {
                new { id = "taken", text = "✅ Tomado" },
                new { id = "snooze", text = "⏰ Posponer" }
            };
        }

        if (sch?.SendAfter != null)
        {
            // OneSignal acepta ISO 8601 y timezone
            payload["send_after"] = sch.SendAfter.Value.ToString("o");
            payload["delayed_option"] = "timezone";
            payload["delivery_time_of_day"] = sch.SendAfter.Value.ToString("HH:mm");
            payload["timezone"] = sch.TimeZone ?? _opt.DefaultTimeZone;
        }

        return payload;
    }

    private async Task PostJsonAsync(string url, object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await _http.PostAsync(url, content);
        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Error POST {url}: {(int)resp.StatusCode} {resp.ReasonPhrase}. {body}");
        }
    }
}
