namespace RMD.Shared.Models.Login.Options
{
    public sealed class TwilioVerifyOptions
    {
        public string AccountSid { get; set; } = "";
        public string AuthToken { get; set; } = "";
        public string VerifyServiceSid { get; set; } = "";
        public string WhatsappSenderE164 { get; set; } = ""; // p.ej. +15558398371
    }
}
