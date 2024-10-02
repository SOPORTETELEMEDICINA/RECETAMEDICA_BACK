using Microsoft.EntityFrameworkCore;
using RMD.Models.Recetas;

namespace RMD.Data
{
    public class RecetasDbContext : DbContext
    {
        public RecetasDbContext(DbContextOptions<RecetasDbContext> options) : base(options) { }

        public DbSet<Receta> Receta { get; set; }
        public DbSet<DetalleReceta> DetalleRecetas { get; set; }

        // DbSet para las entidades de Recetas
        public DbSet<RecetaModel> Recetas { get; set; }
        public DbSet<RecetaDetalleModel> RecetaDetalles { get; set; }


        // DbSet para el modelo combinado de Receta y sus detalles
        public DbSet<RecetaWithDetalleModel> RecetaWithDetalles { get; set; }


        // DbSets para alergias, moléculas y CIM10
        public DbSet<AllergyModel> AllergyModels { get; set; }
        public DbSet<MoleculeModel> MoleculeModels { get; set; }
        public DbSet<CIM10Model> CIM10Models { get; set; }


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
            // Configuración para RecetaModel
            modelBuilder.Entity<RecetaModel>(entity =>
            {
                entity.ToTable("Receta");
                entity.HasKey(e => e.IdReceta);

                entity.Property(e => e.PacPeso).HasColumnType("decimal(5,2)").IsRequired();
                entity.Property(e => e.PacTalla).HasColumnType("decimal(5,2)").IsRequired();
                entity.Property(e => e.PacEmbarazo).IsRequired();
                entity.Property(e => e.PacSemAmenorrea);
                entity.Property(e => e.PacLactancia).IsRequired();
                entity.Property(e => e.PacCreatinina).HasColumnType("decimal(5,2)");

                entity.Property(e => e.Alergias).HasColumnType("varchar(max)");
                entity.Property(e => e.Molecules).HasColumnType("varchar(max)");
                entity.Property(e => e.Patologias).HasColumnType("varchar(max)");

                entity.Property(e => e.IdSucursal).IsRequired();
                entity.Property(e => e.IdGEMP).IsRequired();
            });

            // Configuración para RecetaDetalleModel
            modelBuilder.Entity<RecetaDetalleModel>(entity =>
            {
                entity.ToTable("RecetaDetalle");
                entity.HasKey(e => e.IdDetalleReceta);

                entity.Property(e => e.Medicamento).HasMaxLength(255).IsRequired();
                entity.Property(e => e.CantidadDiaria).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.UnidadDispensacion).HasMaxLength(50).IsRequired();
                entity.Property(e => e.RutaAdministracion).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Indicacion).HasMaxLength(255);
                entity.Property(e => e.Duracion).IsRequired();
                entity.Property(e => e.UnidadDuracion).HasMaxLength(50).IsRequired();
                entity.Property(e => e.PeriodoInicio).HasColumnType("date").IsRequired();
                entity.Property(e => e.PeriodoTerminacion).HasColumnType("date");

                entity.HasOne<RecetaModel>()
                    .WithMany()
                    .HasForeignKey(e => e.IdReceta)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración para AllergyModel
            modelBuilder.Entity<AllergyModel>(entity =>
            {
                entity.HasKey(e => e.IdAllergy);  // Clave primaria
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.VidalUpdateDate).IsRequired();
            });

            // Configuración para MoleculeModel
            modelBuilder.Entity<MoleculeModel>(entity =>
            {
                entity.HasKey(e => e.IdMolecule);  // Clave primaria
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.SafetyAlert).HasMaxLength(50);
                entity.Property(e => e.VidalUpdateDate).IsRequired();
            });

            // Configuración para CIM10Model
            modelBuilder.Entity<CIM10Model>(entity =>
            {
                entity.HasKey(e => e.IdCIM10);  // Clave primaria
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.UpdatedDate).IsRequired();
            });

            // Configuración para RecetaWithDetalleModel (no mapeada directamente a tabla)
            modelBuilder.Entity<RecetaWithDetalleModel>().HasNoKey(); // No necesita clave primaria, ya que es una unión de datos

        }
    }
}
