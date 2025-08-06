namespace RMD.Models.Sucursales
{
    public class UpdateSucursalModel
    {
        public string Nombre { get; set; }
        public string RegistroSanitario { get; set; }
        public string Responsable { get; set; }
        public string CedulaResponsable { get; set; }
        public string TelefonoResponsable { get; set; }
        public string EmailResponsable { get; set; }
        public string Domicilio { get; set; }
        public int IdAsentamiento { get; set; }
    }

}
