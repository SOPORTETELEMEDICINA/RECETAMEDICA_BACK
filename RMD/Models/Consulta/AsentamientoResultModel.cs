namespace RMD.Models.Consulta
{
    public class AsentamientoResultModel
    {
        public int IdAsentamiento { get; set; }
        public string Asentamiento { get; set; } = string.Empty;
        public int IdTipoAsentamiento { get; set; }
        public string TipoAsentamiento { get; set; } = string.Empty;
        public int IdCP { get; set; }
        public string CodigoPostal { get; set; } = string.Empty;
        public int IdMunicipio { get; set; }
        public short NoMunicipio { get; set; }
        public string Municipio { get; set; } = string.Empty;
        public int IdCiudad { get; set; }
        public string Ciudad { get; set; } = string.Empty;
        public int IdEntidad { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Abreviatura { get; set; } = string.Empty;
    }
}
