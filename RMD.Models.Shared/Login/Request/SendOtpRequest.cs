namespace RMD.Shared.Models.Login.Request
{
    public sealed class SendOtpRequest
    {
        // Número E.164: +521234567890
        public string PhoneE164 { get; set; } = string.Empty;

        // Opcional: “es”, “en”, etc.
        public string? Locale { get; set; }
    }
}
