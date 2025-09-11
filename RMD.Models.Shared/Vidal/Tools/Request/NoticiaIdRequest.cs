using System.ComponentModel.DataAnnotations;

namespace RMD.Shared.Models.Vidal.Tools.Request
{

    public sealed class NoticiaIdRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "El id de noticia debe ser mayor que 0.")]
        public int IdNoticia { get; set; }
    }
}
