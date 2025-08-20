namespace RMD.Shared.Models.Receta.AlertaToma.Request
{
    public class GetAlertasProgramadasEnFechaRequest
    {
        public Guid IdPaciente { get; set; }
        public DateTime Fecha { get; set; }
    }
}
