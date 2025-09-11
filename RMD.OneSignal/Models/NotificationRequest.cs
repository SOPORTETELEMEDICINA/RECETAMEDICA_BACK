namespace RMD.OneSignal.Models;

public sealed class NotificationRequest
{
    // Identificador del paciente/usuario (external_user_id en OneSignal)
    public string ExternalUserId { get; set; } = string.Empty;

    // Título y contenido
    public string Title { get; set; } = "Recordatorio de Medicamento";
    public string Body { get; set; } = "Es hora de tomar tu medicamento.";

    // Opcionales
    public bool WithSound { get; set; } = false;
    public bool WithActionButtons { get; set; } = false;

    // Datos extra (para deep-link o tracking)
    public Dictionary<string, string>? Data { get; set; }
}