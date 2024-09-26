namespace RMD.Models.Vidal.CargaCatalogos
{
    public class PackageModel
    {
        public int IdPackage { get; set; }                   // ID del paquete
        public string Name { get; set; }                     // Nombre del paquete
        public string Summary { get; set; }                  // Resumen del paquete
        public int ProductId { get; set; }                   // ID del producto relacionado
        public string MarketStatus { get; set; }             // Estado del mercado
        public bool Otc { get; set; }                        // Over-the-counter (indicador de venta libre)
        public bool IsCeps { get; set; }                     // Indicador de CEPS
        public int DrugId { get; set; }                      // ID del fármaco
        public string Cip13 { get; set; }                    // Código CIP 13
        public string ShortLabel { get; set; }               // Etiqueta corta
        public bool Tfr { get; set; }                        // Tasa de reembolso o relacionado
        public int IdCompany { get; set; }                   // ID de la compañía
        public string CompanyName { get; set; }              // Nombre de la compañía
        public bool NarcoticPrescription { get; set; }       // Indicador de receta narcótica
        public bool SafetyAlert { get; set; }                // Alerta de seguridad
        public bool WithoutPrescription { get; set; }        // Indicador de sin receta
        public int IdGalenicForm { get; set; }               // ID de la forma galénica
        public string GalenicForm { get; set; }              // Descripción de la forma galénica
        public string UcdCode13 { get; set; }                // Código UCD 13
        public string UcdCode7 { get; set; }                 // Código UCD 7
        public int UcdId { get; set; }                       // ID de UCD
        public DateTime VidalUpdateDate { get; set; }        // Fecha de actualización de Vidal
    }
}
