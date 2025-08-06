namespace RMD.Shared.Models.Receta.Header.Base
{
    public class Header_Base
    {
        public Guid IdReceta { get; set; }
        public Guid IdMedico { get; set; }
        public Guid IdPaciente { get; set; }
        public decimal PacPeso { get; set; }
        public decimal PacTalla { get; set; }
        public bool PacEmbarazo { get; set; }
        public int? PacSemAmenorrea { get; set; }
        public bool PacLactancia { get; set; }
        public decimal? PacCreatinina { get; set; }
        public Guid IdSucursal { get; set; }
        public Guid IdGEMP { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
        public bool Timbrada { get; set; } = false;
        public string Folio { get; set; } = string.Empty;
    }
}
