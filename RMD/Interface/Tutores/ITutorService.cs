using RMD.Shared.Models.Tutores.Request;

namespace RMD.Interface.Tutores
{
    public interface ITutorService
    {
        Task<ResponseFromService<string>> AsignarTutorAsync(TutorRequest model, Guid idUsuario);
        Task<ResponseFromService<string>> ActualizarTutorAsync(Guid idTutor, TutorRequest model, Guid idUsuario);
        Task<ResponseFromService<string>> EliminarTutorAsync(Guid idTutor);
        Task<ResponseFromService<object>> GetTutorPorPacienteAsync(Guid idPaciente); // Cambia `object` por tu modelo de respuesta si ya lo tenés
    }
}