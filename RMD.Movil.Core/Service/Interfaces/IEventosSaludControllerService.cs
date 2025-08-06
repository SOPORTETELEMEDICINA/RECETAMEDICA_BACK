using RMD.Shared.Models.GlobalResponse;
using RMD.Shared.Models.Pacientes.Request;
using RMD.Shared.Models.Pacientes.Response;

namespace RMD.Movil.Core.Service.Interfaces
{
    public interface IEventosSaludControllerService
    {
        Task<ResponseFromService<IEnumerable<EventosSaludResponse>>> GetAllEventosPacienteAsync();
        Task<ResponseFromService<EventosSaludResponse>> GetEventoPacienteByIdAsync(Guid idEventoSalud);
        Task<ResponseFromService<bool>> CreateEventoPacienteAsync(EventosSaludRequest evento);
        Task<ResponseFromService<bool>> UpdateEventoPacienteAsync(EventosSaludRequest evento);
        Task<ResponseFromService<bool>> DeleteEventoPacienteAsync(Guid idEventoSalud);
    }
}
