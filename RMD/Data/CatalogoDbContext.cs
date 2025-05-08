using RMD.Models.Catalogo;
using RMD.Models.Consulta;

namespace RMD.Data
{
    public class CatalogoDbContext(DbContextOptions<CatalogoDbContext> options) : DbContext(options)
    {
        public DbSet<CatEntidadesFederativas> CatEntidadesFederativas { get; set; }
        public DbSet<CatMunicipios> CatMunicipios { get; set; }
        public DbSet<CatTipoAsentamiento> CatTipoAsentamiento { get; set; }
        public DbSet<CatCP> CatCP { get; set; }
        public DbSet<CatCiudades> CatCiudades { get; set; }
        public DbSet<AsentamientoResultModel> AsentamientoResultModel { get; set; }
        public DbSet<CatEventosDeSalud> CatEventosDeSalud { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Definir las llaves primarias
            modelBuilder.Entity<CatEntidadesFederativas>()
                .HasKey(e => e.IdEntidad);

            modelBuilder.Entity<AsentamientoResultModel>()
           .HasNoKey();

            modelBuilder.Entity<CatMunicipios>()
                .HasKey(m => m.IdMunicipio);

            modelBuilder.Entity<CatTipoAsentamiento>()
                .HasKey(t => t.IdTipoAsentamiento);

            modelBuilder.Entity<CatCP>()
                .HasKey(cp => cp.IdCP);

            modelBuilder.Entity<CatCiudades>()
                .HasKey(c => c.IdCiudad);

            // Configuración de las relaciones
            modelBuilder.Entity<CatMunicipios>()
                .HasOne<CatEntidadesFederativas>()
                .WithMany()
                .HasForeignKey(m => m.IdEntidad)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CatCP>()
                .HasOne<CatEntidadesFederativas>()
                .WithMany()
                .HasForeignKey(cp => cp.IdEntidad)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CatCP>()
                .HasOne<CatMunicipios>()
                .WithMany()
                .HasForeignKey(cp => cp.IdMunicipio)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración opcional de nombres de tablas si difieren en la base de datos
            modelBuilder.Entity<CatEntidadesFederativas>().ToTable("CatEntidadesFederativas");
            modelBuilder.Entity<CatMunicipios>().ToTable("CatMunicipios");
            modelBuilder.Entity<CatTipoAsentamiento>().ToTable("CatTipoAsentamiento");
            modelBuilder.Entity<CatCP>().ToTable("CatCP"); // Si en SQL sigue siendo CatCP2
            modelBuilder.Entity<CatCiudades>().ToTable("CatCiudades");

            modelBuilder.Entity<CatEventosDeSalud>().HasKey(e => e.IdEvento);
            modelBuilder.Entity<CatEventosDeSalud>().ToTable("CatEventosDeSalud");
        }
    }
}
