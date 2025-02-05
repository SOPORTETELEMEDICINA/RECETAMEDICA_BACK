using RMD.Data;
using RMD.Interface.Sucursales;
using RMD.Models.Sucursales;

namespace RMD.Service.Sucursales
{
    public class SucursalService : ISucursalService
    {
        private readonly SucursalesDbContext _context;

        public SucursalService(SucursalesDbContext context)
        {
            _context = context;
        }

        //public async Task<SucursalRequest> GetSucursalByIdAsync(Guid idSucursal)
        //{
        //    var idParam = new SqlParameter("@IdSucursal", idSucursal);

        //    var results = await _context.SucursalRequest
        //        .FromSqlRaw("EXEC GetSucursalById @IdSucursal", idParam)
        //        .ToListAsync(); // Se mantiene el uso de ToListAsync directamente

        //    if (results.Count == 0)
        //    {
        //        throw new KeyNotFoundException($"Sucursal with ID {idSucursal} not found.");
        //    }

        //    return results[0];
        //}

        //public async Task<IEnumerable<SucursalRequest>> GetSucursalesByAsentamientoAsync(int idAsentamiento)
        //{
        //    var idAsentamientoParam = new SqlParameter("@IdAsentamiento", idAsentamiento);

        //    return await _context.SucursalRequest
        //        .FromSqlRaw("EXEC GetSucursalesByAsentamiento @IdAsentamiento", idAsentamientoParam)
        //        .ToListAsync(); // Se mantiene el uso de ToListAsync directamente
        //}

        //public async Task<IEnumerable<SucursalRequest>> GetSucursalesByGEMPAsync(Guid idGEMP)
        //{
        //    var idGEMPParam = new SqlParameter("@IdGEMP", idGEMP);

        //    return await _context.SucursalRequest
        //        .FromSqlRaw("EXEC GetSucursalesByGEMP @IdGEMP", idGEMPParam)
        //        .ToListAsync(); // Se mantiene el uso de ToListAsync directamente
        //}

        public async Task<bool> CreateSucursalAsync(CreateSucursalModel model)
        {
            var sucursalData = new DataTable();
            sucursalData.Columns.Add("IdGEMP", typeof(Guid));
            sucursalData.Columns.Add("Nombre", typeof(string));
            sucursalData.Columns.Add("RegistroSanitario", typeof(string));
            sucursalData.Columns.Add("Responsable", typeof(string));
            sucursalData.Columns.Add("CedulaResponsable", typeof(string));
            sucursalData.Columns.Add("TelefonoResponsable", typeof(string));
            sucursalData.Columns.Add("EmailResponsable", typeof(string));
            sucursalData.Columns.Add("Domicilio", typeof(string));
            sucursalData.Columns.Add("IdAsentamiento", typeof(int));

            sucursalData.Rows.Add(
                model.IdGEMP, model.Nombre, model.RegistroSanitario, model.Responsable,
                model.CedulaResponsable, model.TelefonoResponsable, model.EmailResponsable, model.Domicilio,
                model.IdAsentamiento
            );

            var sucursalParameter = new SqlParameter("@SucursalData", SqlDbType.Structured)
            {
                TypeName = "dbo.SucursalCreateTableType",
                Value = sucursalData
            };

            var returnParameter = new SqlParameter
            {
                ParameterName = "@ReturnVal",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            // Ejecuta el SP y captura el valor de retorno
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC @ReturnVal = Sucursales_Create @SucursalData",
                returnParameter,
                sucursalParameter
            );

            // Obtener el valor de retorno del SP
            var result = (int)returnParameter.Value;

            // Verifica el valor de retorno
            return result == 1;
        }


        public async Task<bool> UpdateSucursalAsync(Guid idSucursal, UpdateSucursalModel model)
        {
            var sucursalData = new DataTable();
            sucursalData.Columns.Add("Nombre", typeof(string));
            sucursalData.Columns.Add("RegistroSanitario", typeof(string));
            sucursalData.Columns.Add("Responsable", typeof(string));
            sucursalData.Columns.Add("CedulaResponsable", typeof(string));
            sucursalData.Columns.Add("TelefonoResponsable", typeof(string));
            sucursalData.Columns.Add("EmailResponsable", typeof(string));
            sucursalData.Columns.Add("Domicilio", typeof(string));
            sucursalData.Columns.Add("IdAsentamiento", typeof(int));

            sucursalData.Rows.Add(
                model.Nombre, model.RegistroSanitario, model.Responsable, model.CedulaResponsable,
                model.TelefonoResponsable, model.EmailResponsable, model.Domicilio, model.IdAsentamiento
            );

            var idParam = new SqlParameter("@IdSucursal", idSucursal);
            var parameter = new SqlParameter("@SucursalData", SqlDbType.Structured)
            {
                TypeName = "dbo.Sucursales_UpdateSucursal",
                Value = sucursalData
            };

            // Parámetro de salida para capturar el valor de retorno del SP
            var resultParam = new SqlParameter("@Resultado", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Sucursales_Update @IdSucursal, @SucursalData, @Resultado OUTPUT",
                idParam, parameter, resultParam
            );

            // Verificar el valor del parámetro de salida
            int result = (int)resultParam.Value;
            return result == 1;
        }

        public async Task<bool> DeleteSucursalAsync(Guid idSucursal)
        {
            var idParam = new SqlParameter("@IdSucursal", idSucursal);
            var resultParam = new SqlParameter("@Resultado", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC Sucursales_Delete @IdSucursal, @Resultado OUTPUT", idParam, resultParam);

            // Verificar el valor del parámetro de salida
            int result = (int)resultParam.Value;
            return result == 1;
        }



        public async Task<SucursalRequest> GetSucursalByIdSucursalAsync(Guid idSucursal)
        {
            try
            {
                var idParam = new SqlParameter("@IdSucursal", SqlDbType.UniqueIdentifier) // Especificamos que es un UniqueIdentifier
                {
                    Value = idSucursal
                };

                var result = await _context.SucursalRequest
                    .FromSqlRaw("EXEC Sucursales_GetSucursalByIdSucursal @IdSucursal", idParam)
                    .ToListAsync();

                if (result.Count > 0)
                {
                    return result.First();
                }
                else
                {
                    throw new KeyNotFoundException("Sucursal no encontrada.");
                }

            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error en la base de datos: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el Tipo de Usuario: {ex.Message}");
            }
        }



        public async Task<IEnumerable<SucursalRequest>> GetSucursalesByIdGEMPAsync(Guid idGEMP)
        {
            var idParam = new SqlParameter("@IdGEMP", idGEMP);

            return await _context.SucursalRequest
                .FromSqlRaw("EXEC Sucursales_GetSucursalByIdGEMP @IdGEMP", idParam)
                .ToListAsync(); // Se mantiene el uso de ToListAsync directamente
        }

        public async Task<IEnumerable<SucursalRequest>> GetSucursalesByIdGEMPAndIdAsentamientoAsync(Guid idGEMP, int idAsentamiento)
        {
            var gempParam = new SqlParameter("@IdGEMP", idGEMP);
            var asentamientoParam = new SqlParameter("@IdAsentamiento", idAsentamiento);

            return await _context.SucursalRequest
                .FromSqlRaw("EXEC Sucursales_GetSucursalByIdGEMPandIdAsentamiento @IdGEMP, @IdAsentamiento", gempParam, asentamientoParam)
                .ToListAsync(); // Se mantiene el uso de ToListAsync directamente
        }
    }
}
