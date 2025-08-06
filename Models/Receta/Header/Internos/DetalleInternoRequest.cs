using RMD.Models.Receta.Detalle.Base;

namespace RMD.Models.Receta.Header.Internos
{
    public class DetalleInternoRequest : Detalle_Base
    {
        public int Cantidad { get; set; } // Cantidad de medicamento
        public string MedicamentoNombre { get; set; }
        public string UnidadDispensacion { get; set; }
        public string RutaAdministracion { get; set; }
        public bool Surtido { get; set; }
    }
}