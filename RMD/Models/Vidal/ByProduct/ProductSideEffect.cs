using RMD.Models.Vidal.BySideEffect;

namespace RMD.Models.Vidal.ByProduct
{
    public class ProductSideEffect
    {
        public string Title { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public List<SideEffect> SideEffects { get; set; } = new List<SideEffect>();
    }
}
