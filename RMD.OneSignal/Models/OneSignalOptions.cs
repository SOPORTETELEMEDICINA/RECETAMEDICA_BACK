namespace RMD.OneSignal.Models;

public sealed class OneSignalOptions
{
    // Requeridos por el SDK y/o REST
    public string AppId { get; set; } = string.Empty;

    // Solo para modo DEV (si llamas directo a OneSignal)
    public string? RestApiKey { get; set; }

    // Si prefieres proxy por backend (PROD), pones la URL aquí
    public string? BackendBaseUrl { get; set; }

    // Zona horaria por defecto para programación
    public string DefaultTimeZone { get; set; } = "America/Mexico_City";

    // ¿Enviar por backend? si true, ignora RestApiKey en la app
    public bool UseBackendProxy { get; set; } = true;
}