using RMD.Data;
using RMD.Extensions;
using RMD.Interface.Notificaciones;
using RMD.Interface.Sucursales;
using RMD.Models.Responses;
using RMD.Models.Sucursales;

namespace RMD.Service.Sucursales
{
    public class SucursalService : ISucursalService
    {
        private readonly SucursalesDbContext _context;
        private readonly ICatalogoNotificacionService _catalogoNotificacionService;
        private readonly string _connectionString;

        public SucursalService(SucursalesDbContext context, ICatalogoNotificacionService catalogoNotificacionService, string connectionString)
        {
            _context = context;
            _catalogoNotificacionService = catalogoNotificacionService;
            _connectionString = connectionString;
        }

        public async Task<ResponseFromService<bool>> CreateSucursalAsync(CreateSucursalModel model)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameter = new SqlParameter("@SucursalData", SqlDbType.Structured)
                {
                    TypeName = "dbo.SucursalCreateTableType",
                    Value = new List<CreateSucursalModel> { model }.ToDataTable()
                };

                using var command = new SqlCommand("Sucursales_Create", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<bool>.Failure(notificacion);

                return ResponseFromService<bool>.Success(true, notificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }

        public async Task<ResponseFromService<bool>> UpdateSucursalAsync(Guid idSucursal, UpdateSucursalModel model)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var table = new List<UpdateSucursalModel> { model }.ToDataTable();
                var parameter = new SqlParameter("@SucursalData", SqlDbType.Structured)
                {
                    TypeName = "dbo.Sucursales_UpdateSucursal",
                    Value = table
                };

                var idParam = new SqlParameter("@IdSucursal", idSucursal);

                using var command = new SqlCommand("Sucursales_Update", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(idParam);
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<bool>.Failure(notificacion);

                return ResponseFromService<bool>.Success(true, notificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }


        public async Task<ResponseFromService<bool>> DeleteSucursalAsync(Guid idSucursal)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var param = new SqlParameter("@IdSucursal", idSucursal);

                using var command = new SqlCommand("Sucursales_Delete", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(param);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<bool>.Failure(notificacion);

                return ResponseFromService<bool>.Success(true, notificacion);
            }
            catch (Exception ex)
            {
                var notificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<bool>.Exeption(ex, notificacion);
            }
        }

        public async Task<ResponseFromService<SucursalRequest>> GetSucursalByIdSucursalAsync(Guid idSucursal)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var param = new SqlParameter("@IdSucursal", idSucursal);

                using var command = new SqlCommand("Sucursales_GetSucursalByIdSucursal", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(param);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<SucursalRequest>.Failure(notificacion);

                SucursalRequest sucursal = null;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                    sucursal = SucursalRequest.FromDataReader(reader);

                return ResponseFromService<SucursalRequest>.Success(sucursal, notificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<SucursalRequest>.Exeption(ex, errorNotificacion);
            }
        }

        public async Task<ResponseFromService<IEnumerable<SucursalRequest>>> GetSucursalesByIdGEMPAsync(Guid idGEMP)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var param = new SqlParameter("@IdGEMP", idGEMP);

                using var command = new SqlCommand("Sucursales_GetSucursalByIdGEMP", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(param);

                using var reader = await command.ExecuteReaderAsync();
                int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
                var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

                if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
                    return ResponseFromService<IEnumerable<SucursalRequest>>.Failure(notificacion);

                var sucursales = new List<SucursalRequest>();
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                        sucursales.Add(SucursalRequest.FromDataReader(reader));
                }

                return ResponseFromService<IEnumerable<SucursalRequest>>.Success(sucursales, notificacion);
            }
            catch (Exception ex)
            {
                var errorNotificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
                return ResponseFromService<IEnumerable<SucursalRequest>>.Exeption(ex, errorNotificacion);
            }
        }

        //public async Task<ResponseFromService<IEnumerable<SucursalRequest>>> GetSucursalesByIdGEMPAndIdAsentamientoAsync(Guid idGEMP, int idAsentamiento)
        //{
        //    try
        //    {
        //        using var connection = new SqlConnection(_connectionString);
        //        await connection.OpenAsync();

        //        var gempParam = new SqlParameter("@IdGEMP", idGEMP);
        //        var asentamientoParam = new SqlParameter("@IdAsentamiento", idAsentamiento);

        //        using var command = new SqlCommand("Sucursales_GetSucursalByIdGEMPandIdAsentamiento", connection)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };
        //        command.Parameters.Add(gempParam);
        //        command.Parameters.Add(asentamientoParam);

        //        using var reader = await command.ExecuteReaderAsync();
        //        int codigoNotificacion = await ValidationHelper.ReadErrorCodeAsync(reader);
        //        var notificacion = await _catalogoNotificacionService.GetNotificationByCodeAsync(codigoNotificacion);

        //        if (notificacion.ToastType.ToUpperInvariant() == "ERROR")
        //            return ResponseFromService<IEnumerable<SucursalRequest>>.Failure(notificacion);

        //        var sucursales = new List<SucursalRequest>();
        //        if (await reader.NextResultAsync())
        //        {
        //            while (await reader.ReadAsync())
        //                sucursales.Add(SucursalRequest.FromDataReader(reader));
        //        }

        //        return ResponseFromService<IEnumerable<SucursalRequest>>.Success(sucursales, notificacion);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorNotificacion = await _catalogoNotificacionService.GetNotificationByTipoAndFuncionAsync("GENERAL", "EXEPTIONDETECTADA");
        //        return ResponseFromService<IEnumerable<SucursalRequest>>.Exeption(ex, errorNotificacion);
        //    }
        //}

    }
}
