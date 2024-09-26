namespace RMD.Models.Vidal.ByVMP
{
    public class VMPUcdvEntry
    {
        public int IdUcdv { get; set; }
        public string Name { get; set; } = string.Empty;
        public int IdConditioningUnit { get; set; }
        public string ConditioningUnitName { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public int IdQuantityUnit { get; set; }
        public string QuantityUnitName { get; set; } = string.Empty;
        public int IdGalenicForm { get; set; }
        public string GalenicFormName { get; set; } = string.Empty;
    }
}
