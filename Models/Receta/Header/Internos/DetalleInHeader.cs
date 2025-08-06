using RMD.Models.Receta.Detalle.Base;

namespace RMD.Models.Receta.Header.Internos
{
    public class DetalleInHeader : Detalle_Base
    {
        public bool? IsNarcotic { get; set; }
        public string? PsicoAnnexId { get; set; }
    }
}