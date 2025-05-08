using Microsoft.EntityFrameworkCore;
using RMD.Models.Catalogo;
using RMD.Models.Pacientes;

namespace RMD.Data
{
    public class PacientesDbContext : DbContext
    {
        public PacientesDbContext(DbContextOptions<PacientesDbContext> options) : base(options)
        {
        }

        // DbSet para UsuarioPaciente
        public DbSet<UsuarioPaciente> UsuarioPacientes { get; set; }

        // DbSet para Paciente
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<PacienteConsultaRequest> PacienteConsultaRequest { get; set; }

        // Otros DbSet necesarios
        public DbSet<PacienteRequest> PacienteRequest { get; set; }

        // DbSet agregado para PacienteCreate
        public DbSet<PacienteCreate> PacienteCreate { get; set; }

        public DbSet<EntidadNacimiento> EntidadNacimiento { get; set; }

        // DbSets para alergias, moléculas y CIM10
        public DbSet<AllergyModel> AllergyModels { get; set; }
        public DbSet<MoleculeModel> MoleculeModels { get; set; }
        public DbSet<CIM10Model> CIM10Models { get; set; }

        // DbSet para EventosSalud y CatEventosDeSalud
        public DbSet<EventosSalud> EventosSalud { get; set; }
        public DbSet<EventosSaludConsulta> EventosSaludConsulta { get; set; }
        public DbSet<CatEventosDeSalud> CatEventosDeSalud { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para modelos sin clave primaria
            modelBuilder.Entity<PacienteRequest>().HasNoKey();
            modelBuilder.Entity<EntidadNacimiento>().HasNoKey();
            modelBuilder.Entity<PacienteCreate>().HasNoKey();
            modelBuilder.Entity<PacienteConsultaRequest>().HasNoKey();

            // Configuración para UsuarioPaciente
            modelBuilder.Entity<UsuarioPaciente>(entity =>
            {
                entity.HasKey(e => e.IdPaciente);

                entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PrimerApellido).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SegundoApellido).HasMaxLength(100);
                entity.Property(e => e.IdTipoIdentificacion).HasColumnType("integer");
                entity.Property(e => e.NumeroIdentificacion).HasMaxLength(50);
                entity.Property(e => e.TipoIdentificacion).HasMaxLength(50);
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
                entity.HasKey(e => e.IdPaciente);
                entity.Property(e => e.IdTipoIdentificacion).HasColumnType("integer");
                entity.Property(e => e.NumeroIdentificacion).HasMaxLength(50);
                entity.Property(e => e.FechaNacimiento).IsRequired();
                entity.Property(e => e.Genero).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Alergias).HasMaxLength(999999999);
                entity.Property(e => e.Molecules).HasMaxLength(999999999);
                entity.Property(e => e.Patologias).HasMaxLength(999999999);
            });

            // Configuración para AllergyModel
            modelBuilder.Entity<AllergyModel>(entity =>
            {
                entity.HasKey(e => e.IdAllergy);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.VidalUpdateDate).IsRequired();
            });

            // Configuración para MoleculeModel
            modelBuilder.Entity<MoleculeModel>(entity =>
            {
                entity.HasKey(e => e.IdMolecule);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.SafetyAlert).HasMaxLength(50);
                entity.Property(e => e.VidalUpdateDate).IsRequired();
            });

            // Configuración para CIM10Model
            modelBuilder.Entity<CIM10Model>(entity =>
            {
                entity.HasKey(e => e.IdCIM10);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.UpdatedDate).IsRequired();
            });

            // Configuración para CatEventosDeSalud (Catálogo de Eventos de Salud)
            modelBuilder.Entity<CatEventosDeSalud>(entity =>
            {
                entity.HasKey(e => e.IdEvento);
                entity.ToTable("CatEventosDeSalud");
                entity.Property(e => e.NombreEvento)
                      .IsRequired()
                      .HasColumnType("varchar(max)");
            });

            // Configuración para EventosSaludConsulta (modelo sin clave, usado solo en GET)
            modelBuilder.Entity<EventosSaludConsulta>().HasNoKey().ToView(null);

            // Configuración para EventosSalud
            modelBuilder.Entity<EventosSalud>(entity =>
            {
                entity.HasKey(e => e.IdEventoSalud);

                entity.Property(e => e.Descripcion)
                      .HasColumnType("varchar(max)");

                // Índices
                entity.HasIndex(e => new { e.EventoDeSalud, e.IdPaciente });
                entity.HasIndex(e => new { e.IdEventoSalud, e.IdPaciente, e.Fecha });
                entity.HasIndex(e => new { e.IdEventoSalud, e.IdPaciente, e.Fecha, e.EventoDeSalud });

                // Relación con CatEventosDeSalud
                entity.HasOne<CatEventosDeSalud>()
                      .WithMany()
                      .HasForeignKey(e => e.EventoDeSalud)
                      .HasConstraintName("FK_EventosSalud_CatEventosDeSalud");
            });
        }
    }
}
