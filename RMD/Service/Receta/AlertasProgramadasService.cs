using RMD.Interface.Receta;
using RMD.Interface.Security;
using RMD.Shared.Models.Receta.AlertaToma.Request;
using RMD.Shared.Models.Receta.AlertaToma.Response;
using System.Data;

namespace RMD.Service.Receta
{
    public class AlertasProgramadasService : IAlertasProgramadasService
    {
        private readonly IDapperService _dapperService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public AlertasProgramadasService(
            IDapperService dapperService,
            ICatalogoNotificacionService catalogoNotificacionService)
        {
            _dapperService = dapperService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        public async Task<ResponseFromService<List<AlertaProgramadaResponse>>> GetAlertasProgramadasAsync(GetAlertasProgramadasRequest request)
        {
            try
            {
                var result = await _dapperService.QueryAsync<AlertaProgramadaResponse>(
                    "[Receta].[GetAlertasPaciente]",
                    new { request.IdPaciente },
                    commandType: CommandType.StoredProcedure
                );

                var list = result.ToList();

                if (!list.Any())
                {
                    var notifEmpty = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTASPROGRAMADAS", "ALERTAS_PROGRAMADAS_NO_ENCONTRADAS");
                    return ResponseFromService<List<AlertaProgramadaResponse>>.Failure(notifEmpty);
                }

                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("ALERTASPROGRAMADAS", "ALERTAS_PROGRAMADAS_ENCONTRADAS");

                return ResponseFromService<List<AlertaProgramadaResponse>>.Success(list, notif);
            }
            catch
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<List<AlertaProgramadaResponse>>.Failure(notif);
            }
        }
        public async Task<ResponseFromService<IEnumerable<AlertaProgramadaResponse>>> GetAlertasProgramadasEnFechaAsync(GetAlertasProgramadasEnFechaRequest request)
        {
            try
            {
                var result = await _dapperService.QueryAsync<AlertaProgramadaResponse>(
                    "[Receta].[GetAlertasPacienteEnFecha]",
                    new
                    {
                        request.IdPaciente,
                        Fecha = request.Fecha.Date
                    },
                    commandType: CommandType.StoredProcedure
                );

                var list = result.ToList();

                if (!list.Any())
                {
                    var notifEmpty = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTASPROGRAMADAS", "ALERTAS_PROGRAMADAS_NO_ENCONTRADAS");
                    return ResponseFromService<IEnumerable<AlertaProgramadaResponse>>.Failure(notifEmpty);
                }

                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("ALERTASPROGRAMADAS", "ALERTAS_PROGRAMADAS_ENCONTRADAS");

                return ResponseFromService<IEnumerable<AlertaProgramadaResponse>>.Success(list, notif);
            }
            catch
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<AlertaProgramadaResponse>>.Failure(notif);
            }
        }

    }
}
