using Dapper;
using Microsoft.Data.SqlClient;
using RMD.Interface.PuntoVenta;
using RMD.Interface.Security;
using RMD.Models.PuntoVenta;
using RMD.Shared.Models.PuntoVenta;
using RMD.Shared.Models.Receta.Header.Internos;
using System.Data;

namespace RMD.Service.PuntoVenta
{
    public class PuntoVentaService : IPuntoVentaService
    {
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly IDapperService _dapperService;
        //private readonly PacientesDbContext _pacienteContext;

        public PuntoVentaService(ICatalogoNotificacionService catalogoNotificacionService,
            IDapperService dapperService//,
            //PacientesDbContext pacienteContext
            )
        {
            _catalogoNotificacionService = catalogoNotificacionService;
            _dapperService = dapperService;
            //_pacienteContext = pacienteContext;
        }
        public async Task<ResponseFromService<PuntoVentaRecetaResponse>> ObtenerRecetaAsync(string qrEncriptado)
        {
            var datosQR = LeerDatosQR(qrEncriptado);
            if (datosQR == null || datosQR.IdReceta == Guid.Empty || datosQR.IdMedico == Guid.Empty)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "DATOS_INVALIDOS");
                return ResponseFromService<PuntoVentaRecetaResponse>.Failure(notif);
            }

            try
            {
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[PuntoVenta].[GetReceta]",
                    new
                    {
                        datosQR.IdReceta,
                        datosQR.IdMedico,
                        datosQR.IdGEMP,
                        datosQR.IdSucursal
                    }
                );

                var codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<PuntoVentaRecetaResponse>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<PuntoVentaRecetaResponse>.Success(new PuntoVentaRecetaResponse(), notificacion);

                var recetaModel = multi.ReadFirstOrDefault<PuntoVentaRecetaModel>();

                if (recetaModel == null)
                {
                    var notif = await _catalogoNotificacionService
                        .GetNotificationByCodeAsync(codigoNotificacion);
                    return ResponseFromService<PuntoVentaRecetaResponse>.Failure(notif);
                }

                var detalles = (await _dapperService.QueryAsync<DetalleRecetaModel>(
                    "[PuntoVenta].[GetDetalleReceta]",
                    new
                    {
                        datosQR.IdReceta,
                        datosQR.IdMedico
                    }
                )).ToList();

                var notificacionFinal = await _catalogoNotificacionService
                    .GetNotificationByCodeAsync(codigoNotificacion);

                var responseData = new PuntoVentaRecetaResponse
                {
                    Receta = recetaModel,
                    Detalles = detalles
                };

