namespace RMD.Shared.Models.Login.Response
{
    public sealed class OtpSendResult
    {
        public string To { get; init; } = string.Empty;           // +52...
        public string Channel { get; init; } = "whatsapp";
        public string Status { get; init; } = string.Empty;       // "pending" si ok
        public string ServiceSid { get; init; } = string.Empty;   // VAxxxx
        public string? Sid { get; init; }                         // SID de la verificación
        public int? ErrorCode { get; init; }                      // código Twilio si falla
        public string? ErrorMessage { get; init; }
    }
}
