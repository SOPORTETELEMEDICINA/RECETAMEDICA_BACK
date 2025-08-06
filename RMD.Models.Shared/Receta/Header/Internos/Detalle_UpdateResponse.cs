using RMD.Shared.Models.Receta.Detalle.Base;

namespace RMD.Shared.Models.Receta.Header.Internos
{
    public class Detalle_UpdateResponse : Detalle_Base
    {
        public string MedicamentoNombre { get; set; }
        public string UnidadDispensacion { get; set; }
        public string RutaAdministracion { get; set; }
        public bool Surtido { get; set; }
        public string Descripcion { get; set; }
    }
}