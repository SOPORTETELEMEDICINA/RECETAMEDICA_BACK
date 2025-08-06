namespace RMD.Shared.Models.Vidal.ATC
{
    public class ATCClassification
    {
        public int IdATC { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime UpdatedDate { get; set; }
        public int? ParentId { get; set; }
        public int? ChildId { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public bool Estatus { get; set; }
    }
}
