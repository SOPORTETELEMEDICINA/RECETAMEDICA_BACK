namespace RMD.Models.Vidal.ByProduct
{
    public class AtcEntry
    {
        public int AtcId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int VmpsId { get; set; }
        public int ProductsId { get; set; }
        public int ChildrenId { get; set; }
        public int MoleculesId { get; set; }
        public int ParentId { get; set; }
    }
}
