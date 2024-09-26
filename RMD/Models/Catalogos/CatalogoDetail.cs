namespace RMD.Models.Catalogos
{
    /// <summary>
    /// Detalle del catálogo con listas de entidades relacionadas.
    /// </summary>
    public class CatalogoDetail
    {
        public List<CP> CPs { get; set; } = new List<CP>();
        public List<Asentamiento> Asentamientos { get; set; } = [];
        public List<Municipio> Municipios { get; set; } = [];
        public List<Ciudad> Ciudades { get; set; } = [];
        public List<Entidad> Entidades { get; set; } = [];
    }
}
