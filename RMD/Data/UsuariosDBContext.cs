using Microsoft.EntityFrameworkCore;
using RMD.Models.Consulta;
using RMD.Models.Login;
using RMD.Models.Usuarios;

namespace RMD.Data
{
    public class UsuariosDBContext : DbContext
    {
        public UsuariosDBContext(DbContextOptions<UsuariosDBContext> options) : base(options) { }

        public DbSet<CatGrupoEmpresarial> CatGrupoEmpresariales { get; set; }
        public DbSet<TipoUsuario> TipoUsuarios { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioSucursal> UsuarioSucursales { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
        public DbSet<UsuarioDetalle> UsuarioDetalle { get; set; }
        public DbSet<UsuarioImagenRequest> UsuarioImagenes { get; set; }
        public DbSet<SucursalResponse> SucursalResponses { get; set; }
        public DbSet<CrearPacienteRequest> CrearPacienteRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure CatGrupoEmpresarial
            modelBuilder.Entity<CatGrupoEmpresarial>(entity =>
            {
                entity.ToTable("CatGrupoEmpresarial");
                entity.HasKey(e => e.IdGEMP);

                // Configuración para el campo Nombre
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                // Configuración para el campo LogoBase64
                entity.Property(e => e.LogoBase64)
                    .HasColumnType("nvarchar(max)"); // Usamos nvarchar(max) para almacenar grandes cantidades de texto (como Base64)
            });


            // Configure TipoUsuario
            modelBuilder.Entity<TipoUsuario>(entity =>
            {
                entity.ToTable("TipoUsuario");
                entity.HasKey(e => e.IdTipoUsuario);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            });

            // Configure Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.Usr).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PrimerApellido).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            });

            // Configure UsuarioSucursal
            modelBuilder.Entity<UsuarioSucursal>(entity =>
            {
                entity.ToTable("UsuarioSucursal");
                entity.HasKey(e => new { e.IdUsuario, e.IdSucursal });
            });

            // Configuración para UsuarioDetalle
            modelBuilder.Entity<UsuarioDetalle>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.Usr).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PrimerApellido).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CodigoPostal).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Domicilio).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Movil).IsRequired().HasMaxLength(15);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            });

            // Configuración para UsuarioImagenRequest
            modelBuilder.Entity<UsuarioImagenRequest>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.Imagen).HasMaxLength(999999999);
                entity.Property(e => e.Firma).HasMaxLength(999999999);
            });

            // Configuración para SucursalResponse
            modelBuilder.Entity<SucursalResponse>(entity =>
            {
                entity.HasNoKey();  // SucursalResponse es un modelo de respuesta
            });

            // Configuración para CrearPacienteRequest
            modelBuilder.Entity<CrearPacienteRequest>(entity =>
            {
                entity.HasNoKey();  // CrearPacienteRequest es un modelo de solicitud
            });
        }
    }
}
