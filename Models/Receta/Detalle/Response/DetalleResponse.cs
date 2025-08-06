using RMD.Models.Receta.Detalle.Base;

namespace RMD.Models.Receta.Detalle.Response
{
    public class DetalleResponse : Detalle_Base
    {
        public string MedicamentoNombre { get; set; }
        public string UnidadDispensacion { get; set; }
        public string RutaAdministracion { get; set; }
        public string Descripcion { get; set; }
        public int GenerarEventoMedicamentoso { get; set; }
        public bool Surtido { get; set; }
    }
}