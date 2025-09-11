namespace RMD.Shared.Models.Login.Options
{
    public sealed class TwilioMessagingOptions
    {
        public string AccountSid { get; set; } = "";
        public string AuthToken { get; set; } = "";
        public string MessagingServiceSid { get; set; } = "";
    }
}