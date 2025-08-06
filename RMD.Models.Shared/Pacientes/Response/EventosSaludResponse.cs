namespace RMD.Shared.Models.Pacientes.Response
{
    public class EventosSaludResponse
    {
        public Guid IdEventoSalud { get; set; }
        public Guid IdPaciente { get; set; }
        public DateTime Fecha { get; set; }
        public int EventoDeSalud { get; set; }
        public string Descripcion { get; set; }
        public string NombreEvento { get; set; } // Se incluye solo en las consultas
    }
}
