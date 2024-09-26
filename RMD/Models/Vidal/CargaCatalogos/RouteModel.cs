namespace RMD.Models.Vidal.CargaCatalogos
{
    public class RouteModel
    {
        public int IdRoute { get; set; }          // ID de la ruta
        public string Name { get; set; }          // Nombre de la ruta (e.g., bucal)
        public bool Systemic { get; set; }        // Si es sistémica
        public bool Topical { get; set; }         // Si es tópica
        public int? ParentId { get; set; }        // ID de la ruta padre, si existe
        public DateTime VidalUpdateDate { get; set; } // Fecha de actualización de Vidal
    }
}
