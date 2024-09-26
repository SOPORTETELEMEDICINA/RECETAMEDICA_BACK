namespace RMD.Models.Dashboard
{
    public class DashBoardKPIPacientesRecetas
    {
        public Guid IdGEMP { get; set; }
        public Guid IdSucursal { get; set; }
        public int CountPacientes { get; set; }
        public int CountRecetas { get; set; }
    }
}
