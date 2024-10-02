namespace RMD.Models.Vidal.CargaCatalogos
{
    public class ATCClassificationModel
    {
        public int IdATC { get; set; } // vidal:id
        public string Name { get; set; } // vidal:name
        public string Code { get; set; } // vidal:code
        public DateTime UpdatedDate { get; set; } // updated date
        public int? ParentId { get; set; } // href de "PARENT"
        public int? ChildId { get; set; } // href de "CHILDREN"
    }
}
