using RMD.Models.Pacientes;

namespace RMD.Models.Usuarios
{
    public class CrearPacienteRequest
    {
        public required Usuario Usuario { get; set; }
        public required Paciente Paciente { get; set; }
    }

}
