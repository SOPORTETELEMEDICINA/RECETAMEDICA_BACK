using RMD.Shared.Models.Receta.Detalle.Base;

namespace RMD.Shared.Models.Receta.Header.Internos
{
    public class DetalleRecetaGet : Detalle_Base
    {
        public string Medicamento { get; set; }
        public string UnidadDispensacion { get; set; }
        public string RutaAdministracion { get; set; }
        public bool Surtido { get; set; }
    }
}