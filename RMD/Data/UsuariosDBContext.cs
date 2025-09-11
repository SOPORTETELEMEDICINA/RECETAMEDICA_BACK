using Microsoft.EntityFrameworkCore;
using RMD.Shared.Models.Login;
using RMD.Shared.Models.Sucursales;
using RMD.Shared.Models.Usuarios;

namespace RMD.Data
{
    public class UsuariosDBContext(DbContextOptions<UsuariosDBContext> options) : DbContext(options)
    {
        public DbSet<CatGrupoEmpresarial> CatGrupoEmpresariales { get; set; }
    
        public DbSet<AuthToken> AuthTokens { get; set; }
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
                entity.Property(e => e.Abreviatura)
                   .HasColumnType("nvarchar(3)");
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
                entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PrimerApellido).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
            });

            // Configure UsuarioSucursal
            // Configurar la tabla relacional UsuarioSucursal
            modelBuilder.Entity<UsuarioSucursal>(entity =>
            {
                entity.ToTable("UsuarioSucursal");

                // Definir clave primaria compuesta
                entity.HasKey(e => new { e.IdUsuario, e.IdSucursal });

                // Definir las relaciones (Foreign Keys)
                entity.HasOne<Usuario>()
                      .WithMany()
                      .HasForeignKey(e => e.IdUsuario)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Sucursal>()
                      .WithMany()
                      .HasForeignKey(e => e.IdSucursal)
                      .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity<RequestUsuario>(entity =>
            {
                entity.HasNoKey();  
            });
            // AuthTokens
            modelBuilder.Entity<AuthToken>(entity =>
            {
                entity.ToTable("AuthTokens", "auth");
                entity.HasKey(e => e.IdToken);

                entity.Property(e => e.IdToken)
                      .HasColumnName("IdToken")
                      .IsRequired();

                entity.Property(e => e.IdUsuario)
                      .HasColumnName("IdUsuario")
                      .IsRequired();

                entity.Property(e => e.Token)
                      .HasColumnName("Token")
                      .IsRequired()
                      .HasColumnType("nvarchar(max)");

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("CreatedAt")
                      .IsRequired();

                entity.Property(e => e.ExpiresAt)
                      .HasColumnName("ExpiresAt")
                      .IsRequired();

                entity.Property(e => e.RevokedAt)
                      .HasColumnName("RevokedAt");

                entity.Property(e => e.LastAuth2F)
                      .HasColumnName("LastAuth2F");
            });
        }
    }
}
