namespace RMD.Interface.Auth
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendErrorByEmailAsync(string subject, string errorDetails);
    }
}
