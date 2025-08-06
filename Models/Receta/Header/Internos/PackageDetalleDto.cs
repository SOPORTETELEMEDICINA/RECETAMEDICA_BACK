namespace RMD.Models.Receta.Header.Internos
{
    public class PackageDetalleDto
    {
        public int IdPackage { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public int ProductId { get; set; }
        public int DrugId { get; set; }
        public string Cip13 { get; set; }
        public string ShortLabel { get; set; }
        public int IdGalenicForm { get; set; }
        public string GalenicForm { get; set; }
        public int UcdId { get; set; }
        public string Contents { get; set; }
        public int Contents_Unit_Id { get; set; }
        public int UcdvId { get; set; }
        public int ContainerId { get; set; }
        public decimal ContainerQuantityValue { get; set; }
        public int ContainerQuantityUnitId { get; set; }
        public decimal ContentQuantityValue { get; set; }
        public int ContentQuantityUnitId { get; set; }
        public string Unidad { get; set; }
        public string Conversion { get; set; }
        public decimal? Denominator { get; set; }
        public decimal? Numerator { get; set; }
        public int? ParentUnitId { get; set; }
        public decimal? TotalContenidoL { get; set; }
    }

}
