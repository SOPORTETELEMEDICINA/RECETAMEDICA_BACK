using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Vidal.Molecule
{
    public sealed class MoleculeByNameRequest
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        [MinLength(3, ErrorMessage = "Debe tener al menos 3 caracteres.")] // cámbialo a 4 si quieres
        [RegularExpression(@".*\S.*", ErrorMessage = "No puede estar en blanco.")]
        public string Name { get; set; } = string.Empty;
    }
}
