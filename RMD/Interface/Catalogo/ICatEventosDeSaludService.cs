using RMD.Shared.Models.Catalogo;

namespace RMD.Interface.Catalogo
{
    public interface ICatEventosDeSaludService
    {
        // CRUD para Eventos de Salud
        Task<ResponseFromService<IEnumerable<CatEventosDeSalud>>> GetAllEventosSaludAsync();
        Task<ResponseFromService<CatEventosDeSalud>> GetEventoSaludByIdAsync(int id);
        Task<ResponseFromService<CatEventosDeSalud>> CreateEventoSaludAsync(CatEventosDeSalud evento);
        Task<ResponseFromService<string>> UpdateEventoSaludAsync(int id, CatEventosDeSalud evento);
        Task<ResponseFromService<string>> DeleteEventoSaludAsync(int id);
    }
}
