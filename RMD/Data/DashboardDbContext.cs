using Microsoft.EntityFrameworkCore;

namespace RMD.Data
{
    public class DashboardDbContext(DbContextOptions<DashboardDbContext> options) : DbContext(options)
    {
        //public DbSet<SucursalPacientes> SucursalPacientes { get; set; }

        //public DbSet<DashBoardKPIPacientesRecetas> MedicoKPIPacientesRecetas { get; set; }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<SucursalPacientes>()
        //       .HasNoKey();

        //    // Configuración para MedicoKPIPacientesRecetas
        //    modelBuilder.Entity<DashBoardKPIPacientesRecetas>().HasNoKey();  // Modelo sin clave primaria
        //}
    }
}
