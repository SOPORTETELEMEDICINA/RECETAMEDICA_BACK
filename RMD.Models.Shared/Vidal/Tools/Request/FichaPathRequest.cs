using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Vidal.Tools.Request
{
    public sealed class FichaPathRequest
    {
        [Required(ErrorMessage = "La ruta es requerida.")]
        [MinLength(3, ErrorMessage = "La ruta debe tener al menos 3 caracteres.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "La ruta no puede estar en blanco.")]
        public string Path { get; set; } = string.Empty;
    }
}
