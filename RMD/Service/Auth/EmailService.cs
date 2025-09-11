using MailKit.Net.Smtp;
using MimeKit;
using RMD.Interface.Auth;
using System.Security.Cryptography;
using System.Text;

namespace RMD.Service.Auth
{
    public class EmailService : IEmailService
    {
        private readonly byte[] _commonKey;
        private readonly IConfiguration _configuration;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public EmailService(
            byte[] commonKey, 
            IConfiguration configuration, 
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _commonKey = commonKey;
            _configuration = configuration;
            _catalogoNotificacionService = catalogoNotificacionService;
        }
        private string Decrypt(string encrypted)
        {
            var parts = encrypted.Split(':');
            var iv = Convert.FromBase64String(parts[0]);
            var cipherText = Convert.FromBase64String(parts[1]);

            using var aes = Aes.Create();
            aes.Key = _commonKey;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        // Envia un correo electrónico de restablecimiento de contraseña, utilizando respuesta estandarizada.
        public async Task<ResponseFromService<bool>> SendEmailAsync(string toEmail, string subject, string resetLink)
        {
            try
            {
                // Construir mensaje de correo.
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    Decrypt(_configuration["SmtpSettings:SenderName"]),
                    Decrypt(_configuration["SmtpSettings:SenderEmail"])));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                // Reemplazar el placeholder por el enlace real.
                var emailBody = await CargarYReemplazarTemplateAsync("reset_password", new Dictionary<string, string>
                {
                    { "RESET_LINK", resetLink }
                });

                var bodyBuilder = new BodyBuilder { HtmlBody = emailBody };
                email.Body = bodyBuilder.ToMessageBody();

                // Configurar SMTP y enviar el correo con lógica de reintentos.
                using var smtpClient = new SmtpClient();
                int retries = 3, delay = 2000;
                bool mailSent = false;

                for (int i = 0; i < retries; i++)
                {
                    try
                    {
                        // Conexión al servidor SMTP.
                        await smtpClient.ConnectAsync(
                            Decrypt(_configuration["SmtpSettings:Server"]),
                            int.Parse(_configuration["SmtpSettings:Port"]),
                            MailKit.Security.SecureSocketOptions.StartTls);
                        await smtpClient.AuthenticateAsync(
                            Decrypt(_configuration["SmtpSettings:Username"]),
                            Decrypt(_configuration["SmtpSettings:Password"]));
                        await smtpClient.SendAsync(email);
                        mailSent = true;
                        break; // Salir del loop en caso de éxito.
                    }
                    catch (Exception ex)
                    {
                        if (i == retries - 1)
                        {
                            var error = await _catalogoNotificacionService
                                .GetNotificationByTipoAndFuncionAsync("GENERAL", "ERROR_AL_ENVIAR_EMAIL");
                            return ResponseFromService<bool>.Exeption(ex, error);
                        }
                        await Task.Delay(delay);
                    }
                    finally
                    {
                        try
                        {
                            await smtpClient.DisconnectAsync(true);
                        }
                        catch { /* No interrumpir en caso de error al desconectar. */ }
                    }
                }

                if (!mailSent)
                {
                    var error = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "ERROR_AL_ENVIAR_EMAIL");
                    return ResponseFromService<bool>.Failure(error);
                }

                var successNot = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<bool>.Success(true, successNot);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, error);
            }
        }

        // Envia un correo electrónico de error a un administrador, retornando ResponseFromService<bool>.
        public async Task<ResponseFromService<bool>> SendErrorByEmailAsync(string subject, string errorDetails)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    Decrypt(_configuration["SmtpSettings:SenderName"]),
                    Decrypt(_configuration["SmtpSettings:SenderEmail"])));
                email.To.Add(MailboxAddress.Parse("guz_bonilla@hotmail.com"));
                email.Subject = subject;
                var bodyBuilder = new BodyBuilder { TextBody = errorDetails };
                email.Body = bodyBuilder.ToMessageBody();

                using var smtpClient = new SmtpClient();
                int retries = 3, delay = 2000;
                bool mailSent = false;
                for (int i = 0; i < retries; i++)
                {
                    try
                    {
                        await smtpClient.ConnectAsync(
                            Decrypt(_configuration["SmtpSettings:Server"]),
                            int.Parse(_configuration["SmtpSettings:Port"]),
                            MailKit.Security.SecureSocketOptions.StartTls);
                        await smtpClient.AuthenticateAsync(
                            Decrypt(_configuration["SmtpSettings:Username"]),
                            Decrypt(_configuration["SmtpSettings:Password"]));
                        await smtpClient.SendAsync(email);
                        await smtpClient.DisconnectAsync(true);

                        var ok = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                        return ResponseFromService<bool>.Success(true, ok);
                    }
                    catch (Exception ex)
                    {
                        if (i == retries - 1)
                        {
                            var errorNot = await _catalogoNotificacionService
                                .GetNotificationByTipoAndFuncionAsync("GENERAL", "ERROR_AL_ENVIAR_EMAIL");
                            return ResponseFromService<bool>.Exeption(ex, errorNot);
                        }
                        await Task.Delay(delay);
                    }
                    finally
                    {
                        try { await smtpClient.DisconnectAsync(true); }
                        catch
                        {
                            // ignored
                        }
                    }
                }

                if (!mailSent)
                {
                    var errorNot = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("GENERAL", "ERROR_AL_ENVIAR_EMAIL");
                    return ResponseFromService<bool>.Failure(errorNot);
                }

                var successNot = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");
                return ResponseFromService<bool>.Success(true, successNot);
            }
            catch (Exception ex)
            {
                var errorNot = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, errorNot);
            }
        }
        private async Task<string> CargarYReemplazarTemplateAsync(string templateName, Dictionary<string, string> valores)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "extensions", "templates", "emails", $"{templateName}.html");
            var html = await File.ReadAllTextAsync(templatePath);

            foreach (var kvp in valores)
                html = html.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);

            return html;
        }

    }
}
