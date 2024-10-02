using System;
using System.Collections.Generic;

namespace RMD.Models.Recetas
{
    public class RecetaWithDetalleModel
    {
        // Propiedades de la tabla Receta
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

        // Propiedades adicionales para los detalles de receta
        public List<RecetaDetalleModel> DetallesReceta { get; set; } // Lista de detalles de la receta
    }
}
