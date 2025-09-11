using Microsoft.Data.SqlClient;
using RMD.Interface.Receta;
using RMD.Interface.Security;
using RMD.Shared.Models.Receta.Detalle.Request;
using RMD.Shared.Models.Receta.Detalle.Response;
using System.Data;

namespace RMD.Service.Receta
{
    public class DetalleRecetaService : IDetalleRecetaService
    {
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;

        public DetalleRecetaService(
            ICatalogoNotificacionService catalogoNotificacionService,
            IDapperService dapperService)
        {
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;
        }

        public async Task<ResponseFromService<string>> CreateUpdateReaccionAsync(DetalleRequest request, Guid idUsuario)
        {
            try
            {
                var parameters = new
                {
                    request.IdDetalleReceta,
                    request.IdReceta,
                    request.MedicamentoId,
                    request.MedicamentoType,
                    request.Descripcion,
                    IdUsuario = idUsuario
                };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[CreateUpdatePacienteReaccion]",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<string>.Success(string.Empty, notificacion);

                return ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var errNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(sqlEx, errNotificacion);
            }
            catch (Exception ex)
            {
                var errNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, errNotificacion);
            }
        }
        public async Task<ResponseFromService<string>> DeleteReaccionAsync(DetalleRequest request, Guid idUsuario)
        {
            try
            {
                var parameters = new
                {
                    request.IdDetalleReceta,
                    request.IdReceta,
                    request.MedicamentoId,
                    request.MedicamentoType,
                    request.Descripcion,
                    IdUsuario = idUsuario
                };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[DeletePacienteReaccion]",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<string>.Success(string.Empty, notificacion);

                return ResponseFromService<string>.Success(notificacion.Descripcion, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var errNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(sqlEx, errNotificacion);
            }
            catch (Exception ex)
            {
                var errNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<string>.Exeption(ex, errNotificacion);
            }
        }
        public async Task<ResponseFromService<List<DetalleResponse>>> GetDetallesByIdReceta(Guid idUsuario, Guid idReceta)
        {
            try
            {
                var parameters = new
                {
                    IdUsuario = idUsuario,
                    IdReceta = idReceta
                };

                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[GetDetallesByIdReceta]",
                    parameters
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<List<DetalleResponse>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<List<DetalleResponse>>.Success(new List<DetalleResponse>(), notificacion);

                var detalles = multi.Read<DetalleResponse>().ToList();

                if (detalles.Count == 0)
                    return ResponseFromService<List<DetalleResponse>>.Failure(notificacion);

                return ResponseFromService<List<DetalleResponse>>.Success(detalles, notificacion);
            }
            catch (Exception ex)
            {
                var errNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<List<DetalleResponse>>.Exeption(ex, errNotificacion);
            }
        } 
        public async Task<ResponseFromService<IEnumerable<MedicamentoActivoResponse>>> GetSoloMedicamentosActivos(Guid idUsuario)
        {
            try
            {
                // 1️⃣ Obtener IdPaciente desde la tabla Pacientes
                var idPaciente = await _dapperService.QueryFirstOrDefaultAsync<Guid?>(
                    "SELECT IdPaciente FROM Pacientes WITH (NOLOCK) WHERE IdUsuario = @IdUsuario",
                    new { IdUsuario = idUsuario },
                    CommandType.Text
                );

                if (!idPaciente.HasValue)
                {
                    var notificacion = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("RECETASSP", "PACIENTE_NO_ENCONTRADO");
                    return ResponseFromService<IEnumerable<MedicamentoActivoResponse>>.Failure(notificacion);
                }

                // 2️⃣ Ejecutar el SP para obtener los medicamentos
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[GetMedicamentoActivoByPaciente]",
                    new { IdPaciente = idPaciente.Value }
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacionSP = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacionSP.ToastType.ToUpperInvariant() == "ERROR" || notificacionSP.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<MedicamentoActivoResponse>>.Failure(notificacionSP);

                if (notificacionSP.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<MedicamentoActivoResponse>>.Success(new List<MedicamentoActivoResponse>(), notificacionSP);


                var medicamentos = multi.Read<MedicamentoActivoResponse>().ToList();

                if (!medicamentos.Any())
                {
                    var notFound = await _catalogoNotificacionService
                        .GetNotificationByTipoAndFuncionAsync("MEDACTSP", "MEDICAMENTO_ACTIVO_NO_ENCONTRADO");
                    return ResponseFromService<IEnumerable<MedicamentoActivoResponse>>.Failure(notFound);
                }

                return ResponseFromService<IEnumerable<MedicamentoActivoResponse>>.Success(medicamentos, notificacionSP);
            }
            catch (SqlException sqlEx)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicamentoActivoResponse>>.Exeption(sqlEx, errorNotificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<MedicamentoActivoResponse>>.Exeption(ex, errorNotificacion);
            }
        }
        public async Task<ResponseFromService<IEnumerable<ReaccionesPacienteResponse>>> GetReaccionMedicamentoPrevioAsync(Guid idPaciente)
        {
            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[Receta].[GetReaccionesPaciente]",
                    new { IdPaciente = idPaciente }
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<IEnumerable<ReaccionesPacienteResponse>>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<IEnumerable<ReaccionesPacienteResponse>>.Success(new List<ReaccionesPacienteResponse>(), notificacion);

                var reaccionesPrevias = multi.Read<ReaccionesPacienteResponse>().ToList();

                return ResponseFromService<IEnumerable<ReaccionesPacienteResponse>>.Success(reaccionesPrevias, notificacion);
            }
            catch (SqlException sqlEx)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<ReaccionesPacienteResponse>>.Exeption(sqlEx, error);
            }
            catch (Exception ex)
            {
                var error = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<ReaccionesPacienteResponse>>.Exeption(ex, error);
            }
        }

    }
}
