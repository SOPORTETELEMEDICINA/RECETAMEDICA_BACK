namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.Unit
{
    public class UnitApiModel
    {
        public int IdUnit { get; set; }               // ID de la unidad
        public string Name { get; set; }              // Nombre de la unidad
        public string SingularName { get; set; }      // Nombre singular de la unidad
        public string Conversion { get; set; }        // Cadena de conversión
        public int Denominator { get; set; }          // Denominador para la conversión
        public decimal Numerator { get; set; }        // Numerador para la conversión
        public int? ParentUnitId { get; set; }        // ID de la unidad padre
        public DateTime VidalUpdateDate { get; set; } // Fecha de actualización de Vidal
    }
}
