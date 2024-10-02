namespace RMD.Models.Recetas
{
    public class AllergyModel
    {
        public int IdAllergy { get; set; }          // ID de la alergia
        public string Name { get; set; }            // Nombre de la alergia
        public DateTime VidalUpdateDate { get; set; } // Fecha de actualización de Vidal
    }
}
