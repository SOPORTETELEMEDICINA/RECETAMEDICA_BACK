using Microsoft.EntityFrameworkCore;
using RMD.Models.Catalogos;
using RMD.Models.Consulta;

namespace RMD.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Catalogo> Catalogos { get; set; }
        public DbSet<Asentamiento> Asentamiento { get; set; }
        public DbSet<CatalogoDetail> CatalogoDetail { get; set; }
        public DbSet<Ciudad> Ciudad { get; set; }
        public DbSet<CP> CP { get; set; }
        public DbSet<Entidad> Entidad { get; set; }
        public DbSet<Municipio> Municipio { get; set; }

        public DbSet<AsentamientoResultModel> AsentamientoResultModel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AsentamientoResultModel>()
            .HasNoKey();

            modelBuilder.Entity<Catalogo>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null); // Se asegura de que no sea tratado como una vista
            });
            modelBuilder.Entity<Asentamiento>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null); // Se asegura de que no sea tratado como una vista
            });
            modelBuilder.Entity<CatalogoDetail>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null); // Se asegura de que no sea tratado como una vista
            });
            modelBuilder.Entity<Ciudad>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null); // Se asegura de que no sea tratado como una vista
            });
            modelBuilder.Entity<CP>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null); // Se asegura de que no sea tratado como una vista
            });
            modelBuilder.Entity<Entidad>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null); // Se asegura de que no sea tratado como una vista
            });
            modelBuilder.Entity<Municipio>(entity =>
            {
                entity.HasNoKey();
                entity.ToView(null); // Se asegura de que no sea tratado como una vista
            });
        }
    }
}
