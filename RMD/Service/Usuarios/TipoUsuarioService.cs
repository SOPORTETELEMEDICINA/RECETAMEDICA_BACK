using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions; // Asegúrate de tener las extensiones necesarias para ToDataTable()
using RMD.Interface.Usuarios;
using RMD.Models.Usuarios;
using System.Data;

namespace RMD.Service.Usuarios
{
    public class TipoUsuarioService : ITipoUsuarioService
    {
        private readonly UsuariosDBContext _context;

        public TipoUsuarioService(UsuariosDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoUsuario>> GetAllTipoUsuario()
        {
            try
            {
                return await _context.TipoUsuarios
                    .FromSqlRaw("EXEC sp_Cat_GetAllTipoUsuario")
                    .ToListAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error en la base de datos: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todos los tipos de usuario: {ex.Message}");
            }
        }

        public async Task<TipoUsuario> GetTipoUsuarioById(Guid id)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@IdTipoUsuario", id)
                };

                var result = await _context.TipoUsuarios
                    .FromSqlRaw("EXEC sp_Cat_GetTipoUsuarioById @IdTipoUsuario", parameters)
                    .ToListAsync();

                if (result.Count > 0)
                {
                    return result.First();
                }
                else
                {
                    throw new KeyNotFoundException("No existe el Tipo de Usuario especificado.");
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error en la base de datos: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el Tipo de Usuario: {ex.Message}");
            }
        }

        public async Task<string> CreateTipoUsuario(TipoUsuario tipoUsuario)
        {
            try
            {
                var parameter = new SqlParameter("@TipoUsuario", SqlDbType.Structured)
                {
                    TypeName = "dbo.TipoUsuarioType",
                    Value = new List<TipoUsuario> { tipoUsuario }.ToDataTable()
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Cat_CreateTipoUsuario @TipoUsuario", parameter);
                return "Tipo de Usuario creado con éxito.";
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error en la base de datos: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear el Tipo de Usuario: {ex.Message}");
            }
        }

        public async Task<string> UpdateTipoUsuario(TipoUsuario tipoUsuario)
        {
            try
            {
                var parameter = new SqlParameter("@TipoUsuario", SqlDbType.Structured)
                {
                    TypeName = "dbo.TipoUsuarioType",
                    Value = new List<TipoUsuario> { tipoUsuario }.ToDataTable()
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_Cat_UpdateTipoUsuario @TipoUsuario", parameter);
                return "Tipo de Usuario actualizado con éxito.";
            }
            catch (SqlException sqlEx)
            {
                throw new Exception($"Error en la base de datos: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el Tipo de Usuario: {ex.Message}");
            }
        }
    }
}
