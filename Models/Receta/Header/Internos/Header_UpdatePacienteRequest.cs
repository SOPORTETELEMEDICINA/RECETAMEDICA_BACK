using RMD.Models.Receta.Header.Base;

namespace RMD.Models.Receta.Header.Internos
{
    public class Header_UpdatePacienteRequest : Header_PacienteBase
    {
        public bool PacEmbarazo { get; set; }
        public int PacSemAmenorrea { get; set; }
        public bool PacLactancia { get; set; }
        public decimal? PacCreatinina { get; set; }
        public string Alergias { get; set; }
        public string Molecules { get; set; }
        public string Patologias { get; set; }

    }
}
