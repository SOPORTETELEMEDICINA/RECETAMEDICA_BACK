using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Vidal.Producto
{
    public sealed class ProductIdRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "El idProduct debe ser mayor a 0.")]
        public int IdProduct { get; set; }
    }
}
