namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.CIM10
{
    public class CIM10ApiModel
    {
        public int IdCIM10 { get; set; }
        public string Code { get; set; }
        public DateTime UpdatedDate { get; set; }
        // Nuevos campos para los links
        public string ALDSLink { get; set; } = string.Empty;   // Link relacionado a ALDS
        public string ChildrenLink { get; set; } = string.Empty;  // Link relacionado a CHILDREN
    }

}
