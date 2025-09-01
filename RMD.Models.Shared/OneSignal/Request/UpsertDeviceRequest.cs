namespace RMD.Shared.Models.OneSignal.Request
{
    public sealed class UpsertDeviceRequest
    {
        public string Platform { get; set; } = ""; // "ios" | "android"
        public string? Token { get; set; }         // APNs o FCM
        public string? ExternalId { get; set; }    // idPaciente.ToString(), opcional si aún no hay login
    }
}
