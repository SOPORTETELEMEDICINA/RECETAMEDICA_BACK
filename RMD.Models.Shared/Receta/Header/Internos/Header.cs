using RMD.Shared.Models.Receta.Header.Base;

namespace RMD.Shared.Models.Receta.Header.Internos
{
    public class Header : Header_Base
    {
        public string Alergias { get; set; } = string.Empty;
        public string Molecules { get; set; } = string.Empty;
        public string Patologias { get; set; } = string.Empty;
    }
}
