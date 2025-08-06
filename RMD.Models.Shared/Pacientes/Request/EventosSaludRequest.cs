namespace RMD.Shared.Models.Pacientes.Request
{
    public class EventosSaludRequest
    {
        public Guid IdEventoSalud { get; set; }
        public Guid IdPaciente { get; set; }
        public DateTime Fecha { get; set; }
        public int EventoDeSalud { get; set; }
        public string Descripcion { get; set; }
    }
}
