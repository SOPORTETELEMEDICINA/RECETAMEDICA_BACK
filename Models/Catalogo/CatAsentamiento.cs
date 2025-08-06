namespace RMD.Models.Catalogo
{
    public class CatAsentamiento
    {
        public int IdAsentamiento { get; set; }
        public string Nombre { get; set; }
        public int IdTipoAsentamiento { get; set; }
        public int IdCP { get; set; }
        public int IdMunicipio { get; set; }
        public int IdEntidad { get; set; }
    }
}
