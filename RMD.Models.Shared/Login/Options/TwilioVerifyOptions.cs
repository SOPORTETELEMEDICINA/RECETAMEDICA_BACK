namespace RMD.Shared.Models.Login.Options
{
    public sealed class TwilioVerifyOptions
    {
        public string AccountSid { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public string VerifyServiceSid { get; set; } = string.Empty; // VAxxxxxxxx...
    }
}
