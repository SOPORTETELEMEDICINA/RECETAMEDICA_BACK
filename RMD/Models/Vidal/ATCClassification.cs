namespace RMD.Models.Vidal
{
    public class ATCClassification : VidalBaseModel
    {
        public string Code { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }
}
