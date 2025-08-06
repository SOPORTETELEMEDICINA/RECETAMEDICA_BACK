namespace RMD.Shared.Models.Receta.Detalle.Response
{
    public class MedicamentoActivoResponse
    {
        public Guid IdReceta { get; set; }
        public Guid IdPaciente { get; set; }
        public Guid IdDetalleReceta { get; set; }
        public string DrugType { get; set; }
        public int Drug { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Medicamento { get; set; }

    }

}
