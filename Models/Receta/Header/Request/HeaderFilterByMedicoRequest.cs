namespace RMD.Models.Receta.Header.Request
{
    public class HeaderFilterByMedicoRequest
    {
        //public Guid? IdSucursal { get; set; }
        public string Folio { get; set; }       // Folio de la receta
        public DateTime? StartDate { get; set; } // Inicio del rango de fecha
        public DateTime? EndDate { get; set; }   // Fin del rango de fecha
        public string? DateFilter { get; set; }  // 'Dia', 'Semana', 'Mes', 'Año', o null para rango específico
    }
}
