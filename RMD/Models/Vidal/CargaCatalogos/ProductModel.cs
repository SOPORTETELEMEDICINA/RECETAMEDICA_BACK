namespace RMD.Models.Vidal.CargaCatalogos
{
    public class ProductModel
    {
        public int IdProduct { get; set; }               // ID del producto
        public string Summary { get; set; }              // Resumen del producto
        public string Name { get; set; }                 // Nombre del producto
        public string IdItemType { get; set; }           // Tipo de ítem
        public string IdMarketStatus { get; set; }       // Estado del mercado
        public bool HasPublishedDoc { get; set; }        // Si tiene documentos publicados
        public bool WithoutPrescription { get; set; }    // Si no requiere receta
        public int IdAmmType { get; set; }               // Tipo de autorización
        public string BestDocType { get; set; }          // Mejor tipo de documento
        public bool SafetyAlert { get; set; }            // Alerta de seguridad
        public int IdCompany { get; set; }               // ID de la compañía
        public string CompanyName { get; set; }          // Nombre de la compañía
        public string TypeCompany { get; set; }          // Tipo de compañía
        public int IdVmp { get; set; }                   // ID del VMP
        public int IdGalenicForm { get; set; }           // ID de la forma galénica
        public string GalenicForm { get; set; }          // Descripción de la forma galénica
        public DateTime VidalUpdateDate { get; set; }    // Fecha de actualización de Vidal
    }
}
