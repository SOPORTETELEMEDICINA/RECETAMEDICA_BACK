using RMD.Interface.Security;
using RMD.Interface.Tutores;
using RMD.Shared.Models.Tutores.Request;
using System.Data;

namespace RMD.Service.Tutores
{
    public class TutorService : ITutorService
    {
        private readonly IDapperService _dapperService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public TutorService(IDapperService dapperService, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _dapperService = dapperService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        public async Task<ResponseFromService<string>> AsignarTutorAsync(TutorRequest model, Guid idUsuario)
        {
            try
            {
                var json = JsonSerializer.Serialize(model);

                var multi = await _dapperService.QueryMultipleAsync(
                    "[Tutores].[AsignarTutor]",
                    new { JsonTutor = json, IdUsuario = idUsuario },
                    commandType: CommandType.StoredProcedure
                );

                var codigo = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigo);

                return notificacion.ToastType.ToUpperInvariant() is "SUCCESS" or "INFO"
                    ? ResponseFromService<string>.Success("OK", notificacion)
                    : ResponseFromService<string>.Failure(notificacion);
            }
            catch
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Failure(notif);
            }
        }

        public async Task<ResponseFromService<string>> ActualizarTutorAsync(Guid idTutor, TutorRequest model, Guid idUsuario)
        {
            try
            {
                var json = JsonSerializer.Serialize(model);

                var multi = await _dapperService.QueryMultipleAsync(
                    "[Tutores].[ActualizarTutor]",
                    new { IdTutor = idTutor, JsonTutor = json, IdUsuario = idUsuario },
                    commandType: CommandType.StoredProcedure
                );

                var codigo = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigo);

                return notificacion.ToastType.ToUpperInvariant() is "SUCCESS" or "INFO"
                    ? ResponseFromService<string>.Success("OK", notificacion)
                    : ResponseFromService<string>.Failure(notificacion);
            }
            catch
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Failure(notif);
            }
        }


        public async Task<ResponseFromService<string>> EliminarTutorAsync(Guid idTutor)
        {
            try
            {
                var multi = await _dapperService.QueryMultipleAsync(
                    "[Tutores].[EliminarTutor]",
                    new { IdTutor = idTutor },
                    commandType: CommandType.StoredProcedure
                );

                var codigo = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigo);

                return notificacion.ToastType.ToUpperInvariant() is "SUCCESS" or "INFO"
                    ? ResponseFromService<string>.Success("OK", notificacion)
                    : ResponseFromService<string>.Failure(notificacion);
            }
            catch
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Failure(notif);
            }
        }

        public async Task<ResponseFromService<object>> GetTutorPorPacienteAsync(Guid idPaciente)
        {
            try
            {
                var multi = await _dapperService.QueryMultipleAsync(
                    "[Tutores].[ObtenerTutorPorPaciente]",
                    new { IdPaciente = idPaciente },
                    commandType: CommandType.StoredProcedure
                );

                var codigo = await multi.ReadFirstOrDefaultAsync<int>();
                var tutor = await multi.ReadFirstOrDefaultAsync<object>(); // reemplazá con modelo fuerte si lo tenés

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigo);

                return notificacion.ToastType.ToUpperInvariant() is "SUCCESS" or "INFO"
                    ? ResponseFromService<object>.Success(tutor, notificacion)
                    : ResponseFromService<object>.Failure(notificacion);
            }
            catch
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<object>.Failure(notif);
            }
        }
    }
}
