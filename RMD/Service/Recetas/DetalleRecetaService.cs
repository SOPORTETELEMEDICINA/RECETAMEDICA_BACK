using Microsoft.EntityFrameworkCore;
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
    }
}
