namespace RMD.Models.Catalogo
{
    public class DomicilioRequest
    {
        public int IdPais { get; set; }
        public string NombrePais { get; set; }
        public string AbreviaturaPais { get; set; }

        public int IdEntidad { get; set; }
        public string NombreEntidad { get; set; }
        public string AbreviaturaEntidad { get; set; }

        public int IdMunicipio { get; set; }
        public string NombreMunicipio { get; set; }

        public int IdCiudad { get; set; }
        public string NombreCiudad { get; set; }

        public int IdCP { get; set; }
        public string CodigoPostal { get; set; }

        public int IdTipoAsentamiento { get; set; }
        public string TipoAsentamiento { get; set; }

        public string NombreAsentamiento { get; set; }
    }
}
