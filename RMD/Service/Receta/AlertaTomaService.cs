using RMD.Interface.OneSignal;
using RMD.Interface.Pacientes;
using RMD.Interface.Receta;
using RMD.Interface.Security;
using RMD.Shared.Models.Consulta;
using RMD.Shared.Models.OneSignal.Request;
using RMD.Shared.Models.Receta.AlertaToma.Request;
using System.Data;

namespace RMD.Service.Receta
{
    public class AlertaTomaService : IAlertaTomaService
    {

        private readonly IDapperService _dapperService;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IPacienteService _pacienteService;
        private readonly IOneSignalNotificationService _oneSignalNotificationService;
        // Constructor
        public AlertaTomaService(IDapperService dapperService, ICatalogoNotificacionService catalogoNotificacionService,
            IPacienteService pacienteService, IOneSignalNotificationService oneSignalNotificationService)
        {
            _dapperService = dapperService;
            _catalogoNotificacionService = catalogoNotificacionService;
            _pacienteService = pacienteService;
            _oneSignalNotificationService = oneSignalNotificationService;
        }

        public async Task<ResponseFromService<bool>> ActivarAlertaTomaAsync(
        ActivarAlertaTomaRequest request, Guid idUsuario)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
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

                // 1) Primer resultset: código de notificación
                var codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                var toast = (notificacion.ToastType ?? string.Empty).ToUpperInvariant();

                // Si el SP regresó ERROR/WARNING, cortar
                if (toast == "ERROR" || toast == "WARNING")
                    return ResponseFromService<bool>.Failure(notificacion);

                // 2) Segundo resultset: tomas de HOY (ProgramarTomaRequest)
                var tomas = (await multi.ReadAsync<ProgramarTomaRequest>()).ToList();

                // Si no hay tomas para hoy, el SP fue éxito; no hay nada que programar en OneSignal
                if (tomas.Count == 0)
                    return ResponseFromService<bool>.Success(true, notificacion);

                // 4) Programar notificaciones en OneSignal (usa external_user_id = IdPaciente)
                var resultado = await _oneSignalNotificationService.ProgramarRecordatorioTomaAsync(tomas);

                // Propagar tal cual la respuesta del servicio OneSignal (SUCCESS/INFO/ERROR)
                return resultado;
            }
            catch (Exception ex)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notif);
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
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[Get_PackagesByName]",
                    new { Nombre = nombrePackage }, // agrega IdUsuario si tu SP lo requiere: new { Nombre = nombrePackage, IdUsuario = idUsuario }
                    commandType: CommandType.StoredProcedure
                );

                // 1) PRIMER RESULTSET: el código de notificación (int)
                var codigoNotificacion = multi.ReadFirstOrDefault<int>();

                // Busca la notificación por código en tu catálogo
                var notif = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                var toast = (notif.ToastType ?? string.Empty).ToUpperInvariant();

                // Si es ERROR o WARNING -> corta y regresa
                if (toast == "ERROR" || toast == "WARNING")
                    return ResponseFromService<List<Medicamentos>>.Failure(notif);

                // Si es INFO -> regresa éxito con lista vacía (o lo que definas para INFO)
                if (toast == "INFO")
                    return ResponseFromService<List<Medicamentos>>.Success(new List<Medicamentos>(), notif);

                // 2) SEGUNDO RESULTSET: la lista de paquetes
                var paquetes = multi.Read<Medicamentos>().ToList();

                // Si tu SP maneja el "no encontrado" vía código, no hace falta esta verificación.
                // Si quieres doble seguro:
                // if (paquetes.Count == 0) {
                //     var nf = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("ALERTATOMASP", "PACKAGE_NO_ENCONTRADO");
                //     return ResponseFromService<List<Medicamentos>>.Failure(nf);
                // }

                return ResponseFromService<List<Medicamentos>>.Success(paquetes, notif);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<List<Medicamentos>>.Exeption(ex, error);
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
                        request.Activo,
                        IdUsuario = idUsuario
                    },
                    commandType: CommandType.StoredProcedure
                );

                var codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();

                var tomaRequests = await multi.ReadFirstOrDefaultAsync<List<ProgramarTomaRequest>>();

                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                var respPaciente = await _pacienteService.GetIdPacienteByUsuarioAsync(idUsuario);

                Guid idPaciente;
                ResponseFromService<bool> estatus;
                if (respPaciente.Toast is "success" or "info" && respPaciente.Data != Guid.Empty)
                {
                    idPaciente = respPaciente.Data;
                    estatus = await _oneSignalNotificationService.ProgramarRecordatorioTomaAsync(tomaRequests);
                }
                else
                {
                    // Maneja el error/notificación como acostumbras
                    return ResponseFromService<bool>.Failure(new CatalogoNotificacion
                    {
                        CodigoNotificacion = respPaciente.Code,
                        Mensaje = respPaciente.Message,
                        ToastType = respPaciente.Toast.ToUpperInvariant(),
                        Descripcion = string.Join(" | ", respPaciente.Descripcion ?? new List<string>())
                    });
                }

                var paciente = _pacienteService.GetIdPacienteByUsuarioAsync(idUsuario);
               
                return notificacion.ToastType.ToUpperInvariant() is "SUCCESS" or "INFO"
                    ? ResponseFromService<bool>.Success(estatus.Data, notificacion)
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
