using Microsoft.EntityFrameworkCore;
using RMD.Models.Pacientes;

namespace RMD.Data
{
    public class PacientesDbContext : DbContext
    {
        public PacientesDbContext(DbContextOptions<PacientesDbContext> options) : base(options) { }

        // DbSet para UsuarioPaciente
        public DbSet<UsuarioPaciente> UsuarioPacientes { get; set; }

        // DbSet para Paciente
        public DbSet<Paciente> Pacientes { get; set; }

        // Otros DbSet necesarios
        public DbSet<PacienteRequest> PacienteRequest { get; set; }

        // DbSet agregado para PacienteCreate
        public DbSet<PacienteCreate> PacienteCreate { get; set; }

        public DbSet<EntidadNacimiento> EntidadNacimiento { get; set; }

        // DbSets para alergias, moléculas y CIM10
        public DbSet<AllergyModel> AllergyModels { get; set; }
        public DbSet<MoleculeModel> MoleculeModels { get; set; }
        public DbSet<CIM10Model> CIM10Models { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para PacienteRequest
            modelBuilder.Entity<PacienteRequest>().HasNoKey();  // Modelo sin clave primaria
            modelBuilder.Entity<EntidadNacimiento>().HasNoKey();  // Modelo sin clave primaria

            // Configuración para UsuarioPaciente
            modelBuilder.Entity<UsuarioPaciente>(entity =>
            {
                entity.HasKey(e => e.IdPaciente);  // Clave primaria

                entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PrimerApellido).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SegundoApellido).HasMaxLength(100);
                entity.Property(e => e.Genero).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Alergias).HasMaxLength(999999999);
                entity.Property(e => e.Molecules).HasMaxLength(999999999);
                entity.Property(e => e.Patologias).HasMaxLength(999999999);
                entity.Property(e => e.Movil).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Domicilio).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Asentamiento).HasMaxLength(100);
                entity.Property(e => e.TipoAsentamiento).HasMaxLength(50);
                entity.Property(e => e.CodigoPostal).HasMaxLength(10);
                entity.Property(e => e.Municipio).HasMaxLength(100);
                entity.Property(e => e.Ciudad).HasMaxLength(100);
                entity.Property(e => e.Estado).HasMaxLength(100);
            });

            // Configuración para Paciente
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.ToTable("Pacientes");
                entity.HasKey(e => e.IdPaciente);  // Clave primaria

                entity.Property(e => e.FechaNacimiento).IsRequired();
                entity.Property(e => e.Genero).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Alergias).HasMaxLength(999999999);
                entity.Property(e => e.Molecules).HasMaxLength(999999999);
                entity.Property(e => e.Patologias).HasMaxLength(999999999);
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
            // Configuración para PacienteCreate
            modelBuilder.Entity<PacienteCreate>().HasNoKey();  // Modelo sin clave primaria
        }
    }
}
