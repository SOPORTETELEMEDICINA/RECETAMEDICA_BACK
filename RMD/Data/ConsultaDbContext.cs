using RMD.Models.Consulta;
using RMD.Models.Sql;

namespace RMD.Data
{
    public class ConsultaDbContext : DbContext
    {
        public ConsultaDbContext(DbContextOptions<ConsultaDbContext> options) : base(options)
        {
        }

        // Tablas dinámicas basadas en SPs
        public DbSet<RequestSearchAllergy> Allergies { get; set; }
        public DbSet<RequestSearchMolecules> Molecules { get; set; }
        public DbSet<RequestSearchVMP> VMPS { get; set; }
        public DbSet<RequestSearchProducts> Products { get; set; }
        public DbSet<RequestSearchPackage> Packages { get; set; }
        public DbSet<RequestSearchCIM10> CIM10s { get; set; }
        public DbSet<DetalleRecetaResponse> DetalleRecetaResponse { get; set; }

        // Tablas Receta y DetalleReceta
        // Usamos RecetaCreate para inserciones y RecetaSqlModel para operaciones que requieran el mapeo completo
        public DbSet<RecetaCreate> Recetas { get; set; }
        public DbSet<RecetaSqlModel> RecetasSql { get; set; }
        public DbSet<DetalleReceta> DetalleRecetas { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<SucursalModel> Sucursales { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<RecetaQR> RecetasQR { get; set; }
        public DbSet<RecetaGetSQL> RecetaGetSQL { get; set; }


        public DbSet<BooleanResult> BooleanResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de la tabla Pacientes
            modelBuilder.Entity<Paciente>()
                .ToTable("Pacientes")
                .HasKey(p => p.IdPaciente);

            // Configuración de la tabla Medicos
            modelBuilder.Entity<Medico>()
                .ToTable("Medicos")
                .HasKey(m => m.IdMedico);

            // Entidades sin clave primaria (usadas en SPs)
            modelBuilder.Entity<RequestSearchAllergy>()
                .HasNoKey()
                .ToView(null);
            modelBuilder.Entity<DetalleRecetaResponse>()
                .HasNoKey()
                .ToView(null);
            modelBuilder.Entity<RequestSearchVMP>()
                .HasNoKey()
                .ToView(null);
            modelBuilder.Entity<RequestSearchProducts>()
                .HasNoKey()
                .ToView(null);
            modelBuilder.Entity<RequestSearchPackage>()
                .HasNoKey()
                .ToView(null);
            modelBuilder.Entity<RequestSearchMolecules>()
                .HasNoKey()
                .ToView(null);
            modelBuilder.Entity<RequestSearchCIM10>()
                .HasNoKey()
                .ToView(null);

            // Configuración de la tabla Receta usando RecetaCreate para inserciones
            modelBuilder.Entity<RecetaCreate>()
                .ToTable("Receta")
                .HasKey(r => r.IdReceta);

            // Configuración de la tabla Receta usando RecetaSqlModel para lecturas/operaciones completas
            modelBuilder.Entity<RecetaSqlModel>()
                .ToTable("Receta")
                .HasKey(r => r.IdReceta);

            // Configurar Table Splitting: ambas entidades comparten la misma clave primaria
            modelBuilder.Entity<RecetaSqlModel>()
                .HasOne<RecetaCreate>()
                .WithOne()
                .HasForeignKey<RecetaSqlModel>(r => r.IdReceta);

            // Configuración de la tabla DetalleReceta
            modelBuilder.Entity<DetalleReceta>()
                .ToTable("RecetaDetalle")
                .HasKey(dr => dr.IdDetalleReceta);

            // Relación entre DetalleReceta y Receta (usando RecetaCreate)
            modelBuilder.Entity<DetalleReceta>()
                .HasOne<RecetaCreate>()
                .WithMany()
                .HasForeignKey(dr => dr.IdReceta)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de la tabla Sucursales
            modelBuilder.Entity<SucursalModel>()
                .ToTable("Sucursales")
                .HasKey(s => s.IdSucursal);

            // Configuración de la tabla RecetaQR
            modelBuilder.Entity<RecetaQR>()
                .ToTable("RecetaQR")
                .HasKey(qr => qr.IdRecetaQR);
            modelBuilder.Entity<RecetaQR>()
                .HasOne<RecetaCreate>()
                .WithMany()
                .HasForeignKey(qr => qr.IdReceta)
                .OnDelete(DeleteBehavior.Cascade);

            // Entidades sin clave primaria para consultas vía SPs
            modelBuilder.Entity<RecetaGet>()
                .HasNoKey()
                .ToView(null);
            modelBuilder.Entity<RecetaGetSQL>()
                .HasNoKey()
                .ToView(null);
            modelBuilder.Entity<DetalleRecetaGet>()
                .HasNoKey()
                .ToView(null);

            modelBuilder.Entity<BooleanResult>()
                .HasNoKey()
                .ToView(null);

        }
    }
}
