namespace RMD.Models.Catalogo
{
    public class CatAsentamientos
    {
        public int IdAsentamiento { get; set; }
        public string Nombre { get; set; }
        public int IdTipoAsentamiento { get; set; }
        public int IdCP { get; set; }
        public int IdMunicipio { get; set; }
        public int IdCiudad { get; set; } // Incluido para el GET
        public int IdEntidad { get; set; }

        // Campos adicionales de las relaciones
        public string TipoAsentamiento { get; set; }
        public string CodigoPostal { get; set; }
        public string NombreMunicipio { get; set; }
        public short? NoMunicipio { get; set; }
        public string NombreCiudad { get; set; }
        public string NombreEntidad { get; set; }
        public string AbreviaturaEntidad { get; set; }
    }
}
