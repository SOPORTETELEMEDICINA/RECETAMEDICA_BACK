namespace RMD.Shared.Models.Receta.Header.Request
{
    public class HeaderFilterByPacienteRequest
    {
        public Guid? IdPaciente { get; set; }     // Solo para Super Admin
        public DateTime? StartDate { get; set; } // Inicio del rango de fecha
        public DateTime? EndDate { get; set; }   // Fin del rango de fecha
        //public string? Folio { get; set; }       // Folio de la receta
        public string? DateFilter { get; set; }  // 'Dia', 'Semana', 'Mes', 'Año', o null para rango específico
    }
}
