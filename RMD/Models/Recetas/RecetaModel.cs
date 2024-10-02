namespace RMD.Models.Recetas
{
    public class RecetaModel
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
        public string Alergias { get; set; } // varchar(max)
        public string Molecules { get; set; } // varchar(max)
        public string Patologias { get; set; } // varchar(max)
        public Guid IdSucursal { get; set; }
        public Guid IdGEMP { get; set; }
    }

}
