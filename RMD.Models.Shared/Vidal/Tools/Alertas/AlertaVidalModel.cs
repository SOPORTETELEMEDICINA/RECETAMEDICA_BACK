namespace RMD.Shared.Models.Vidal.Tools.Alertas
{
    public class AlertaVidalModel
    {
        public required string Id { get; set; }
        public required string ProductId { get; set; }
        public required string InternalId { get; set; }
        public required string Titulo { get; set; }
        public required string Breve { get; set; }
        public required string Imagen { get; set; }
        public required string Autor { get; set; }
        public required string Enlace { get; set; }
        public required string Categoria { get; set; }
        public required string IdSubcategoria { get; set; }
        public required string Subcategoria { get; set; }
        public required string Publicacion { get; set; }
    }

}
