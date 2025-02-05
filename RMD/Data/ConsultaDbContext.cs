using RMD.Models.Consulta;


namespace RMD.Data
{
    public class ConsultaDbContext(DbContextOptions<ConsultaDbContext> options) : DbContext(options)
    {

        // Tablas dinámicas basadas en SPs
        public DbSet<RequestSearchAllergy> Allergies { get; set; }
        public DbSet<RequestSearchMolecules> Molecules { get; set; }
        public DbSet<RequestSearchVMP> VMPS { get; set; }
        public DbSet<RequestSearchProducts> Products { get; set; }
        public DbSet<RequestSearchPackage> Packages { get; set; }
        public DbSet<RequestSearchCIM10> CIM10s { get; set; }
        public DbSet<DetalleRecetaResponse> DetalleRecetaResponse { get; set; }
        // Tablas Receta y DetalleReceta
        // Agrega estos DbSet en tu DbContext
        public DbSet<RecetaCreate> Recetas { get; set; }
        public DbSet<DetalleReceta> DetalleRecetas { get; set; }
        public DbSet<Medico> Medicos { get; set; }  // Agregamos Medicos
        public DbSet<SucursalModel> Sucursales { get; set; } // Agregamos Sucursales
        //public DbSet<UsuarioSucursal> UsuarioSucursales { get; set; } // Agregamos UsuarioSucursal
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<RecetaQR> RecetasQR { get; set; }
        public DbSet<RecetaGetSQL> RecetaGetSQL { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paciente>().ToTable("Pacientes").HasKey(p => p.IdPaciente);

            // Configuración de Medico
            modelBuilder.Entity<Medico>()
                .ToTable("Medicos")
                .HasKey(m => m.IdMedico);


            // Configuración de entidades sin claves primarias (consultas basadas en SPs)
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

            // Configuración de la tabla Receta
            modelBuilder.Entity<RecetaCreate>()
                .ToTable("Receta")
                .HasKey(r => r.IdReceta);  // Clave primaria

            // Configuración de la tabla DetalleReceta
            modelBuilder.Entity<DetalleReceta>()
                .ToTable("RecetaDetalle")
                .HasKey(dr => dr.IdDetalleReceta);  // Clave primaria

            // Relación entre Receta y DetalleReceta
            modelBuilder.Entity<DetalleReceta>()
                .HasOne<RecetaCreate>()  // Una receta tiene múltiples detalles
                .WithMany()  // Los detalles pertenecen a una receta
                .HasForeignKey(dr => dr.IdReceta)
                .OnDelete(DeleteBehavior.Cascade);  // Elimina detalles si se borra la receta

            // Configuración de la tabla Sucursal
            modelBuilder.Entity<SucursalModel>()
                .ToTable("Sucursales")
                .HasKey(s => s.IdSucursal);

            modelBuilder.Entity<RecetaQR>()
                .ToTable("RecetaQR")
                .HasKey(qr => qr.IdRecetaQR);  // Clave primaria

            modelBuilder.Entity<RecetaQR>()
            .HasOne<RecetaCreate>()  // Relación con la tabla Receta
            .WithMany()  // Una receta puede tener un QR asociado
            .HasForeignKey(qr => qr.IdReceta)  // Clave foránea
            .OnDelete(DeleteBehavior.Cascade);  // Borrar el QR si se elimina la receta

            modelBuilder.Entity<RecetaGet>()
                .HasNoKey()
                .ToView(null);
            modelBuilder.Entity<RecetaGetSQL>()
               .HasNoKey()
               .ToView(null);
            modelBuilder.Entity<DetalleRecetaGet>()
                .HasNoKey()
                .ToView(null);


        }
    }
}
