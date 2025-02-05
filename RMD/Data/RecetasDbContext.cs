using RMD.Models.Recetas;

namespace RMD.Data
{
    public class RecetasDbContext(DbContextOptions<RecetasDbContext> options) : DbContext(options)
    {

        // DbSet para las entidades relacionadas con recetas
        public DbSet<Receta> Receta { get; set; } // Usado por `RecetaService`
        public DbSet<DetalleReceta> DetalleRecetas { get; set; } // Usado por `DetalleRecetaService`

        // DbSet para las consultas de SP
        public DbSet<RecetaWithDetalleModel> RecetaWithDetalles { get; set; } // Usado por `RecetaService`
        public DbSet<RecetaList> RecetaList { get; set; } // Usado por `RecetaService`

        // DbSet para modelos relacionados con alergias, moléculas y CIM10
        public DbSet<AllergyModel> AllergyModels { get; set; } // Usado por `RecetaService`
        public DbSet<MoleculeModel> MoleculeModels { get; set; } // Usado por `RecetaService`
        public DbSet<CIM10Model> CIM10Models { get; set; } // Usado por `RecetaService`

        // DbSet para detalles de recetas
        public DbSet<DetalleRecetaRequest> DetalleRecetaRequest { get; set; } // Usado por `DetalleRecetaService`
        public DbSet<DetalleRecetaResponse> DetalleRecetaResponse { get; set; } // Usado por `DetalleRecetaService`

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para Receta
            modelBuilder.Entity<Receta>(entity =>
            {
                entity.ToTable("Receta");
                entity.HasKey(e => e.IdReceta);
                entity.Property(e => e.PacPeso).HasColumnType("decimal(5,2)").IsRequired();
                entity.Property(e => e.PacTalla).HasColumnType("decimal(5,2)").IsRequired();
                entity.Property(e => e.PacEmbarazo).IsRequired();
                entity.Property(e => e.PacLactancia).IsRequired();
                entity.Property(e => e.PacCreatinina).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Alergias).HasColumnType("varchar(max)");
                entity.Property(e => e.Molecules).HasColumnType("varchar(max)");
                entity.Property(e => e.Patologias).HasColumnType("varchar(max)");
                entity.Property(e => e.IdSucursal).IsRequired();
                entity.Property(e => e.IdGEMP).IsRequired();
            });

            // Configuración para DetalleReceta
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
                entity.HasOne<Receta>()
                    .WithMany()
                    .HasForeignKey(e => e.IdReceta)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración para RecetaWithDetalleModel
            modelBuilder.Entity<RecetaWithDetalleModel>().HasNoKey(); // Utilizado para SP

            // Configuración para RecetaList
            modelBuilder.Entity<RecetaList>().HasNoKey(); // Utilizado para SP

            // Configuración para AllergyModel
            modelBuilder.Entity<AllergyModel>().HasNoKey(); // Utilizado para SP

            // Configuración para MoleculeModel
            modelBuilder.Entity<MoleculeModel>().HasNoKey(); // Utilizado para SP

            // Configuración para CIM10Model
            modelBuilder.Entity<CIM10Model>().HasNoKey(); // Utilizado para SP

            // Configuración para DetalleRecetaRequest
            modelBuilder.Entity<DetalleRecetaRequest>().HasNoKey(); // Utilizado para SP

            // Configuración para DetalleRecetaResponse
            modelBuilder.Entity<DetalleRecetaResponse>().HasNoKey(); // Utilizado para SP
        }
    }
}