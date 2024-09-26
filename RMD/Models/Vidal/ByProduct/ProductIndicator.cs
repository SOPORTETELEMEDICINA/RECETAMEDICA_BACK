namespace RMD.Models.Vidal.ByProduct
{
    public class ProductIndicator
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public List<Indicator> Indicators { get; set; } = new List<Indicator>();
    }
}
