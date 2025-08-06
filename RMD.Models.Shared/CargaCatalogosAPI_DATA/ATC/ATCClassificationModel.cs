namespace RMD.Shared.Models.CargaCatalogosAPI_DATA.ATC
{
    public class ATCClassificationModel
    {
        public int IdATC { get; set; } // vidal:id
        public required string Name { get; set; } // vidal:name
        public required string Code { get; set; } // vidal:code
        public DateTime UpdatedDate { get; set; } // updated date
        public int? ParentId { get; set; } // href de "PARENT"
        public int? ChildId { get; set; } // href de "CHILDREN"
    }
}
