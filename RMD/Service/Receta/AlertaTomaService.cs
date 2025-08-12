using RMD.Interface.Receta;
using RMD.Interface.Security;
using RMD.Shared.Models.Consulta;
using RMD.Shared.Models.Receta.AlertaToma.Request;
using System.Data;

namespace RMD.Service.Receta
{
    public class AlertaTomaService : IAlertaTomaService
    {

        private readonly IDapperService _dapperService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;

        public AlertaTomaService(IDapperService dapperService, ICatalogoNotificacionService catalogoNotificacionService)
        {
            _dapperService = dapperService;
            _catalogoNotificacionService = catalogoNotificacionService;
        }

        public async Task<ResponseFromService<bool>> ActivarAlertaTomaAsync(ActivarAlertaTomaRequest request, Guid idUsuario)
        {
            try
            {
                var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[ActivarAlertaToma]",
                    new
                    {
                        request.IdReceta,
                        request.IdDetalleReceta,
                        request.MedicamentoId,
                        request.MedicamentoType,
                        request.FechaHoraPrimerToma,
                        IdUsuario = idUsuario
                    },
                    commandType: CommandType.StoredProcedure);

                var codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var estatus = await multi.ReadFirstOrDefaultAsync<bool>();

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                return notificacion.ToastType.ToUpperInvariant() == "SUCCESS" || notificacion.ToastType.ToUpperInvariant() == "INFO"
                    ? ResponseFromService<bool>.Success(estatus,notificacion )
                    : ResponseFromService<bool>.Failure(notificacion);
            }
            catch (Exception)
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Failure(notificacion);
            }
        }

        public async Task<ResponseFromService<bool>> DesactivarAlertaTomaAsync(DesactivarAlertaTomaRequest request, Guid idUsuario)
        {
            try
            {
                var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[DesactivarAlertaToma]",
                    new
                    {
                        request.IdAlertaToma,
                        IdUsuario = idUsuario
                    },
                    commandType: CommandType.StoredProcedure
                );

                var codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var estatus = await multi.ReadFirstOrDefaultAsync<bool>();

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                return notificacion.ToastType.ToUpperInvariant() is "SUCCESS" or "INFO"
                    ? ResponseFromService<bool>.Success(estatus, notificacion)
                    : ResponseFromService<bool>.Failure(notificacion);
            }
            catch
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Failure(notificacion);
            }
        }
        public async Task<ResponseFromService<List<Medicamentos>>> BuscarPackagePorNombreAsync(string nombrePackage, Guid idUsuario)
        {
            try
            {
                IEnumerable<Medicamentos> resultados = await _dapperService.QueryAsync<Medicamentos>(
                    "[Receta].[Get_PackagesByName]",
                    new { Nombre = nombrePackage },
                    commandType: CommandType.StoredProcedure
                );

                IEnumerable<Medicamentos> medicamentosEnumerable = resultados.ToList();
                if (!medicamentosEnumerable.Any())
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTATOMASP", "PACKAGE_NO_ENCONTRADO");
                    return ResponseFromService<List<Medicamentos>>.Failure(notif);
                }

                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "CONSULTA_EXISTOSA");

                return ResponseFromService<List<Medicamentos>>.Success(medicamentosEnumerable.ToList(), notificacion);
            }
            catch
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<List<Medicamentos>>.Failure(notif);
            }
        }

        public async Task<ResponseFromService<bool>> ActivarAlertaManualAsync(ActivarAlertaManualRequest request, Guid idUsuario)
        {
            try
            {
                var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[ActivarAlertaTomaManual]",
                    new
                    {
                        request.MedicamentoId,
                        request.MedicamentoType,
                        request.CantidadDiaria,
                        request.UnidadDispensacionId,
                        request.IdRutaAdministracion,
                        request.Duracion,
                        request.UnidadDuracion,
                        request.FechaHoraPrimerToma,
                        request.Frecuency,
                        request.IdFrecuencyType,
                        request.Notas,
                        IdUsuario = idUsuario
                    },
                    commandType: CommandType.StoredProcedure
                );

                var codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var estatus = await multi.ReadFirstOrDefaultAsync<bool>();

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                return notificacion.ToastType.ToUpperInvariant() is "SUCCESS" or "INFO"
                    ? ResponseFromService<bool>.Success(estatus, notificacion)
                    : ResponseFromService<bool>.Failure(notificacion);
            }
            catch
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Failure(notificacion);
            }
        }


        public async Task<ResponseFromService<bool>> DesactivarAlertaManualAsync(DesactivarAlertaManualRequest request, Guid idUsuario)
        {
            try
            {
                var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[DesactivarAlertaTomaManual]",
                    new
                    {
                        IdAlertaToma = request.IdAlertaTomaManual,
                        IdUsuario = idUsuario
                    },
                    commandType: CommandType.StoredProcedure
                );

                var codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var estatus = await multi.ReadFirstOrDefaultAsync<bool>();

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                return notificacion.ToastType.ToUpperInvariant() is "SUCCESS" or "INFO"
                    ? ResponseFromService<bool>.Success(estatus, notificacion)
                    : ResponseFromService<bool>.Failure(notificacion);
            }
            catch
            {
                var notificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Failure(notificacion);
            }
        }

    }
}
