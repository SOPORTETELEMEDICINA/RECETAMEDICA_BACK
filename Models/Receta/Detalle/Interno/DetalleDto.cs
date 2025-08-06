using RMD.Models.Receta.Detalle.Base;

namespace RMD.Models.Receta.Detalle.Interno
{
    public class DetalleDto : Detalle_Base
    {
        public int IdProduct { get; set; }
        public string Summary { get; set; }
        public string Name { get; set; }
        public int IdVmp { get; set; }
        public string GalenicForm { get; set; }
        public string GalenicName { get; set; }
        public string AtcCode { get; set; }
        public string AtcCodeMJ { get; set; }
        public int? Pictogram { get; set; }
        public bool? IsNarcotic { get; set; }
        public string PsicoAnnexId { get; set; }
        public string Monodrug { get; set; }
        public bool Surtido { get; set; }
        public DateTime? FechaSurtido { get; set; }
        public int DuracionEnDias { get; set; }
        public decimal? DosisPorDia { get; set; }
        public decimal? TotalDosis { get; set; }
        public decimal? TotalLitros { get; set; }
        public string Unidad { get; set; }
        public string Conversion { get; set; }
        public decimal? Denominator { get; set; }
        public decimal? Numerator { get; set; }
    }
}