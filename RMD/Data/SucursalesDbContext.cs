using Microsoft.EntityFrameworkCore;
using RMD.Shared.Models.Sucursales;

namespace RMD.Data
{
    public class SucursalesDbContext(DbContextOptions<SucursalesDbContext> options) : DbContext(options)
    {
        public DbSet<Sucursal> Sucursales { get; set; }
        
    }
}
