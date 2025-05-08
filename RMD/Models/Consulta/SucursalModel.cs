namespace RMD.Models.Consulta
{
    public class SucursalModel
    {
        public Guid IdSucursal { get; set; }
        public string Nombre { get; set; }
        public string Domicilio { get; set; }
        public string TelefonoResponsable { get; set; }
        public string EmailResponsable { get; set; }
        public static SucursalModel FromDataReader(IDataRecord r) => new SucursalModel
        {
            IdSucursal = r.GetGuid(r.GetOrdinal("IdSucursal")),
            Nombre = r.GetString(r.GetOrdinal("Nombre")),
            Domicilio = r.GetString(r.GetOrdinal("Domicilio")),
            TelefonoResponsable = r.GetString(r.GetOrdinal("TelefonoResponsable")),
            EmailResponsable = r.GetString(r.GetOrdinal("EmailResponsable")),
        };
    }

}
