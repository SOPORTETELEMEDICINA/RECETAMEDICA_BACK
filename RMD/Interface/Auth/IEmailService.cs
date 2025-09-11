namespace RMD.Interface.Auth
{
    public interface IEmailService
    {
        Task<ResponseFromService<bool>> SendEmailAsync(string toEmail, string subject, string resetLink);
        Task<ResponseFromService<bool>> SendErrorByEmailAsync(string subject, string errorDetails);
    }
}
