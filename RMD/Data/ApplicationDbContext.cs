using RMD.Models.CatalogoErrors;
using RMD.Models.Login;
using RMD.Models.Responses;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RegistroPeticion> RegistroPeticiones { get; set; }
    public DbSet<ErrorCatalog> CatalogoErrores { get; set; }
    public DbSet<CatalogoNotificacion> CatalogoNotificaciones { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RegistroPeticion>().ToTable("RegistroPeticiones");
        modelBuilder.Entity<ErrorCatalog>(entity =>
        {
            entity.ToTable("CatalogoErrores", "Configuracion"); // Mapea al esquema "Configuracion"
            entity.HasKey(e => e.CodigoError); // Especifica la clave primaria
        });

        modelBuilder.Entity<CatalogoNotificacion>(entity =>
        {
            entity.ToTable("CatalogoNotificaciones", "Configuracion"); // Mapea al esquema "Configuracion"
            entity.HasKey(e => e.CodigoNotificacion); // Especifica la clave primaria
        });
    }
    
}
