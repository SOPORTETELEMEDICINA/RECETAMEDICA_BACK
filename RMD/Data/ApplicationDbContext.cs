using Microsoft.EntityFrameworkCore;
using RMD.Shared.Models.Login;

namespace RMD.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RegistroPeticion> RegistroPeticiones { get; set; }
        public DbSet<CatalogoNotificacion> CatalogoNotificaciones { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegistroPeticion>()
                .ToTable("RegistroPeticiones", "Configuracion");

            modelBuilder.Entity<CatalogoNotificacion>(entity =>
            {
                entity.ToTable("CatalogoNotificaciones", "Configuracion"); // Mapea al esquema "Configuracion"
                entity.HasKey(e => e.CodigoNotificacion); // Especifica la clave primaria
            });
        }

    }
}

