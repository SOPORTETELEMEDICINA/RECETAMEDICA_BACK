namespace RMD.Models.Vidal.CargaCatalogos
{
    public class SideEffectModel
    {
        public int IdSideEffect { get; set; } // Corresponde a <vidal:id>
        public string Name { get; set; } // Corresponde a <vidal:name>
        public string ApparatusName { get; set; } // Corresponde a <vidal:apparatus>
        public int? ApparatusId { get; set; } // Corresponde a <vidal:apparatus>.Attribute("vidalId")
        public DateTime Updated { get; set; } // Corresponde a <updated>
    }
}
