using Microsoft.EntityFrameworkCore;

namespace RMD.Data
{
    public class ConsultaDbContext : DbContext
    {
        public ConsultaDbContext(DbContextOptions<ConsultaDbContext> options) : base(options)
        {
        }
        //public DbSet<SucursalModel> Sucursales { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //    // Configuración de la tabla Receta usando RecetaSqlModel para lecturas/operaciones completas
        //    modelBuilder.Entity<RecetaSqlModel>()
        //     .ToTable("Header", schema: "Receta")
        //     .HasKey(r => r.IdReceta);


        //    // Configuración de la tabla Sucursales
        //    modelBuilder.Entity<SucursalModel>()
        //        .ToTable("Sucursales")
        //        .HasKey(s => s.IdSucursal);
        //}
    }
}
