using Microsoft.EntityFrameworkCore;
using RMD.Shared.Models.ServiciosInternos;

namespace RMD.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ConfiguracionGlobal> ConfiguracionGlobal { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConfiguracionGlobal>().ToTable("ConfiguracionGlobal", "Configuracion");
        }
    }

}
