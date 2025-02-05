//namespace RMD.Models.Vidal.CargaCatalogos
//{
//    public class PackageModel
//    {
//        public int IdPackage { get; set; }                   // ID del paquete
//        public string Name { get; set; }                     // Nombre del paquete
//        public string Summary { get; set; }                  // Resumen del paquete
//        public int ProductId { get; set; }                   // ID del producto relacionado
//        public string MarketStatus { get; set; }             // Estado del mercado
//        public bool Otc { get; set; }                        // Over-the-counter (indicador de venta libre)
//        public bool IsCeps { get; set; }                     // Indicador de CEPS
//        public int DrugId { get; set; }                      // ID del fármaco
//        public string Cip13 { get; set; }                    // Código CIP 13
//        public string ShortLabel { get; set; }               // Etiqueta corta
//        public bool Tfr { get; set; }                        // Tasa de reembolso o relacionado
//        public int IdCompany { get; set; }                   // ID de la compañía
//        public string CompanyName { get; set; }              // Nombre de la compañía
//        public bool NarcoticPrescription { get; set; }       // Indicador de receta narcótica
//        public bool SafetyAlert { get; set; }                // Alerta de seguridad
//        public bool WithoutPrescription { get; set; }        // Indicador de sin receta
//        public int IdGalenicForm { get; set; }               // ID de la forma galénica
//        public string GalenicForm { get; set; }              // Descripción de la forma galénica
//        public string UcdCode13 { get; set; }                // Código UCD 13
//        public string UcdCode7 { get; set; }                 // Código UCD 7
        
//                                                             // Nuevas propiedades para los enlaces
//        public string LargerPacks { get; set; } = string.Empty;             // Link a paquetes más grandes
//        public string AffiliationCenter { get; set; } = string.Empty;          // Link a centros de afiliación
//        public string Pds { get; set; } = string.Empty;                       // Link a PDS
//        public string PricingSchedule { get; set; } = string.Empty;            // Link a la programación de precios
//        public string Units { get; set; } = string.Empty;                     // Link a unidades
//        public string Routes { get; set; } = string.Empty;                    // Link a rutas
//        public string Indicators { get; set; } = string.Empty;                // Link a indicadores
//        public string Indications { get; set; } = string.Empty;                // Link a indicaciones
//        public string SideEffects { get; set; } = string.Empty;               // Link a efectos secundarios
//        public string Alds { get; set; } = string.Empty;                // Link a ALDS
//        public string VatExcAffiliationCenter { get; set; } = string.Empty;   // Link a excepción de tasa de IVA
//        public string RefundIndications { get; set; } = string.Empty;       // Link a indicaciones de reembolso
//        public string OptDocument { get; set; } = string.Empty;            // Link a documento OPT
//        public string Document { get; set; } = string.Empty;              // Link a documentos
//        public string Ucd { get; set; } = string.Empty;                 // Link a UCD
//        public int UcdId { get; set; }                       // ID de UCD
//        public DateTime VidalUpdateDate { get; set; }        // Fecha de actualización de Vidal

//    }
//}
