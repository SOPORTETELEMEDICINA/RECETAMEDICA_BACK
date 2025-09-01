namespace RMD.Shared.Models.Login.Response
{
    public sealed class TwilioMessageDto
    {
        public string Sid { get; set; } = "";
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public string Body { get; set; } = "";
        public string Direction { get; set; } = "";
        public string Status { get; set; } = "";
        public int? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateSent { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
