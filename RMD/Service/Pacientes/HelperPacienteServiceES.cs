using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Pacientes;
using RMD.Shared.Models.Pacientes;

namespace RMD.Service.Pacientes
{
    public class HelperPacienteServiceES(PacientesDbContext context,
           ICatalogoNotificacionService catalogoNotificacionService) : IHelperPacienteServiceES
    {
        private readonly PacientesDbContext _context = context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService = catalogoNotificacionService;

        public async Task<ResponseFromService<string>> GenerarQRParaPacienteAsync(Guid idPaciente)
        {
            try
            {
                var tokenExistente = await _context.TokensQRPacientes
                    .Where(t =>
                        t.IdPaciente == idPaciente &&
                        !t.Usado &&
                        t.FechaExpiracion > DateTime.Now)
                    .OrderByDescending(t => t.FechaExpiracion)
                    .FirstOrDefaultAsync();
                TokensQRPaciente tokensQRPaciente;
                if (tokenExistente != null)
                {
                    tokensQRPaciente = await ActualizarExpiracionTokenExistente(tokenExistente);
                }
                else
                {
                    tokensQRPaciente = await CrearNuevoTokenParaPacienteAsync(idPaciente);
                }
                var textoEncriptado = GenerarTextoEncriptadoParaQR(idPaciente);

                var tokenLiga = GenerarTextoEncriptadoParaQR(textoEncriptado, tokensQRPaciente.Token);
                var liga = $"https://puntoventa.recetamedica.digital/repository?{tokenLiga}";
                // TO DO: lógica para generar QR y registrar token
                var notificacion = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("PACIENTES", "QR_GENERADO");

                var qrBase64 = QRGenerator.GenerarQR(liga);

                return ResponseFromService<string>.Success(qrBase64, notificacion);

            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");

                return ResponseFromService<string>.Exeption(ex, notificacion);
            }
        }
        private string GenerarTextoEncriptadoParaQR(string idPaciente, string token)
        {
            var textoPlano = $"{idPaciente}|{token}";
            var textoEncriptado = EncryptionHelper.Encrypt(textoPlano);
            return textoEncriptado;
        }
        private string GenerarTextoEncriptadoParaQR(Guid idPaciente)
        {
            var textoPlano = idPaciente.ToString();
            var textoEncriptado = EncryptionHelper.Encrypt(textoPlano);
            return textoEncriptado;
        }
        private async Task<TokensQRPaciente> ActualizarExpiracionTokenExistente(TokensQRPaciente token)
        {
            token.FechaExpiracion = DateTime.Now.AddHours(36);
            _context.TokensQRPacientes.Update(token);
            await _context.SaveChangesAsync();
            return token;
        }
        private async Task<TokensQRPaciente> CrearNuevoTokenParaPacienteAsync(Guid idPaciente)
        {
            var nuevoToken = new TokensQRPaciente
            {
                IdToken = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString("N"), // También podrías usar un JWT u otro string seguro
                IdPaciente = idPaciente,
                FechaGeneracion = DateTime.Now,
                FechaExpiracion = DateTime.Now.AddHours(36),
                Usado = false
            };

            _context.TokensQRPacientes.Add(nuevoToken);
            await _context.SaveChangesAsync();

            return nuevoToken;
        }
    }
}
