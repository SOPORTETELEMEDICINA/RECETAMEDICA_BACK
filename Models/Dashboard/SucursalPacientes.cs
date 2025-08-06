namespace RMD.Models.Dashboard
{
    public class SucursalPacientes
    {
        public Guid IdGEMP { get; set; }
        public Guid IdSucursal { get; set; }
        public required string NombreSucursal { get; set; } 
        public int TotalPacientes { get; set; }
    }
}
