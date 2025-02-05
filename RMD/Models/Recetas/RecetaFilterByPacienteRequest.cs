namespace RMD.Models.Recetas
{
    public class RecetaFilterByPacienteRequest
    {
        public Guid IdPaciente { get; set; }     // Solo para Super Admin
        public DateTime? StartDate { get; set; } // Inicio del rango de fecha
        public DateTime? EndDate { get; set; }   // Fin del rango de fecha
        public string? DateFilter { get; set; }  // 'Dia', 'Semana', 'Mes', 'Año', o null para rango específico
    }
}
