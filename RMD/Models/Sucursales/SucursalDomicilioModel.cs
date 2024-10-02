namespace RMD.Models.Sucursales
{
    public class SucursalDomicilioModel
    {
        public Guid IdSucursal { get; set; }
        public string NombreSucursal { get; set; } = string.Empty;
        public string RegistroSanitario { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
        public string CedulaResponsable { get; set; } = string.Empty;
        public string TelefonoResponsable { get; set; } = string.Empty;
        public string EmailResponsable { get; set; } = string.Empty;
        public string Calle { get; set; } = string.Empty;
        public string NombreAsentamiento { get; set; } = string.Empty;
        public string TipoAsentamiento { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string AbreviaturaEstado { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

}
