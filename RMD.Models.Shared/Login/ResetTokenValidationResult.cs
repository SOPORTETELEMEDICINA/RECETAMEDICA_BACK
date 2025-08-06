namespace RMD.Shared.Models.Login
{
    public class ResetTokenValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
