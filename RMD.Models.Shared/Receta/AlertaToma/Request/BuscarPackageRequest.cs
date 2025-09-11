using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Receta.AlertaToma.Request
{
    public class BuscarPackageRequest
    {
        [Required(ErrorMessage = "NombrePackage es requerido.")]
        [RegularExpression(@"^(?=(?:.*\S){4,}).+$", ErrorMessage = "Debe contener al menos 4 caracteres no vacíos.")]
        public string NombrePackage { get; set; } = string.Empty;
    }

}
