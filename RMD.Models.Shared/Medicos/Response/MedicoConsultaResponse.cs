using RMD.Shared.Models.Medicos.Base;

namespace RMD.Shared.Models.Medicos.Response
{
    public class MedicoConsultaResponse : MedicoConsultaBase
    {
       
        public string Firma { get; set; } = string.Empty; // T11.Firma
        public string Imagen { get; set; } = string.Empty; // T11.Imagen
        public string Status { get; set; }
    }
}
