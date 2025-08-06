using Microsoft.EntityFrameworkCore;

namespace RMD.Data
{
    public class PuntoVentaDbContext(DbContextOptions<PuntoVentaDbContext> options) : DbContext(options)
    {

        //// DbSet para las entidades principales
        //public DbSet<RecetaModel> Recetas { get; set; }
        //public DbSet<DetalleRecetaModel> DetalleRecetas { get; set; }
        //public DbSet<PacienteModel> Pacientes { get; set; }
        //public DbSet<MedicoModel> Medicos { get; set; }
        //public DbSet<GrupoEmpresarialModel> GrupoEmpresariales { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // Configuración de RecetaModel
        //    modelBuilder.Entity<RecetaModel>()
        //        .HasNoKey()
        //        .ToView(null); // Asume que solo se utiliza para consultas de procedimientos almacenados

        //    // Configuración de DetalleRecetaModel
        //    modelBuilder.Entity<DetalleRecetaModel>()
        //        .HasNoKey()
        //        .ToView(null); // También para procedimientos almacenados

        //    // Configuración de PacienteModel
        //    modelBuilder.Entity<PacienteModel>()
        //        .HasNoKey()
        //        .ToView(null);

        //    // Configuración de MedicoModel
        //    modelBuilder.Entity<MedicoModel>()
        //        .HasNoKey()
        //        .ToView(null);

        //    // Configuración de GrupoEmpresarialModel
        //    modelBuilder.Entity<GrupoEmpresarialModel>()
        //        .HasNoKey()
        //        .ToView(null);
        //}
    }
}
