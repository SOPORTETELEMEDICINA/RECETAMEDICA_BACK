using Microsoft.EntityFrameworkCore;
using RMD.Models.Consulta;

namespace RMD.Data
{
    public class ConsultaDbContext : DbContext
    {
        public ConsultaDbContext(DbContextOptions<ConsultaDbContext> options) : base(options) { }

        public DbSet<RequestSearchAllergy> Allergies { get; set; } // Refleja el resultado del SP
        public DbSet<RequestSearchMolecules> Molecules { get; set; } // Refleja el resultado del SP
        public DbSet<RequestSearchVMP> VMPS { get; set; } // Refleja el resultado del SP
        public DbSet<RequestSearchProducts> Products { get; set; } // Refleja el resultado del SP
        public DbSet<RequestSearchPackage> Packages { get; set; }
        public DbSet<RequestSearchCIM10> CIM10s { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestSearchAllergy>()
                .HasNoKey() // No tiene una clave primaria ya que es una consulta
                .ToView(null); // Indica que no es una tabla mapeada
            modelBuilder.Entity<RequestSearchVMP>()
                .HasNoKey() // No tiene una clave primaria ya que es una consulta
                .ToView(null); // Indica que no es una tabla mapeada
            modelBuilder.Entity<RequestSearchProducts>()
               .HasNoKey() // No tiene una clave primaria ya que es una consulta
               .ToView(null); // Indica que no es una tabla mapeada
            modelBuilder.Entity<RequestSearchPackage>()
             .HasNoKey() // No tiene una clave primaria ya que es una consulta
             .ToView(null); // Indica que no es una tabla mapeada
            modelBuilder.Entity<RequestSearchMolecules>()
             .HasNoKey() // No tiene una clave primaria ya que es una consulta
             .ToView(null); // Indica que no es una tabla mapeada            
        }
    }
}
