namespace RMD.Models.Consulta
{
    public class AsentamientoResultModel
    {
        public int IdAsentamiento { get; set; }
        public string NombreAsentamiento { get; set; } = string.Empty;
        public string TipoAsentamiento { get; set; } = string.Empty;
        public int IdCP { get; set; }
        public int CodigoPostal { get; set; }
        public int IdMunicipio { get; set; }
        public string NombreMunicipio { get; set; } = string.Empty;
        public int NoMunicipio { get; set; }
        public int IdCiudad { get; set; }
        public string NombreCiudad { get; set; } = string.Empty;
        public int IdEntidad { get; set; }
        public string NombreEntidad { get; set; } = string.Empty;
        public string Abreviatura { get; set; } = string.Empty;
    }

}
