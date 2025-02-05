namespace RMD.Models.Recetas
{
    public class RecetaFilterRequest
    {
        public Guid? IdSucursal { get; set; } // Obligatorio para roles que no son farmacia
        public Guid? IdGEMP { get; set; }     // Solo para Super Admin
        public DateTime? StartDate { get; set; } // Inicio del rango de fecha
        public DateTime? EndDate { get; set; }   // Fin del rango de fecha
        public string? DateFilter { get; set; }  // 'Dia', 'Semana', 'Mes', 'Año', o null para rango específico
    }
}
