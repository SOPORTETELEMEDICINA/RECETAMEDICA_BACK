using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Sucursal> GetSucursalByIdAsync(Guid idSucursal)
        {
            var idParam = new SqlParameter("@IdSucursal", idSucursal);

            var results = await _context.Sucursales
                .FromSqlRaw("EXEC GetSucursalById @IdSucursal", idParam)
                .ToListAsync();

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
                .ToListAsync();
        }

        public async Task<IEnumerable<Sucursal>> GetSucursalesByGEMPAsync(Guid idGEMP)
        {
            var idGEMPParam = new SqlParameter("@IdGEMP", idGEMP);

            return await _context.Sucursales
                .FromSqlRaw("EXEC GetSucursalesByGEMP @IdGEMP", idGEMPParam)
                .ToListAsync();
        }
    }
}
