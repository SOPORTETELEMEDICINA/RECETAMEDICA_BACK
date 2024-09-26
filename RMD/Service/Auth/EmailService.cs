using MailKit.Net.Smtp;
using MimeKit;
using RMD.Interface.Auth;

namespace RMD.Service.Auth
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string resetLink)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_configuration["SmtpSettings:SenderName"], _configuration["SmtpSettings:SenderEmail"]));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            // Modificar la plantilla para solo mostrar el link en texto
            var htmlTemplate = @"
                <!DOCTYPE html>
                <html lang='es'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <style>
                        body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
                        .email-container { background-color: #ffffff; margin: 20px auto; padding: 20px; max-width: 600px; border-radius: 8px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }
                        .email-header { background-color: #4CAF50; color: #ffffff; padding: 10px 0; text-align: center; border-radius: 8px 8px 0 0; }
                        .email-body { padding: 20px; color: #333333; }
                        .email-body p { line-height: 1.6; }
                        .email-button {
                            display: inline-block;
                            padding: 10px 20px;
                            margin: 20px 0;
                            background-color: #4CAF50;
                            color: #ffffff;
                            text-decoration: none;
                            border-radius: 5px;
                            font-weight: bold;
                        }
                        .email-footer { text-align: center; font-size: 12px; color: #999999; margin-top: 20px; }
                    </style>
                </head>
                <body>
                    <div class='email-container'>
                        <div class='email-header'>
                            <h2>Recuperación de Contraseña</h2>
                        </div>
                        <div class='email-body'>
                            <p>Hola,</p>
                            <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.</p>
                            <p>Haz clic en el siguiente botón para restablecer tu contraseña:</p>
                            <p><a href='{{RESET_LINK}}' class='email-button'>Restablecer Contraseña</a></p>
                            <p>Si no solicitaste este cambio, puedes ignorar este mensaje.</p>
                        </div>
                        <div class='email-footer'>
                            <p>&copy; 2024 Receta Médica Digital. Todos los derechos reservados.</p>
                        </div>
                    </div>
                </body>
                </html>";

            // Reemplazar el marcador {{RESET_LINK}} con el enlace real
            var emailBody = htmlTemplate.Replace("{{RESET_LINK}}", resetLink);

            var bodyBuilder = new BodyBuilder { HtmlBody = emailBody };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtpClient = new SmtpClient();
            var retries = 3;
            var delay = 2000; // Tiempo de espera entre reintentos (milisegundos)

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    await smtpClient.ConnectAsync(_configuration["SmtpSettings:Server"], int.Parse(_configuration["SmtpSettings:Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]);
                    await smtpClient.SendAsync(email);
                    break; // Si se envía correctamente, salir del bucle
                }
                catch (Exception ex)
                {
                    if (i == retries - 1) // Último intento fallido
                    {
                        throw new Exception($"Failed to send email after {retries} attempts: {ex.Message}");
                    }
                    await Task.Delay(delay); // Esperar antes del siguiente intento
                }
                finally
                {
                    await smtpClient.DisconnectAsync(true);
                }
            }
        }
    }



}
