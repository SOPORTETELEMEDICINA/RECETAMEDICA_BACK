namespace RMD.Models.CargaCatalogosAPI_DATA.Allergy
{
    public class AllergyApiModel
    {
        public int IdAllergy { get; set; }          // ID de la alergia
        public string Name { get; set; }            // Nombre de la alergia
        public string MoleculesLink { get; set; } = string.Empty;   // Enlace a las moléculas relacionadas
        public DateTime VidalUpdateDate { get; set; } // Fecha de actualización de Vidal
      
    }
}
