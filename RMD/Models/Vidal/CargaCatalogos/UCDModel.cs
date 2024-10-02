namespace RMD.Models.Vidal.CargaCatalogos
{
    public class UCDModel
    {
        public int IdUCD { get; set; } // Corresponde a <vidal:id>
        public string Summary { get; set; } // Corresponde a <summary>
        public string Name { get; set; } // Corresponde a <vidal:name>
        public string MarketStatusName { get; set; } // Corresponde a <vidal:marketStatus>.Attribute("name")
        public string MarketStatus { get; set; } // Corresponde al valor del elemento <vidal:marketStatus>
        public bool SafetyAlert { get; set; } // Corresponde a <vidal:safetyAlert>
        public DateTime Updated { get; set; } // Corresponde a <updated> del XML
        public string VmpDescription { get; set; } // Corresponde a <vidal:vmp> (descripción)
        public int? VmpId { get; set; } // Corresponde a <vidal:vmp>.Attribute("vidalId")
    }
}
