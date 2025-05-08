using MailKit.Net.Smtp;
using MimeKit;
using RMD.Interface.Auth;
using RMD.Interface.Notificaciones;
using RMD.Models.Responses;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace RMD.Service.Auth
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public EmailService(IConfiguration configuration, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _configuration = configuration;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        // Envia un correo electrónico de restablecimiento de contraseña, utilizando respuesta estandarizada.
        public async Task<ResponseFromService<bool>> SendEmailAsync(string toEmail, string subject, string resetLink)
        {
            try
            {
                // Construir mensaje de correo.
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _configuration["SmtpSettings:SenderName"],
                    _configuration["SmtpSettings:SenderEmail"]));
                email.To.Add(new MailboxAddress(string.Empty, toEmail));
                email.Subject = subject;

                // HTML template
                var htmlTemplate = @"
                    <!DOCTYPE html>
                    <html lang='es'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <style>
                            body { font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }
                            .email-container { background-color: #ffffff; margin: 20px auto; padding: 20px; max-width: 600px; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1); }
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
                                <h2>RECUPERACIÓN DE CONTRASEÑA</h2>
                            </div>
                            <div class='email-body'>
                                <p>HOLA,</p>
                                <p>HEMOS RECIBIDO UNA SOLICITUD PARA RESTABLECER LA CONTRASEÑA DE TU CUENTA.</p>
                                <p>HAZ CLIC EN EL SIGUIENTE BOTÓN PARA RESTABLECER TU CONTRASEÑA:</p>
                                <p><a href='{{RESET_LINK}}' class='email-button'>RESTABLECER CONTRASEÑA</a></p>
                                <p>SI NO SOLICITASTE ESTE CAMBIO, PUEDES IGNORAR ESTE MENSAJE.</p>
                            </div>
                            <div class='email-footer'>
                                <p>&COPY; 2024 RECETA MÉDICA DIGITAL. TODOS LOS DERECHOS RESERVADOS.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                // Reemplazar el placeholder por el enlace real.
                var emailBody = htmlTemplate.Replace("{{RESET_LINK}}", resetLink);
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
                            _configuration["SmtpSettings:Server"],
                            int.Parse(_configuration["SmtpSettings:Port"]),
                            MailKit.Security.SecureSocketOptions.StartTls);
                        await smtpClient.AuthenticateAsync(
                            _configuration["SmtpSettings:Username"],
                            _configuration["SmtpSettings:Password"]);
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
                    _configuration["SmtpSettings:SenderName"],
                    _configuration["SmtpSettings:SenderEmail"]));
                // En este ejemplo, el correo destino se fija de forma estática; podría parametrizarse si se desea.
                email.To.Add(new MailboxAddress("", "guz_bonilla@hotmail.com"));
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
                            _configuration["SmtpSettings:Server"],
                            int.Parse(_configuration["SmtpSettings:Port"]),
                            MailKit.Security.SecureSocketOptions.StartTls);
                        await smtpClient.AuthenticateAsync(
                            _configuration["SmtpSettings:Username"],
                            _configuration["SmtpSettings:Password"]);
                        await smtpClient.SendAsync(email);
                        mailSent = true;
                        break;
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
                        catch { }
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
    }
}
