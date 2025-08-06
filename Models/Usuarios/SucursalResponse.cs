namespace RMD.Models.Usuarios
{
    public class SucursalResponse
    {
        public Guid IdGEMP { get; set; }
        public Guid IdSucursal { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string RegistroSanitario { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
        public string CedulaResponsable { get; set; } = string.Empty;
        public string TelefonoResponsable { get; set; } = string.Empty;
        public string EmailResponsable { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        public int IdAsentamiento { get; set; }
        public string Asentamiento { get; set; } = string.Empty;
        public string TipoAsentamiento { get; set; } = string.Empty;
        public int IdCP { get; set; }
        public string CodigoPostal { get; set; } = string.Empty;
        public int IdMunicipio { get; set; }
        public string Municipio { get; set; } = string.Empty;
        public int IdCiudad { get; set; }
        public string Ciudad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
