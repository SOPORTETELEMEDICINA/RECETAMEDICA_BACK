namespace RMD.Shared.Models.Vidal.CIM10
{
    public class PatologiaRequest
    {
        public string Nombre { get; set; }  // Ej: "Diabetes Mellitus tipo 2"
        public float Edad { get; set; } = 0;     // Edad del paciente al momento del diagnóstico
        public string Genero { get; set; } = "N/A"; // "Masculino", "Femenino", etc.
    }

}
