using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Sucursales;
using RMD.Models.Sucursales;
using RMD.Models.Usuarios;
using System.Data;

namespace RMD.Service.Sucursales
{
    public class SucursalService : ISucursalService
    {
        private readonly SucursalesDbContext _context;

        public SucursalService(SucursalesDbContext context)
        {
            _context = context;
        }

        public async Task<Sucursal> GetSucursalByIdAsync(Guid idSucursal)
        {
            var idParam = new SqlParameter("@IdSucursal", idSucursal);

            var results = await _context.Sucursales
                .FromSqlRaw("EXEC GetSucursalById @IdSucursal", idParam)
                .ToListAsync(); // Se mantiene el uso de ToListAsync directamente

            if (results.Count == 0)
            {
                throw new KeyNotFoundException($"Sucursal with ID {idSucursal} not found.");
            }

            return results[0];
        }

        public async Task<IEnumerable<Sucursal>> GetSucursalesByAsentamientoAsync(int idAsentamiento)
        {
            var idAsentamientoParam = new SqlParameter("@IdAsentamiento", idAsentamiento);

            return await _context.Sucursales
                .FromSqlRaw("EXEC GetSucursalesByAsentamiento @IdAsentamiento", idAsentamientoParam)
                .ToListAsync(); // Se mantiene el uso de ToListAsync directamente
        }

        public async Task<IEnumerable<Sucursal>> GetSucursalesByGEMPAsync(Guid idGEMP)
        {
            var idGEMPParam = new SqlParameter("@IdGEMP", idGEMP);

            return await _context.Sucursales
                .FromSqlRaw("EXEC GetSucursalesByGEMP @IdGEMP", idGEMPParam)
                .ToListAsync(); // Se mantiene el uso de ToListAsync directamente
        }

        public async Task<bool> CreateSucursalAsync(CreateSucursalModel model)
        {
            var sucursalData = new DataTable();
            sucursalData.Columns.Add("IdGEMP", typeof(Guid));
            sucursalData.Columns.Add("Numero", typeof(int));
            sucursalData.Columns.Add("Nombre", typeof(string));
            sucursalData.Columns.Add("RegistroSanitario", typeof(string));
            sucursalData.Columns.Add("Responsable", typeof(string));
            sucursalData.Columns.Add("CedulaResponsable", typeof(string));
            sucursalData.Columns.Add("TelefonoResponsable", typeof(string));
            sucursalData.Columns.Add("EmailResponsable", typeof(string));
            sucursalData.Columns.Add("Domicilio", typeof(string));
            sucursalData.Columns.Add("IdAsentamiento", typeof(int));
            sucursalData.Columns.Add("Status", typeof(string));

            sucursalData.Rows.Add(
                model.IdGEMP, model.Numero, model.Nombre, model.RegistroSanitario, model.Responsable,
                model.CedulaResponsable, model.TelefonoResponsable, model.EmailResponsable, model.Domicilio,
                model.IdAsentamiento, model.Status
            );

            var parameter = new SqlParameter("@SucursalData", SqlDbType.Structured)
            {
                TypeName = "dbo.SucursalTableType",
                Value = sucursalData
            };

            var result = await _context.Database.ExecuteSqlRawAsync("EXEC Sucursales_Create @SucursalData", parameter);
            return result > 0;
        }

        public async Task<bool> UpdateSucursalAsync(Guid idSucursal, UpdateSucursalModel model)
        {
            var sucursalData = new DataTable();
            sucursalData.Columns.Add("IdGEMP", typeof(Guid));
            sucursalData.Columns.Add("Numero", typeof(int));
            sucursalData.Columns.Add("Nombre", typeof(string));
            sucursalData.Columns.Add("RegistroSanitario", typeof(string));
            sucursalData.Columns.Add("Responsable", typeof(string));
            sucursalData.Columns.Add("CedulaResponsable", typeof(string));
            sucursalData.Columns.Add("TelefonoResponsable", typeof(string));
            sucursalData.Columns.Add("EmailResponsable", typeof(string));
            sucursalData.Columns.Add("Domicilio", typeof(string));
            sucursalData.Columns.Add("IdAsentamiento", typeof(int));
            sucursalData.Columns.Add("Status", typeof(string));

            sucursalData.Rows.Add(
                model.IdGEMP, model.Numero, model.Nombre, model.RegistroSanitario, model.Responsable,
                model.CedulaResponsable, model.TelefonoResponsable, model.EmailResponsable, model.Domicilio,
                model.IdAsentamiento, model.Status
            );

            var parameter = new SqlParameter("@SucursalData", SqlDbType.Structured)
            {
                TypeName = "dbo.SucursalTableType",
                Value = sucursalData
            };

            var idParam = new SqlParameter("@IdSucursal", idSucursal);
            var result = await _context.Database.ExecuteSqlRawAsync("EXEC Sucursales_Update @IdSucursal, @SucursalData", idParam, parameter);
            return result > 0;
        }

        public async Task<bool> DeleteSucursalAsync(Guid idSucursal)
        {
            var idParam = new SqlParameter("@IdSucursal", idSucursal);
            var result = await _context.Database.ExecuteSqlRawAsync("EXEC Sucursales_Delete @IdSucursal", idParam);
            return result > 0;
        }


        public async Task<SucursalDomicilioModel> GetSucursalByIdSucursalAsync(Guid idSucursal)
        {
            try
            {
                var idParam = new SqlParameter("@IdSucursal", SqlDbType.UniqueIdentifier) // Especificamos que es un UniqueIdentifier
                {
                    Value = idSucursal
                };

                var result = await _context.SucursalDomicilioModel
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



        public async Task<IEnumerable<SucursalDomicilioModel>> GetSucursalesByIdGEMPAsync(Guid idGEMP)
        {
            var idParam = new SqlParameter("@IdGEMP", idGEMP);

            return await _context.SucursalDomicilioModel
                .FromSqlRaw("EXEC Sucursales_GetSucursalByIdGEMP @IdGEMP", idParam)
                .ToListAsync(); // Se mantiene el uso de ToListAsync directamente
        }

        public async Task<IEnumerable<SucursalDomicilioModel>> GetSucursalesByIdGEMPAndIdAsentamientoAsync(Guid idGEMP, int idAsentamiento)
        {
            var gempParam = new SqlParameter("@IdGEMP", idGEMP);
            var asentamientoParam = new SqlParameter("@IdAsentamiento", idAsentamiento);

            return await _context.SucursalDomicilioModel
                .FromSqlRaw("EXEC Sucursales_GetSucursalByIdGEMPandIdAsentamiento @IdGEMP, @IdAsentamiento", gempParam, asentamientoParam)
                .ToListAsync(); // Se mantiene el uso de ToListAsync directamente
        }
    }
}
