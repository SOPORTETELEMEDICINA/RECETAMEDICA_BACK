using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Vidal.Tools.Request
{
    public sealed class TipoAlertaRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "El tipo debe ser un entero positivo.")]
        public int Type { get; set; }
    }
}
