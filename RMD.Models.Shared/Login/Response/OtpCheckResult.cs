namespace RMD.Shared.Models.Login.Response
{
    public sealed class OtpCheckResult
    {
        public string To { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;       // "approved" si ok
        public bool Approved { get; init; }                       // true si Status == approved
        public string ServiceSid { get; init; } = string.Empty;
        public string? Sid { get; init; }                         // SID del check
        public int? ErrorCode { get; init; }
        public string? ErrorMessage { get; init; }
    }
}
