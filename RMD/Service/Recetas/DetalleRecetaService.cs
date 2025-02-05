using RMD.Data;
using RMD.Interface.Recetas;
using RMD.Models.Recetas;

namespace RMD.Service.Recetas
{
    public class DetalleRecetaService : IDetalleRecetaService
    {
        private readonly RecetasDbContext _context;

        public DetalleRecetaService(RecetasDbContext context)
        {
            _context = context;
        }

        public async Task<DetalleReceta> GetDetalleRecetaByIdAsync(Guid idDetalleReceta)
        {
            return await _context.DetalleRecetas
                .FirstOrDefaultAsync(dr => dr.IdDetalleReceta == idDetalleReceta);
        }

        public async Task<IEnumerable<DetalleReceta>> GetDetalleRecetasByRecetaAsync(Guid idReceta)
        {
            return await _context.DetalleRecetas
                .Where(dr => dr.IdReceta == idReceta)
                .ToListAsync();
        }


        public async Task<DetalleRecetaResponse> GetReaccionByIdRecetaAsync(DetalleRecetaRequest request, Guid IdUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("DetalleReceta_GetPacienteReaccion", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdDetalleReceta", request.IdDetalleReceta);
                command.Parameters.AddWithValue("@IdReceta", request.IdReceta);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new DetalleRecetaResponse
                    {
                        IdDetalleReceta = reader.GetGuid(reader.GetOrdinal("IdDetalleReceta")),
                        IdReceta = reader.GetGuid(reader.GetOrdinal("IdReceta")),
                        MedicamentoId = reader.GetInt32(reader.GetOrdinal("MedicamentoId")),
                        MedicamentoType = reader.GetString(reader.GetOrdinal("MedicamentoType")),
                        Descripcion = reader.GetString(reader.GetOrdinal("Descripcion"))
                    };
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error en la ejecución del procedimiento almacenado.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Se produjo un error al obtener la reacción de la receta.", ex);
            }
            return null;
        }

        public async Task<(bool, string)> CreateUpdateReaccionAsync(DetalleRecetaRequest request, Guid IdUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("DetalleReceta_CreateUpdatePacienteReaccion", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdDetalleReceta", request.IdDetalleReceta);
                command.Parameters.AddWithValue("@IdReceta", request.IdReceta);
                command.Parameters.AddWithValue("@MedicamentoId", request.MedicamentoId);
                command.Parameters.AddWithValue("@MedicamentoType", request.MedicamentoType);
                command.Parameters.AddWithValue("@Descripcion", request.Descripcion);
                command.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                command.Parameters.Add("@OutMessage", SqlDbType.VarChar, -1).Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();
                string outMessage = command.Parameters["@OutMessage"].Value.ToString();
                return (outMessage.Contains("correctamente"), outMessage);
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error en la ejecución del procedimiento almacenado.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Se produjo un error al crear o actualizar la reacción de la receta.", ex);
            }
        }

        public async Task<(bool, string)> DeleteReaccionAsync(DetalleRecetaRequest request, Guid IdUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand("DetalleReceta_DeletePacienteReaccion", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdDetalleReceta", request.IdDetalleReceta);
                command.Parameters.AddWithValue("@IdReceta", request.IdReceta);
                command.Parameters.AddWithValue("@MedicamentoId", request.MedicamentoId);
                command.Parameters.AddWithValue("@MedicamentoType", request.MedicamentoType);
                command.Parameters.AddWithValue("@Descripcion", request.Descripcion);
                command.Parameters.AddWithValue("@IdUsuario", IdUsuario);
                command.Parameters.Add("@OutMessage", SqlDbType.VarChar, -1).Direction = ParameterDirection.Output;

                await command.ExecuteNonQueryAsync();
                string outMessage = command.Parameters["@OutMessage"].Value.ToString();
                return (outMessage.Contains("correctamente"), outMessage);
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Error en la ejecución del procedimiento almacenado.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("Se produjo un error al eliminar la reacción de la receta.", ex);
            }
        }
    }
}
