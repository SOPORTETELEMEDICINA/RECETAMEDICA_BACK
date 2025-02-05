using RMD.Models.Login;
using RMD.Models.Sucursales;
using RMD.Models.Usuarios;

namespace RMD.Data
{
    public class UsuariosDBContext(DbContextOptions<UsuariosDBContext> options) : DbContext(options)
    {
        public DbSet<CatGrupoEmpresarial> CatGrupoEmpresariales { get; set; }
        public DbSet<TipoUsuario> TipoUsuarios { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RequestUsuario> RequestUsuario { get; set; }
        
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
            // Configuración para BlacklistedTokens
            modelBuilder.Entity<BlacklistedToken>(entity =>
            {
                entity.ToTable("BlacklistedTokens"); // Asegúrate de que coincide con el nombre de la tabla en la base de datos
                entity.HasKey(e => e.Id); // Configura Id como clave primaria
                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)"); // Permitir valores largos
                entity.Property(e => e.ExpirationDate)
                      .IsRequired(); // Asegura que el campo es obligatorio
            });
        }
    }
}
