using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Vidal.Allergy
{
    public sealed class AllergyByNameRequest
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        [MinLength(3, ErrorMessage = "El nombre debe tener al menos 3 caracteres.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "El nombre no puede estar en blanco.")]
        public string Name { get; set; } = string.Empty;
    }
}
