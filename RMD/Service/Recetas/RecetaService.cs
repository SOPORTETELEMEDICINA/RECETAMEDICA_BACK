using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions;
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

        public async Task<bool> CreateRecetaAsync(RecetaModel recetaModel, List<RecetaDetalleModel> recetaDetalles)
        {
            try
            {
                // Convertir RecetaModel a DataTable
                var recetaTable = new List<RecetaModel> { recetaModel }.ToDataTable();
                var recetaTableParam = new SqlParameter("@RecetaTable", SqlDbType.Structured)
                {
                    TypeName = "dbo.RecetaTableType", // Usar el Table Type correcto
                    Value = recetaTable
                };

                // Convertir RecetaDetalleModel a DataTable
                var recetaDetalleTable = recetaDetalles.ToDataTable();
                var recetaDetalleTableParam = new SqlParameter("@RecetaDetalleTable", SqlDbType.Structured)
                {
                    TypeName = "dbo.RecetaDetalleTableType", // Usar el Table Type correcto
                    Value = recetaDetalleTable
                };

                // Parámetro para mensaje de salida
                var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };

                // Ejecutar el stored procedure
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Receta_CreateReceta @RecetaTable, @RecetaDetalleTable, @OutputMessage OUTPUT",
                    recetaTableParam, recetaDetalleTableParam, outputMessageParam
                );

                // Leer el mensaje de salida
                var outputMessage = outputMessageParam.Value.ToString();
                return outputMessage.Contains("éxito");
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al crear la receta: {ex.Message}");
                return false;
            }
        }

        public async Task<RecetaRecibidaModel> GetRecetaByIdRecetaByMedicoAsync(Guid idReceta, Guid idUsuario)
        {
            var idRecetaParam = new SqlParameter("@IdReceta", idReceta);
            var idUsuarioParam = new SqlParameter("@IdUsuario", idUsuario);
            var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            // Ejecutar el SP para obtener la receta y sus detalles
            var receta = await _context.RecetaWithDetalles
                .FromSqlRaw("EXEC Receta_GetByIdRecetaByMedico @IdReceta, @IdUsuario, @OutputMessage OUTPUT",
                            idRecetaParam, idUsuarioParam, outputMessageParam)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var outputMessage = outputMessageParam.Value.ToString();
            Console.WriteLine($"Resultado del SP: {outputMessage}");

            if (receta == null || outputMessage.Contains("Error"))
            {
                return null;
            }

            // Obtener las listas de alergias, molecules y patologías
            var alergias = await ObtenerAlergiasPorIdsAsync(receta.Alergias);  // Debería devolver List<AllergyModel>
            var molecules = await ObtenerMoleculesPorIdsAsync(receta.Molecules);  // Debería devolver List<MoleculeModel>
            var patologias = await ObtenerCIM10PorIdsAsync(receta.Patologias);  // Debería devolver List<CIM10Model>

            // Crear el modelo final RecetaRecibidaModel con los detalles y las listas de alergias, molecules, y patologías
            var recetaRecibida = new RecetaRecibidaModel
            {
                IdReceta = receta.IdReceta == Guid.Empty ? Guid.NewGuid() : receta.IdReceta,  // Si es Guid.Empty, generamos un nuevo GUID
                IdMedico = receta.IdMedico,
                IdPaciente = receta.IdPaciente,
                PacPeso = receta.PacPeso,  // Sin ?? si es un decimal no nullable
                PacTalla = receta.PacTalla,  // Sin ?? si es un decimal no nullable
                PacEmbarazo = receta.PacEmbarazo,
                PacSemAmenorrea = receta.PacSemAmenorrea ?? 0,  // Si es nullable, usamos 0 como valor por defecto
                PacLactancia = receta.PacLactancia,
                PacCreatinina = receta.PacCreatinina ?? 0m,  // Si es nullable, usamos 0m como valor por defecto
                Alergias = alergias,  // Asigna la lista correcta de AllergyModel
                Molecules = molecules,  // Asigna la lista correcta de MoleculeModel
                Patologias = patologias,  // Lista de CIM10Model obtenida del SP
                IdSucursal = receta.IdSucursal,
                IdGEMP = receta.IdGEMP,
                DetallesReceta = receta.DetallesReceta.Select(detalle => new RecetaDetalleRecibidaModel
                {
                    IdDetalleReceta = detalle.IdDetalleReceta ?? Guid.NewGuid(),  // Si es nullable, generamos un nuevo GUID
                    Medicamento = detalle.Medicamento,
                    CantidadDiaria = detalle.CantidadDiaria ?? 0m,  // Si es nullable, asignar 0m
                    UnidadDispensacion = detalle.UnidadDispensacion,
                    RutaAdministracion = detalle.RutaAdministracion,
                    Indicacion = detalle.Indicacion,
                    Duracion = detalle.Duracion ?? 0,  // Si es nullable, asignar 0
                    UnidadDuracion = detalle.UnidadDuracion,
                    PeriodoInicio = detalle.PeriodoInicio ?? DateTime.Now,  // Si es nullable, asignar fecha actual
                    PeriodoTerminacion = detalle.PeriodoTerminacion ?? DateTime.Now  // Si es nullable, asignar fecha actual
                }).ToList()  // Detalles de receta obtenidos directamente del SP
            };

            return recetaRecibida;
        }

        public async Task<bool> UpdateRecetaAsync(RecetaModel recetaModel, List<RecetaDetalleModel> recetaDetalles, Guid idUsuario)
        {
            try
            {
                // Convertir RecetaModel a DataTable
                var recetaTable = new List<RecetaModel> { recetaModel }.ToDataTable();
                var recetaTableParam = new SqlParameter("@RecetaTable", SqlDbType.Structured)
                {
                    TypeName = "dbo.RecetaTableType",  // Usar el TableType correcto
                    Value = recetaTable
                };

                // Convertir RecetaDetalleModel a DataTable
                var recetaDetalleTable = recetaDetalles.ToDataTable();
                var recetaDetalleTableParam = new SqlParameter("@RecetaDetalleTable", SqlDbType.Structured)
                {
                    TypeName = "dbo.RecetaDetalleTableType",  // Usar el TableType correcto
                    Value = recetaDetalleTable
                };

                // Parámetro para el IdUsuario
                var idUsuarioParam = new SqlParameter("@IdUsuario", idUsuario);

                // Parámetro para el mensaje de salida
                var outputMessageParam = new SqlParameter("@OutputMessage", SqlDbType.NVarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };

                // Ejecutar el stored procedure de actualización
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Receta_UpdateReceta @IdUsuario, @RecetaTable, @RecetaDetalleTable, @OutputMessage OUTPUT",
                    idUsuarioParam, recetaTableParam, recetaDetalleTableParam, outputMessageParam
                );

                // Leer el mensaje de salida
                var outputMessage = outputMessageParam.Value.ToString();
                Console.WriteLine($"Resultado del SP: {outputMessage}");

                // Retornar si fue exitoso
                return outputMessage.Contains("éxito");
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al actualizar la receta: {ex.Message}");
                return false;
            }
        }























        public async Task<Receta> GetRecetaByIdAsync(Guid idReceta)
        {
            var idParam = new SqlParameter("@IdReceta", idReceta);

            var results = await _context.Receta
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

            return await _context.Receta
                .FromSqlRaw("EXEC GetRecetasByMedico @IdMedico", idParam)
                .ToListAsync();
        }

        public async Task<IEnumerable<Receta>> GetRecetasByPacienteAsync(Guid idPaciente)
        {
            var idParam = new SqlParameter("@IdPaciente", idPaciente);

            return await _context.Receta
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


        private async Task<List<AllergyModel>> ObtenerAlergiasPorIdsAsync(string alergiasIds)
        {
            if (string.IsNullOrEmpty(alergiasIds))
            {
                // Si alergiasIds está vacío o es null, devolvemos una lista vacía.
                return new List<AllergyModel>();
            }

            var alergiasParam = new SqlParameter("@Ids", alergiasIds);

            return await _context.AllergyModels
                .FromSqlRaw("EXEC Vidal_GetAllergiesByIds @Ids", alergiasParam)
                .ToListAsync();
        }

        private async Task<List<MoleculeModel>> ObtenerMoleculesPorIdsAsync(string moleculesIds)
        {
            if (string.IsNullOrEmpty(moleculesIds))
            {
                // Si moleculesIds está vacío o es null, devolvemos una lista vacía.
                return new List<MoleculeModel>();
            }

            var moleculesParam = new SqlParameter("@Ids", moleculesIds);

            return await _context.MoleculeModels
                .FromSqlRaw("EXEC Vidal_GetMoleculesByIds @Ids", moleculesParam)
                .ToListAsync();
        }

        private async Task<List<CIM10Model>> ObtenerCIM10PorIdsAsync(string cim10Ids)
        {
            if (string.IsNullOrEmpty(cim10Ids))
            {
                // Si cim10Ids está vacío o es null, devolvemos una lista vacía.
                return new List<CIM10Model>();
            }

            var cim10Param = new SqlParameter("@Ids", cim10Ids);

            return await _context.CIM10Models
                .FromSqlRaw("EXEC Vidal_GetCIM10ByIds @Ids", cim10Param)
                .ToListAsync();
        }
    }
}
