using Microsoft.EntityFrameworkCore;
using RMD.Models.Sucursales;

namespace RMD.Data
{
    public class SucursalesDbContext : DbContext
    {
        public SucursalesDbContext(DbContextOptions<SucursalesDbContext> options) : base(options) { }

        public DbSet<Sucursal> Sucursales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Sucursal
            modelBuilder.Entity<Sucursal>(entity =>
            {
                entity.ToTable("Sucursales");
                entity.HasKey(e => e.IdSucursal);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RegistroSanitario).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Responsable).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CedulaResponsable).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TelefonoResponsable).IsRequired().HasMaxLength(15);
                entity.Property(e => e.EmailResponsable).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Domicilio).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            });
        }
    }
}
