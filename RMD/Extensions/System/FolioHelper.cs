using Microsoft.EntityFrameworkCore;
using RMD.Data;

namespace RMD.Extensions.System
{
    public sealed class FolioHelper
    {
        private readonly SucursalesDbContext _sucursales;
        private readonly UsuariosDBContext _usuarios;
        private readonly RecetasDbContext _recetas;

        public FolioHelper(
            SucursalesDbContext sucursales,
            UsuariosDBContext usuarios,
            RecetasDbContext recetas)
        {
            _sucursales = sucursales;
            _usuarios = usuarios;
            _recetas = recetas;
        }

        public async Task<string> GenerarFolioAsync(Guid idGEMP, DateTime fechaCreacion, Guid idSucursal)
        {
            var abreviatura = await _usuarios.CatGrupoEmpresariales
                .Where(x => x.IdGEMP == idGEMP)
                .Select(x => x.Abreviatura)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(abreviatura))
                throw new Exception("No se encontró la abreviatura del grupo empresarial.");

            var numeroSucursal = await _sucursales.Sucursales
                .Where(x => x.IdSucursal == idSucursal)
                .Select(x => x.Numero)
                .FirstOrDefaultAsync();

            var fechaFormato = fechaCreacion.ToString("yyMMdd");
            var sucursalFormato = numeroSucursal.ToString("D3");
            var prefijo = $"{abreviatura}-{fechaFormato}-{sucursalFormato}";

            var consecutivo = await _recetas.ConsultaRecetas
                .CountAsync(r => r.Folio.StartsWith(prefijo)) + 1;

            return $"{prefijo}-{consecutivo:D3}";
        }
    }
}
