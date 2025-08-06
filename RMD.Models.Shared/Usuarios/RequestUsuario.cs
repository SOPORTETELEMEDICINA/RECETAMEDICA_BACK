namespace RMD.Shared.Models.Usuarios
{
    public class RequestUsuario
    {
        public Guid IdUsuario { get; set; }
        public string? Usr { get; set; }
        public Guid IdTipoUsuario { get; set; }
        public string? TipoUsuario { get; set; }
        public Guid IdGEMP { get; set; }
        public string? Empresa { get; set; }
        public Guid IdSucursal { get; set; }
        public int? NoSucursal { get; set; }
        public string? NombreSucursal { get; set; }
        public string? Nombres { get; set; }
        public string? PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        public int? IdAsentamiento { get; set; }
        public string? NombreAsentamiento { get; set; }
        public int? IdTipoAsentamiento { get; set; }
        public string? TipoAsentamiento { get; set; }
        public int? IdCP { get; set; }
        public string? CodigoPostal { get; set; }
        public int? IdMunicipio { get; set; }
        public short? NoMunicipio { get; set; }
        public string? Municipio { get; set; }
        public int? IdCiudad { get; set; }
        public string? Ciudad { get; set; }
        public int? IdEntidad { get; set; }
        public string? Estado { get; set; }
        public string? Abreviatura { get; set; }
        public string? Domicilio { get; set; }
        public string? Movil { get; set; }
        public string? Email { get; set; }
        public required string Firma { get; set; } = string.Empty;
        public required string Imagen { get; set; } = string.Empty;
        public string? Status { get; set; }

    }
}
