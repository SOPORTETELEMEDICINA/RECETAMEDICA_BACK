using RMD.Models.Pacientes;
using RMD.Models.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RMD.Interface.Pacientes
{
    public interface IPacienteService
    {
        Task<ResponseFromService<Guid?>> GetIdPacienteByUsuarioAsync(Guid idUsuario);

        Task<ResponseFromService<IEnumerable<PacienteConsultaRequest>>> GetPacienteByNameAsync(string nombreBusqueda, Guid idGEMP);
        Task<ResponseFromService<PacienteConsultaRequest>> GetPacienteByIdUsuarioAsync(Guid idUsuario);
        Task<ResponseFromService<PacienteConsultaRequest>> GetPacienteByIdPacienteAsync(Guid idPaciente);
        Task<ResponseFromService<bool>> EliminarPacienteAsync(Guid idPaciente, Guid idUsuarioSolicitante);
        Task<ResponseFromService<bool>> CreatePacienteAsync(PacienteCreateConListas pacienteRequest, Guid idUsuarioSolicitante);
        Task<ResponseFromService<bool>> UpdatePacienteAsync(PacienteConListas paciente, Guid idUsuarioSolicitante);
        Task<ResponseFromService<IEnumerable<PacienteConsultaRequest>>> GetPacientesByMedicoAsync(Guid idMedico);
        Task<ResponseFromService<IEnumerable<PacienteConsultaRequest>>> GetPacientesByGEMPAsync(Guid idGEMP);
        Task<ResponseFromService<IEnumerable<PacienteConsultaRequest>>> GetPacientesBySucursalAsync(Guid idSucursal);

        Task<ResponseFromService<IEnumerable<EntidadNacimiento>>> GetEntidadesFederativasAsync();
        //Task<ResponseFromService<IEnumerable<Paciente>>> GetPacientesByEntidadNacimientoAsync(int idEntidadNacimiento);

        // Métodos para Eventos de Salud
        Task<ResponseFromService<IEnumerable<EventosSaludConsulta>>> GetAllEventosPacienteAsync(Guid idPaciente);
        Task<ResponseFromService<EventosSaludConsulta>> GetEventoPacienteByIdAsync(Guid idEventoSalud);
        Task<ResponseFromService<EventosSalud>> CreateEventoPacienteAsync(EventosSalud evento);
        Task<ResponseFromService<bool>> UpdateEventoPacienteAsync(EventosSalud evento);
        Task<ResponseFromService<bool>> DeleteEventoPacienteAsync(Guid idEventoSalud);
    }
}
