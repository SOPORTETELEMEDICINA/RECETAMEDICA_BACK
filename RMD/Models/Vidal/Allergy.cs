namespace RMD.Models.Vidal
{
    public class Allergy : VidalBaseModel
    {
        // No necesita campos adicionales de Id, Name, o RelatedLinks porque los hereda del VidalBaseModel
        public int VidalId { get; set; }
        // Si el campo "Title" es redundante con el "Name" se puede eliminar
    }
}
