using Microsoft.EntityFrameworkCore;
using RMD.Models.Vidal.CargaCatalogos;

namespace RMD.Data
{
    public class VidalDbContext : DbContext
    {
        public VidalDbContext(DbContextOptions<VidalDbContext> options) : base(options) { }

        // Definir las tablas
        public DbSet<VMPModel> VMPs { get; set; }
        public DbSet<ProductModel> Productos { get; set; }   // Mapeo de la tabla Productos
        public DbSet<ATCClassificationModel> ATCClassifications { get; set; }   // Mapeo de la tabla Productos
      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Definir la tabla VMP y sus propiedades
            modelBuilder.Entity<VMPModel>(entity =>
            {
                entity.ToTable("VMPs"); // Nombre de la tabla

                // Definir la clave primaria
                entity.HasKey(e => e.IdVMP);

                // Definir propiedades y columnas
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ActivePrinciples)
                    .HasColumnType("NVARCHAR(MAX)");

                //entity.Property(e => e.RouteName)
                //    .HasMaxLength(255);

                entity.Property(e => e.GalenicForm)
                    .HasMaxLength(255);

                entity.Property(e => e.RegulatoryGenericPrescription)
                    .IsRequired();

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("VidalUpdateDate")
                    .IsRequired();

                //entity.Property(e => e.IdRoute)
                //    .IsRequired();

                entity.Property(e => e.IdVTM)
                    .IsRequired();

                // Índices
                entity.HasIndex(e => e.IdVMP).HasDatabaseName("IDX_VMP_IdVMP");          // Índice para buscar por IdVMP
                entity.HasIndex(e => e.Name).HasDatabaseName("IDX_VMP_Name");             // Índice para buscar por Name
                //entity.HasIndex(e => e.IdRoute).HasDatabaseName("IDX_VMP_IdRoute");       // Índice para buscar por IdRoute
                entity.HasIndex(e => e.IdVTM).HasDatabaseName("IDX_VMP_IdVTM");           // Índice para buscar por IdVTM
                entity.HasIndex(e => e.UpdatedDate).HasDatabaseName("IDX_VMP_UpdatedDate"); // Índice para buscar por UpdatedDate
            });


            // Configuración de la tabla Vidal_Producto
            modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.ToTable("Vidal_Producto");  // Nombre de la tabla en SQL

                entity.HasKey(e => e.IdProduct);  // Clave primaria
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);  // Nombre del producto es requerido

                entity.Property(e => e.Summary)
                    .HasColumnType("NVARCHAR(MAX)");  // Cambio a NVARCHAR(MAX)

                entity.Property(e => e.IdItemType)
                    .HasMaxLength(50);   // Máxima longitud para ItemType

                entity.Property(e => e.IdMarketStatus)
                    .HasMaxLength(50);   // Máxima longitud para MarketStatus

                entity.Property(e => e.BestDocType)
                    .HasMaxLength(50);   // Máxima longitud para BestDocType

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(255);  // Máxima longitud para CompanyName

                entity.Property(e => e.TypeCompany)
                    .HasMaxLength(50);   // Máxima longitud para TypeCompany

                entity.Property(e => e.GalenicForm)
                    .HasMaxLength(255);  // Máxima longitud para GalenicForm

                // Configuración de campos adicionales
                entity.Property(e => e.VidalUpdateDate)
                    .IsRequired();  // Fecha de actualización de Vidal es obligatoria

                // Índices para búsqueda eficiente
                entity.HasIndex(e => e.IdProduct)
                    .HasDatabaseName("IX_Vidal_Producto_IdProduct");

                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("IX_Vidal_Producto_Name");

                entity.HasIndex(e => e.IdMarketStatus)
                    .HasDatabaseName("IX_Vidal_Producto_IdMarketStatus");

                entity.HasIndex(e => e.IdCompany)
                    .HasDatabaseName("IX_Vidal_Producto_IdCompany");

                entity.HasIndex(e => e.VidalUpdateDate)
                    .HasDatabaseName("IX_Vidal_Producto_VidalUpdateDate");

                entity.HasIndex(e => e.IdVmp)
                    .HasDatabaseName("IX_Vidal_Producto_IdVmp");

            });


            modelBuilder.Entity<ATCClassificationModel>(entity =>
            {
                entity.ToTable("Vidal_ATCClassifications");

                entity.HasKey(e => e.IdATC);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate)
                    .IsRequired();

                entity.HasIndex(e => e.IdATC)
                    .HasDatabaseName("IX_ATCClassification_IdATC");

                entity.HasIndex(e => e.Code)
                    .HasDatabaseName("IX_ATCClassification_Code");
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}

