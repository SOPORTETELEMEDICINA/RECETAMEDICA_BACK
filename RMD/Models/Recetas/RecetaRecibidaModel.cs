namespace RMD.Models.Recetas
{    
    public class RecetaRecibidaModel
    {
        public Guid IdReceta { get; set; } = Guid.Empty; // Si no se proporciona, se genera en el controlador
        public Guid IdMedico { get; set; }
        public Guid IdPaciente { get; set; }
        public decimal PacPeso { get; set; }
        public decimal PacTalla { get; set; }
        public bool PacEmbarazo { get; set; }
        public int? PacSemAmenorrea { get; set; }
        public bool PacLactancia { get; set; }
        public decimal? PacCreatinina { get; set; }
        public List<AllergyModel> Alergias { get; set; } // varchar(max)
        public List<MoleculeModel> Molecules { get; set; } // varchar(max)
        public List<CIM10Model> Patologias { get; set; } // varchar(max)
        public Guid IdSucursal { get; set; }
        public Guid IdGEMP { get; set; }

        // Lista de detalles de la receta
        public List<RecetaDetalleRecibidaModel> DetallesReceta { get; set; } = new List<RecetaDetalleRecibidaModel>();
    }
}
