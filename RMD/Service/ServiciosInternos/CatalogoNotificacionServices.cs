using RMD.Data;
using RMD.Interface.Notificaciones;
using RMD.Models.Responses;

namespace RMD.Service.ServiciosInternos
{
    public class CatalogoNotificacionServices : ICatalogoNotificacionService
    {
        private readonly ApplicationDbContext _context;
        public CatalogoNotificacionServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CatalogoNotificacion?> GetNotificationByCodeAsync(int codigoNotificacion)
        {
            return await _context.CatalogoNotificaciones
                .FirstOrDefaultAsync(n => n.CodigoNotificacion == codigoNotificacion);
        }

        public async Task<CatalogoNotificacion?> GetNotificationByTipoAndFuncionAsync(string tipo, string funcion)
        {
            return await _context.CatalogoNotificaciones
              .FirstOrDefaultAsync(n => n.Tipo == tipo && n.Funcion == funcion);
        }
    }
    
   
}