                return ResponseFromService<PuntoVentaRecetaResponse>.Success(responseData, notificacionFinal);
            }
            catch (SqlException sqlEx)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "ERROR_SURTIDO");
                return ResponseFromService<PuntoVentaRecetaResponse>.Exeption(sqlEx, notif);
            }
            catch (Exception ex)
            {
                var notif = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "ERROR_SURTIDO");
                return ResponseFromService<PuntoVentaRecetaResponse>.Exeption(ex, notif);
            }
        }

        private DatosQRHeader? LeerDatosQR(string qrEncriptado)
        {
            try
            {
                var partes = EncryptionHelper.Decrypt(qrEncriptado).Split('|');
                if (partes.Length != 5)
                    return null;

                return new DatosQRHeader
                {
                    IdReceta = Guid.TryParse(partes[0], out var idReceta) ? idReceta : Guid.Empty,
                    IdMedico = Guid.TryParse(partes[1], out var idMedico) ? idMedico : Guid.Empty,
                    IdGEMP = Guid.TryParse(partes[2], out var idGemp) ? idGemp : Guid.Empty,
                    IdSucursal = Guid.TryParse(partes[3], out var idSucursal) ? idSucursal : Guid.Empty
                };
            }
            catch
            {
                return null;
            }
        }
        public async Task<ResponseFromService<PuntoVentaRecetaResponse>> ConsultarRecetaPorIdAsync(string folio)
        {
            try
            {
                // 🔹 Ejecutar primer SP para obtener receta
                using var multi = await _dapperService.QueryMultipleAsync(
                    "[PuntoVenta].[ConsultarPorId]",
                    new { Folio = folio },
                    commandType: CommandType.StoredProcedure
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<PuntoVentaRecetaResponse>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<PuntoVentaRecetaResponse>.Success(new PuntoVentaRecetaResponse(), notificacion);

                var recetaModel = multi.Read<PuntoVentaRecetaModel>().FirstOrDefault();

                if (recetaModel == null)
                    return ResponseFromService<PuntoVentaRecetaResponse>.Failure(notificacion);

                // 🔹 Ejecutar segundo SP para obtener detalles
                await using var multiDetalles = await _dapperService.QueryMultipleAsync(
                    "[PuntoVenta].[GetDetalleReceta]",
                    new
                    {
                        recetaModel.IdReceta,
                        recetaModel.IdMedico
                    },
                    commandType: CommandType.StoredProcedure
                );

                int codigoNotifDetalles = multiDetalles.ReadFirstOrDefault<int>();
                var notificacionDetalles = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotifDetalles);

                if (notificacionDetalles.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<PuntoVentaRecetaResponse>.Failure(notificacionDetalles);

                var detalles = multiDetalles.Read<DetalleRecetaModel>().ToList();

                // 🔹 Construir y retornar la respuesta
                var responseData = new PuntoVentaRecetaResponse
                {
                    Receta = recetaModel,
                    Detalles = detalles
                };

                return ResponseFromService<PuntoVentaRecetaResponse>.Success(responseData, notificacionDetalles);
            }
            catch (SqlException sqlEx)
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<PuntoVentaRecetaResponse>.Exeption(sqlEx, notif);
            }
            catch (Exception ex)
            {
                var notif = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<PuntoVentaRecetaResponse>.Exeption(ex, notif);
            }
        }
        public async Task<ResponseFromService<string>> SurtirMedicamentosAsync(SurtirRecetaRequest RecetSurtida)
        {
            try
            {
                var detalleTable = new DataTable();
                detalleTable.Columns.Add("IdDetalleReceta", typeof(Guid));
                detalleTable.Columns.Add("MedicamentoId", typeof(int));
                detalleTable.Columns.Add("PiezasSurtidas", typeof(int));

                RecetSurtida.DetallesReceta.ForEach(detalle =>
                {
                    detalleTable.Rows.Add(detalle.IdDetalleReceta, detalle.MedicamentoId, detalle.PiezasSurtidas);
                });

                var parameters = new DynamicParameters();
                parameters.Add("@IdReceta", RecetSurtida.IdReceta);
                parameters.Add("@DetallesReceta", detalleTable.AsTableValuedParameter("dbo.PuntoVenta_SurtidoTableType"));

                using var multi = await _dapperService.QueryMultipleAsync(
                    "[PuntoVenta].[SurtirMedicamentos]",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                int codigoNotificacion = multi.ReadFirstOrDefault<int>();
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR" || notificacion.ToastType.ToUpperInvariant() == "WARNING")
                    return ResponseFromService<string>.Failure(notificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "INFO")
                    return ResponseFromService<string>.Success(string.Empty, notificacion);

                return ResponseFromService<string>.Success("Medicamentos surtidos", notificacion);
            }
            catch (SqlException sqlEx)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "ERROR_SURTIDO");
                return ResponseFromService<string>.Exeption(sqlEx, errorNotificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService
                    .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "ERROR_SURTIDO");
                return ResponseFromService<string>.Exeption(ex, errorNotificacion);
            }
        }
        //public async Task<ResponseFromService<RepositoryPaciente>> GetRepositoryPacienteByQR(string qrEncriptado)
        //{
        //    try
        //    {
        //        var datos = LeerDatosQRPaciente(qrEncriptado);
        //        if (datos == null || datos.Value.IdPaciente == Guid.Empty || string.IsNullOrWhiteSpace(datos.Value.Token))
        //        {
        //            var notif = await _catalogoNotificacionService
        //                .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "QR_INVALIDO");
        //            return ResponseFromService<RepositoryPaciente>.Failure(notif);
        //        }

        //        var tokenValido = await _pacienteContext.TokensQRPacientes.FirstOrDefaultAsync(t =>
        //            t.IdPaciente == datos.Value.IdPaciente &&
        //            t.Token == datos.Value.Token &&
        //            !t.Usado &&
        //            t.FechaExpiracion > DateTime.Now);

        //        if (tokenValido == null)
        //        {
        //            var notif = await _catalogoNotificacionService
        //                .GetNotificationByTipoAndFuncionAsync("PUNTOVENTA", "TOKEN_INVALIDO");
        //            return ResponseFromService<RepositoryPaciente>.Failure(notif);
        //        }

        //        using var multi = await _dapperService.QueryMultipleAsync(
        //            "[PuntoVenta].[GetRepository]",
        //            new { datos.Value.IdPaciente }
        //        );

        //        int codigoNotif = multi.ReadFirstOrDefault<int>();
        //        var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotif);

        //        if (notificacion.ToastType.ToUpperInvariant() != "SUCCESS" && notificacion.ToastType.ToUpperInvariant() != "INFO")
        //            return ResponseFromService<RepositoryPaciente>.Failure(notificacion);

        //        if (notificacion.ToastType.ToUpperInvariant() == "INFO")
        //            return ResponseFromService<RepositoryPaciente>.Success(new RepositoryPaciente(), notificacion);

        //        var paciente = multi.ReadFirstOrDefault<PacienteConsultaRequest>();
        //        var recetas = multi.Read<RepositorioRecetaModel>().ToList();

        //        var resultado = new RepositoryPaciente
        //        {
        //            Paciente = paciente,
        //            repositorioRecetaModel = recetas
        //        };

        //        return ResponseFromService<RepositoryPaciente>.Success(resultado, notificacion);
        //    }
        //    catch (Exception ex)
        //    {
        //        var notif = await _catalogoNotificacionService
        //            .GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
        //        return ResponseFromService<RepositoryPaciente>.Exeption(ex, notif);
        //    }
        //}

        //private (Guid IdPaciente, string Token)? LeerDatosQRPaciente(string qrEncriptado)
        //{
        //    try
        //    {
        //        var textoPlano = EncryptionHelper.Decrypt(qrEncriptado);
        //        var partes = textoPlano.Split('|');

        //        if (partes.Length != 2)
        //            return null;

        //        var idPaciente = Guid.TryParse(partes[0], out var guid) ? guid : Guid.Empty;
        //        var token = partes[1];

        //        if (idPaciente == Guid.Empty || string.IsNullOrWhiteSpace(token))
        //            return null;

        //        return (idPaciente, token);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
    }
}

