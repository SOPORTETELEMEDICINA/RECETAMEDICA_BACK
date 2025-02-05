using System;

namespace RMD.Models.Recetas
{
    public class Receta
    {
        public Guid IdReceta { get; set; }
        public Guid IdMedico { get; set; }
        public Guid IdPaciente { get; set; }
        public decimal PacPeso { get; set; }
        public decimal PacTalla { get; set; }
        public bool PacEmbarazo { get; set; }
        public int? PacSemAmenorrea { get; set; } // Opcional
        public bool PacLactancia { get; set; }
        public decimal? PacCreatinina { get; set; } // Opcional
        public string? Alergias { get; set; } // Puede ser null o vacío
        public string? Molecules { get; set; } // Puede ser null o vacío
        public string? Patologias { get; set; } // Puede ser null o vacío
        public Guid IdSucursal { get; set; }
        public Guid IdGEMP { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
    }
}
