using RMD.Shared.Models.Pacientes.Request;
using RMD.Shared.Models.Pacientes.Response;

namespace RMD.Interface.Pacientes
{
    public interface IEventosSaludService
    {
        // Métodos para Eventos de Salud
        Task<ResponseFromService<IEnumerable<EventosSaludResponse>>> GetAllEventosPacienteAsync(Guid idPaciente);
        Task<ResponseFromService<EventosSaludResponse>> GetEventoPacienteByIdAsync(Guid idEventoSalud);
        Task<ResponseFromService<bool>> CreateEventoPacienteAsync(EventosSaludRequest evento);
        Task<ResponseFromService<bool>> UpdateEventoPacienteAsync(EventosSaludRequest evento);
        Task<ResponseFromService<bool>> DeleteEventoPacienteAsync(Guid idEventoSalud);
    }
}
