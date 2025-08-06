using RMD.Shared.Models.Catalogo;
using RMD.Shared.Models.GlobalResponse;

namespace RMD.Movil.Core.Service.Interfaces
{
    public interface ICatEventosSaludControllerService
    {
        Task<ResponseFromService<IEnumerable<CatEventosDeSalud>>> GetAllEventosSaludAsync();
    }
}
