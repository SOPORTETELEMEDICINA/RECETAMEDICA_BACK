namespace RMD.Models.Recetas
{
    public class MoleculeModel
    {
        public int IdMolecule { get; set; }          // ID de la molécula
        public string Name { get; set; }             // Nombre de la molécula
        public bool SafetyAlert { get; set; }        // Alerta de seguridad
        public bool Homeopathy { get; set; }         // Si es homeopática
        public string Role { get; set; }             // Rol (Principio Activo, Excipiente, etc.)
        public DateTime VidalUpdateDate { get; set; } // Fecha de actualización de Vidal
    }
}
