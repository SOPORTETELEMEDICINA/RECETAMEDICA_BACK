using Microsoft.EntityFrameworkCore;
using RMD.Shared.Models.Catalogo;
using RMD.Shared.Models.Pacientes;
using RMD.Shared.Models.Pacientes.Request;
using RMD.Shared.Models.Pacientes.Response;

namespace RMD.Data
{
    public class PacientesDbContext : DbContext
    {
        public PacientesDbContext(DbContextOptions<PacientesDbContext> options) : base(options)
        {
        }
        public DbSet<EventosSaludRequest> EventosSalud { get; set; }
        public DbSet<CatEventosDeSalud> CatEventosDeSalud { get; set; }
        public DbSet<TokensQRPaciente> TokensQRPacientes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para modelos sin clave primaria
            //modelBuilder.Entity<PacienteRequest>().HasNoKey();
            modelBuilder.Entity<EntidadNacimientoResponse>().HasNoKey();
            modelBuilder.Entity<PacienteCreate>().HasNoKey();
            modelBuilder.Entity<PacienteConsultaResponse>().HasNoKey();

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
            modelBuilder.Entity<PacienteRequest>(entity =>
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
            modelBuilder.Entity<EventosSaludResponse>().HasNoKey().ToView(null);

            // Configuración para EventosSalud
            modelBuilder.Entity<EventosSaludRequest>(entity =>
            {
                entity.ToTable("EventosSalud"); // nombre real de la tabla
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
            modelBuilder.Entity<TokensQRPaciente>(entity =>
            {
                entity.ToTable("TokensQRPacientes", "PuntoVenta");

                entity.HasKey(e => e.IdToken);

                entity.Property(e => e.Token)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(e => e.FechaGeneracion)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.FechaExpiracion)
                      .IsRequired();

                entity.Property(e => e.Usado)
                      .HasDefaultValue(false);

                entity.Property(e => e.UsadoEn)
                      .IsRequired(false);

                entity.Property(e => e.IpUso)
                      .HasMaxLength(100)
                      .IsRequired(false);

                entity.Property(e => e.EndpointAccedido)
                      .HasMaxLength(255)
                      .IsRequired(false);
            });

        }
    }
}
