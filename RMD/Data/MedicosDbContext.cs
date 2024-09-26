using Microsoft.EntityFrameworkCore;
using RMD.Models.Medicos;

namespace RMD.Data
{
    public class MedicosDbContext : DbContext
    {
        public MedicosDbContext(DbContextOptions<MedicosDbContext> options) : base(options) { }

        public DbSet<Medico> Medicos { get; set; }
        public DbSet<UsuarioMedico> UsuarioMedico { get; set; }
        public DbSet<MedicoCreate> MedicoCreate { get; set; }  // DbSet agregado
        public DbSet<PacientePorSucursalModel> PacientesPorSucursal { get; set; }
        public DbSet<PacientePorSucursalListModel> PacientesPorSucursalList { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Medico>(entity =>
            {
                entity.ToTable("Medicos");
                entity.HasKey(e => e.IdMedico);
                entity.Property(e => e.CedulaGeneral).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Universidad).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Especialidad).HasMaxLength(100);
                entity.Property(e => e.CedulaEspecialidad).HasMaxLength(50);
                entity.Property(e => e.Horario).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<UsuarioMedico>(entity =>
            {
                entity.ToTable("UsuariosMedicos");
                entity.HasKey(e => e.IdMedico);
                entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PrimerApellido).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SegundoApellido).HasMaxLength(100);
                entity.Property(e => e.CedulaGeneral).HasMaxLength(50);
                entity.Property(e => e.Universidad).HasMaxLength(150);
                entity.Property(e => e.Especialidad).HasMaxLength(150);
                entity.Property(e => e.CedulaEspecialidad).HasMaxLength(50);
                entity.Property(e => e.Horario).HasMaxLength(250);
                entity.Property(e => e.Movil).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.Domicilio).HasMaxLength(250);
                entity.Property(e => e.Asentamiento).HasMaxLength(150);
                entity.Property(e => e.TipoAsentamiento).HasMaxLength(50);
                entity.Property(e => e.CodigoPostal).HasMaxLength(10);
                entity.Property(e => e.Municipio).HasMaxLength(150);
                entity.Property(e => e.Ciudad).HasMaxLength(150);
                entity.Property(e => e.Estado).HasMaxLength(150);
            });

            // Configuración para MedicoCreate
            modelBuilder.Entity<MedicoCreate>().HasNoKey();  // Modelo sin clave primaria
            modelBuilder.Entity<PacientePorSucursalModel>().HasNoKey();
            modelBuilder.Entity<PacientePorSucursalListModel>().HasNoKey();
        }
    }
}
