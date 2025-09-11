using Microsoft.EntityFrameworkCore;
using RMD.Shared.Models.Vidal.Package;
using RMD.Shared.Models.Vidal.Producto;

namespace RMD.Data
{
    public class VidalDBContext : DbContext
    {
        public VidalDBContext(DbContextOptions<VidalDBContext> options) : base(options)
        {
        }

        //// Tablas dinámicas (SP sin clave)
        //public DbSet<RequestSearchCIM10> CIM10s { get; set; }
        //public DbSet<RequestSearchAllergy> Allergies { get; set; }
        //public DbSet<RequestSearchMolecules> Molecules { get; set; }

        // Tablas fijas (con clave primaria)
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Package> Packages { get; set; }
        //public DbSet<VMP> VMPs { get; set; }
        //public DbSet<VMP_ATC> VMP_ATCs { get; set; }
        //public DbSet<ATCClassification> ATCClassifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //// Estas ya son tablas reales, sin PK definida
            //modelBuilder.Entity<RequestSearchMolecules>()
            //    .HasNoKey()
            //    .ToTable("Molecule", "VIDAL");

            //modelBuilder.Entity<RequestSearchCIM10>()
            //    .HasNoKey()
            //    .ToTable("CIM10", "VIDAL");

            //modelBuilder.Entity<RequestSearchAllergy>()
            //    .HasNoKey()
            //    .ToTable("Allergy", "VIDAL");

            // Entidades reales solo lectura
            modelBuilder.Entity<Producto>()
                .ToTable("Producto", "VIDAL")
                .HasNoKey();

            modelBuilder.Entity<Package>()
                .ToTable("Package", "VIDAL")
                .HasNoKey();

            //modelBuilder.Entity<VMP>()
            //    .ToTable("VMP", "VIDAL")
            //    .HasNoKey();
            //modelBuilder.Entity<VMP_ATC>()
            //    .ToTable("VMP_ATC", "VIDAL")
            //    .HasKey(x => new { x.AtcClassId, x.VmpId });

            //modelBuilder.Entity<ATCClassification>()
            //    .ToTable("ATCClassifications", "VIDAL")
            //    .HasKey(x => x.IdATC);

        }

    }
}