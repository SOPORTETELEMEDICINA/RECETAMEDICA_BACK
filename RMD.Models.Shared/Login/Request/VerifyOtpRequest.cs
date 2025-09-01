namespace RMD.Shared.Models.Login.Request
{
    public sealed class VerifyOtpRequest
    {
        public string PhoneE164 { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
