using Microsoft.EntityFrameworkCore;
using RMD.Models.Recetas;

namespace RMD.Data
{
    public class RecetasDbContext : DbContext
    {
        public RecetasDbContext(DbContextOptions<RecetasDbContext> options) : base(options) { }

        public DbSet<Receta> Recetas { get; set; }
        public DbSet<DetalleReceta> DetalleRecetas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Receta
            modelBuilder.Entity<Receta>(entity =>
            {
                entity.ToTable("Receta");
                entity.HasKey(e => e.IdReceta);
                entity.Property(e => e.PacPeso).IsRequired();
                entity.Property(e => e.PacTalla).IsRequired();
                entity.Property(e => e.PacEmbarazo).IsRequired();
                entity.Property(e => e.PacLactancia).IsRequired();
                entity.Property(e => e.PacDx1).HasMaxLength(50);
                entity.Property(e => e.PacDx2).HasMaxLength(50);
                entity.Property(e => e.PacDx3).HasMaxLength(50);
                entity.Property(e => e.PacDx4).HasMaxLength(50);
                entity.Property(e => e.PacDx5).HasMaxLength(50);
            });

            // Configure DetalleReceta
            modelBuilder.Entity<DetalleReceta>(entity =>
            {
                entity.ToTable("DetalleReceta");
                entity.HasKey(e => e.IdDetalleReceta);
                entity.Property(e => e.Medicamento).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CantidadDiaria).IsRequired();
                entity.Property(e => e.UnidadDispensacion).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RutaAdministracion).IsRequired().HasMaxLength(50);
                entity.Property(e => e.UnidadDuracion).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PeriodoInicio).IsRequired();
            });
        }
    }
}
