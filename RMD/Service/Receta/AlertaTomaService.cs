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
        private readonly IOneSignalNotificationService _oneSignalNotificationService;
        // Constructor
        public AlertaTomaService(IDapperService dapperService, ICatalogoNotificacionService catalogoNotificacionService,
            IPacienteService pacienteService, IOneSignalNotificationService oneSignalNotificationService)
        {
            _dapperService = dapperService;
            _catalogoNotificacionService = catalogoNotificacionService;
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

        public async Task<ResponseFromService<bool>> DesactivarAlertaTomaAsync(
            DesactivarAlertaTomaRequest request, Guid idUsuario)
        {
            try
            {
                var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[DesactivarAlertaToma]",
                    new { request.IdAlertaToma, IdUsuario = idUsuario },
                    commandType: CommandType.StoredProcedure
                );

                // 1) Leer SOLO el código de notificación
                var codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var notifBase = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                // Si el SP devolvió un código de error, regresamos tal cual (sin intentar cancelar en OneSignal)
                if (notifBase.ToastType.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
                    return ResponseFromService<bool>.Failure(notifBase);

                // 2) DESCARTAR el segundo resultset (estatus) sin usarlo
                //    Esto solo avanza el cursor al siguiente resultset.
                try { await multi.ReadAsync<dynamic>(); } catch { /* tolerante si no existe */ }

                // 3) Leer la tabla con las notificaciones a cancelar en OneSignal
                var porCancelar = (await multi.ReadAsync<CancelarNotificacionRequest>()).ToList();

                // Si no hay nada que cancelar → 28606 (sin notificaciones a cancelar)
                if (porCancelar.Count == 0)
                {
                    var notifSin = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "SIN_NOTIFICACIONES_A_CANCELAR");

                    notifSin.Mensaje = $"{notifBase.Mensaje}. {notifSin.Mensaje}";
                    return ResponseFromService<bool>.Success(true, notifSin);
                }

                // Hay notificaciones: cancelar en OneSignal y combinar mensajes
                var resCancel = await _oneSignalNotificationService.CancelarProgramadasAsync(porCancelar);

                // Construimos una respuesta combinada manteniendo el resultado de cancelación
                var combinado = new ResponseFromService<bool>
                {
                    Code = resCancel.Code,
                    Message = $"{notifBase.Mensaje}. {resCancel.Message}",
                    Toast = resCancel.Toast,
                    Data = resCancel.Data // true/false según cancelación (parcial/total/fracaso)
                };

                return combinado;
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


        public async Task<ResponseFromService<bool>> ActivarAlertaManualAsync(
            ActivarAlertaManualRequest request, Guid idUsuario)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
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

                // 1) Primer resultset: código de notificación
                var codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);
                var toast = (notificacion.ToastType ?? string.Empty).ToUpperInvariant();

                // Si el SP regresó ERROR/WARNING, cortar
                if (toast == "ERROR" || toast == "WARNING")
                    return ResponseFromService<bool>.Failure(notificacion);

                // 2) Segundo resultset: tomas del DÍA (ProgramarTomaRequest)
                //    ¡OJO! Con Dapper es ReadAsync<T>(), luego ToList().
                var tomas = (await multi.ReadAsync<ProgramarTomaRequest>()).ToList();

                // Si no hay tomas para hoy (p. ej. @Activo = 0), no hay nada que programar
                if (tomas.Count == 0)
                    return ResponseFromService<bool>.Success(true, notificacion);

                // 3) Programar notificaciones en OneSignal
                //    Asumiendo que tu ProgramarRecordatorioTomaAsync ya acepta la lista con IdPaciente dentro de cada item.
                var resultado = await _oneSignalNotificationService.ProgramarRecordatorioTomaAsync(tomas);

                // 4) Propaga tal cual la respuesta del servicio OneSignal (SUCCESS/INFO/ERROR)
                return resultado;
            }
            catch (Exception ex)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notif);
            }
        }

        public async Task<ResponseFromService<bool>> DesactivarAlertaManualAsync(
      DesactivarAlertaManualRequest request, Guid idUsuario)
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

                // 1) Primer resultset: código de notificación del SP
                var codigoNotificacion = await multi.ReadFirstOrDefaultAsync<int>();

                // 2) Segundo resultset: filas a cancelar en OneSignal
                var porCancelar = (await multi.ReadAsync<CancelarNotificacionRequest>()).ToList();

                // 3) Evaluar la notificación base del SP
                var notifBase = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                // Si el SP regresó ERROR/WARNING, devolvemos tal cual
                var toast = notifBase.ToastType?.ToUpperInvariant();
                if (toast is "ERROR" or "WARNING")
                    return ResponseFromService<bool>.Failure(notifBase);

                // 4) Si no hay nada que cancelar en OneSignal
                if (porCancelar.Count == 0)
                {
                    var notifVacio = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("ALERTAS", "SIN_NOTIFICACIONES_A_CANCELAR");

                    return ResponseFromService<bool>.Success(true, notifVacio);
                }

                // 5) Cancelar en OneSignal (mismo flujo para alerta normal y manual)
                //    No pasamos CancellationToken para evitar abortos por cierre de request/app.
                return await _oneSignalNotificationService.CancelarProgramadasAsync(porCancelar);
            }
            catch
            {
                var notifEx = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Failure(notifEx);
            }
        }
    }
}
