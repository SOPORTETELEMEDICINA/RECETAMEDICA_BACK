using RMD.Shared.Models.Pacientes.Request;

namespace RMD.Shared.Models.Usuarios
{
    public class CrearPacienteRequest
    {
        public required Usuario Usuario { get; set; }
        public required PacienteRequest Paciente { get; set; }
    }

}
