namespace RMD.Models.Consulta
{
    public class Medico
    {
        public Guid IdMedico { get; set; }
        public Guid IdUsuario { get; set; }
        public string CedulaGeneral { get; set; } = string.Empty;
        public string Universidad { get; set; } = string.Empty;
        public string? Especialidad { get; set; }
        public string? CedulaEspecialidad { get; set; }
        public string Horario { get; set; } = string.Empty;
    }
}
