namespace RMD.Models.Medicos
{
    public class MedicoCreate
    {
        public Guid IdUsuario { get; set; }

        public string CedulaGeneral { get; set; } = string.Empty;

        public string Universidad { get; set; } = string.Empty;

        public string Especialidad { get; set; } = string.Empty;

        public string CedulaEspecialidad { get; set; } = string.Empty;

        public string Horario { get; set; } = string.Empty;
    }
}
