using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Recetas;
using RMD.Models.Recetas;
using System.Data;

namespace RMD.Service.Recetas
{
    public class RecetaService : IRecetaService
    {
        private readonly RecetasDbContext _context;

        public RecetaService(RecetasDbContext context)
        {
            _context = context;
        }

        public async Task<Receta> GetRecetaByIdAsync(Guid idReceta)
        {
            var idParam = new SqlParameter("@IdReceta", idReceta);

            var results = await _context.Recetas
                .FromSqlRaw("EXEC GetRecetaById @IdReceta", idParam)
                .ToListAsync();

            if (results.Count == 0)
            {
                throw new KeyNotFoundException($"No se encontró una receta con el Id {idReceta}");
            }

            return results[0];
        }

        public async Task<IEnumerable<Receta>> GetRecetasByMedicoAsync(Guid idMedico)
        {
            var idParam = new SqlParameter("@IdMedico", idMedico);

            return await _context.Recetas
                .FromSqlRaw("EXEC GetRecetasByMedico @IdMedico", idParam)
                .ToListAsync();
        }

        public async Task<IEnumerable<Receta>> GetRecetasByPacienteAsync(Guid idPaciente)
        {
            var idParam = new SqlParameter("@IdPaciente", idPaciente);

            return await _context.Recetas
                .FromSqlRaw("EXEC GetRecetasByPaciente @IdPaciente", idParam)
                .ToListAsync();
        }

        public async Task<bool> CreateRecetaYDetalleRecetaAsync(Receta receta, List<DetalleReceta> detalles)
        {
            var recetaTable = new DataTable();
            recetaTable.Columns.Add("IdMedico", typeof(Guid));
            recetaTable.Columns.Add("IdPaciente", typeof(Guid));
            recetaTable.Columns.Add("PacPeso", typeof(decimal));
            recetaTable.Columns.Add("PacTalla", typeof(decimal));
            recetaTable.Columns.Add("PacEmbarazo", typeof(bool));
            recetaTable.Columns.Add("PacSemAmenorrea", typeof(int));
            recetaTable.Columns.Add("PacLactancia", typeof(bool));
            recetaTable.Columns.Add("PacCreatinina", typeof(decimal));
            recetaTable.Columns.Add("PacAlergiaClase", typeof(bool));
            recetaTable.Columns.Add("PacAlergiaMolecula", typeof(bool));
            recetaTable.Columns.Add("PacDx1", typeof(string));
            recetaTable.Columns.Add("PacDx2", typeof(string));
            recetaTable.Columns.Add("PacDx3", typeof(string));
            recetaTable.Columns.Add("PacDx4", typeof(string));
            recetaTable.Columns.Add("PacDx5", typeof(string));

            recetaTable.Rows.Add(receta.IdMedico, receta.IdPaciente, receta.PacPeso, receta.PacTalla, receta.PacEmbarazo, receta.PacSemAmenorrea, receta.PacLactancia, receta.PacCreatinina, receta.PacAlergiaClase, receta.PacAlergiaMolecula, receta.PacDx1, receta.PacDx2, receta.PacDx3, receta.PacDx4, receta.PacDx5);

            var detalleRecetaTable = new DataTable();
            detalleRecetaTable.Columns.Add("Medicamento", typeof(string));
            detalleRecetaTable.Columns.Add("CantidadDiaria", typeof(decimal));
            detalleRecetaTable.Columns.Add("UnidadDispensacion", typeof(string));
            detalleRecetaTable.Columns.Add("RutaAdministracion", typeof(string));
            detalleRecetaTable.Columns.Add("Indicacion", typeof(string));
            detalleRecetaTable.Columns.Add("Duracion", typeof(int));
            detalleRecetaTable.Columns.Add("UnidadDuracion", typeof(string));
            detalleRecetaTable.Columns.Add("PeriodoInicio", typeof(DateTime));
            detalleRecetaTable.Columns.Add("PeriodoTerminacion", typeof(DateTime));

            foreach (var detalle in detalles)
            {
                detalleRecetaTable.Rows.Add(detalle.Medicamento, detalle.CantidadDiaria, detalle.UnidadDispensacion, detalle.RutaAdministracion, detalle.Indicacion, detalle.Duracion, detalle.UnidadDuracion, detalle.PeriodoInicio, detalle.PeriodoTerminacion);
            }

            var recetaParam = new SqlParameter("@RecetaTable", SqlDbType.Structured)
            {
                TypeName = "dbo.RecetaTableType",
                Value = recetaTable
            };

            var detalleRecetaParam = new SqlParameter("@DetalleRecetaTable", SqlDbType.Structured)
            {
                TypeName = "dbo.DetalleRecetaTableType",
                Value = detalleRecetaTable
            };

            var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                "EXEC CreateRecetaYDetalleReceta @RecetaTable, @DetalleRecetaTable",
                recetaParam, detalleRecetaParam);

            return rowsAffected > 0;
        }
    }
}
