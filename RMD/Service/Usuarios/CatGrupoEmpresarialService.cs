using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Extensions; // Asegúrate de tener las extensiones necesarias para ToDataTable()
using RMD.Interface.Usuarios;
using RMD.Models.Usuarios;
using System.Data;

namespace RMD.Service.Usuarios
{
    public class CatGrupoEmpresarialService : ICatGrupoEmpresarialService
    {
        private readonly UsuariosDBContext _context;

        public CatGrupoEmpresarialService(UsuariosDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CatGrupoEmpresarial>> GetAllGrupoEmpresarial()
        {
            return await _context.CatGrupoEmpresariales
                .FromSqlRaw("EXEC Usuarios_GetAllGrupoEmpresarial")
                .ToListAsync();
        }

        public async Task<CatGrupoEmpresarial> GetGrupoEmpresarialById(Guid id)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@IdGEMP", id)
            };

            var results = await _context.CatGrupoEmpresariales
                .FromSqlRaw("EXEC Usuarios_GetGrupoEmpresarialById @IdGEMP", parameters)
                .ToListAsync();

            if (results.Count > 0)
            {
                return results[0];
            }
            else
            {
                throw new KeyNotFoundException("No existe el Grupo Empresarial especificado.");
            }
        }

        public async Task<string> CreateGrupoEmpresarial(CatGrupoEmpresarial grupoEmpresarial)
        {
            var parameter = new SqlParameter("@GrupoEmpresarialData", SqlDbType.Structured)
            {
                TypeName = "dbo.GrupoEmpresarialType",
                Value = new List<CatGrupoEmpresarial> { grupoEmpresarial }.ToDataTable()
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Usuarios_CreateGrupoEmpresarial @GrupoEmpresarialData", parameter);

            return "Operación realizada con éxito.";
        }

        public async Task<string> UpdateGrupoEmpresarial(CatGrupoEmpresarial grupoEmpresarial)
        {
            var parameter = new SqlParameter("@GrupoEmpresarialData", SqlDbType.Structured)
            {
                TypeName = "dbo.GrupoEmpresarialType",
                Value = new List<CatGrupoEmpresarial> { grupoEmpresarial }.ToDataTable()
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC Usuarios_UpdateGrupoEmpresarial @GrupoEmpresarialData", parameter);

            return "Operación realizada con éxito.";
        }
    }
}
