using Microsoft.EntityFrameworkCore;
using RMD.Shared.Models.Receta.Detalle.Response;
using RMD.Shared.Models.Receta.Header.Internos;
using RMD.Shared.Models.Receta.Header.Responses;
using RMD.Shared.Models.Sql;

namespace RMD.Data
{
    public class RecetasDbContext(DbContextOptions<RecetasDbContext> options) : DbContext(options)
    {

        public DbSet<DetalleInHeader> DetalleRecetas { get; set; } // Usado por `DetalleRecetaService`
    
        public DbSet<Header> ConsultaRecetas { get; set; }
       // public DbSet<RecetaList> RecetaList { get; set; } // Usado por `RecetaService`
        public DbSet<RecetaSqlModel> RecetasSql { get; set; }

        public DbSet<EF_MedicoResponse> Medicos { get; set; }
        public DbSet<EF_PacienteResponse> Pacientes { get; set; }
        public DbSet<DetalleCronico> DetalleCronico { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EF_PacienteResponse>()
                .ToTable("Pacientes")
                .HasKey(p => p.IdPaciente);

            modelBuilder.Entity<EF_MedicoResponse>()
                .ToTable("Medicos")
                .HasKey(m => m.IdMedico);

            // Cambiar Receta → Receta.Header
            modelBuilder.Entity<Header>(entity =>
            {
                entity.ToTable("Header", "Receta");
                entity.HasKey(e => e.IdReceta);
                entity.Property(e => e.PacPeso).HasPrecision(5, 2).IsRequired();
                entity.Property(e => e.PacTalla).HasPrecision(5, 2).IsRequired();
                entity.Property(e => e.PacEmbarazo).IsRequired();
                entity.Property(e => e.PacLactancia).IsRequired();
                entity.Property(e => e.PacCreatinina).HasPrecision(5, 2);
                entity.Property(e => e.Alergias).HasColumnType("varchar(max)");
                entity.Property(e => e.Molecules).HasColumnType("varchar(max)");
                entity.Property(e => e.Patologias).HasColumnType("varchar(max)");
                entity.Property(e => e.IdSucursal).IsRequired();
                entity.Property(e => e.IdGEMP).IsRequired();
            });

            // Cambiar DetalleReceta → Receta.Detalle
            modelBuilder.Entity<DetalleInHeader>(entity =>
            {
                entity.ToTable("Detalle", "Receta");
                entity.HasKey(e => e.IdDetalleReceta);

                entity.Property(e => e.IdDetalleReceta).ValueGeneratedNever();
                entity.Property(e => e.MedicamentoId).IsRequired();
                entity.Property(e => e.CantidadDiaria).HasPrecision(5, 2).IsRequired();
                entity.Property(e => e.UnidadDispensacionId).IsRequired();
                entity.Property(e => e.RutaAdministracionId).IsRequired();
                entity.Property(e => e.UnidadDuracion).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PeriodoInicio).IsRequired();
                entity.Property(e => e.PackageQty);
                entity.Property(e => e.CantidadSurtida);
                entity.Ignore(e => e.FrecuencyType); // 👈 agrega esta línea
                entity.Ignore(e => e.IsNarcotic);      // ← Ignorar mapeo
                entity.Ignore(e => e.PsicoAnnexId);    // ← Ignorar mapeo
                entity.HasOne<Header>()
                    .WithMany()
                    .HasForeignKey(e => e.IdReceta)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<HeaderTextPlainResponse>().HasNoKey()
                .Property(e => e.PacPeso).HasPrecision(5, 2);
            modelBuilder.Entity<HeaderTextPlainResponse>()
                .Property(e => e.PacTalla).HasPrecision(5, 2);
            modelBuilder.Entity<HeaderTextPlainResponse>()
                .Property(e => e.PacCreatinina).HasPrecision(5, 2);

            modelBuilder.Entity<MedicamentoActivoResponse>().HasNoKey();

            modelBuilder.Entity<RecetaSqlModel>()
                .HasOne<Header>()
                .WithOne()
                .HasForeignKey<RecetaSqlModel>(r => r.IdReceta);

            modelBuilder.Entity<HeaderCounltResponse>().HasNoKey()
                .Property(e => e.PacPeso).HasPrecision(5, 2);
            modelBuilder.Entity<HeaderCounltResponse>()
                .Property(e => e.PacTalla).HasPrecision(5, 2);
            modelBuilder.Entity<HeaderCounltResponse>()
                .Property(e => e.PacCreatinina).HasPrecision(5, 2);

            modelBuilder.Entity<HeaderBySQL>().HasNoKey()
                .Property(e => e.PacPeso).HasPrecision(5, 2);
            modelBuilder.Entity<HeaderBySQL>()
                .Property(e => e.PacTalla).HasPrecision(5, 2);
            modelBuilder.Entity<HeaderBySQL>()
                .Property(e => e.PacCreatinina).HasPrecision(5, 2);

            modelBuilder.Entity<DetalleRecetaGet>().HasNoKey()
                .Property(e => e.CantidadDiaria).HasPrecision(5, 2);

            modelBuilder.Entity<DetalleCronico>(entity =>
            {
                entity.ToTable("DetalleCronico", "Receta");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CantidadDiaria).HasPrecision(10, 2);
            });

        }


    }
}