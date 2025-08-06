namespace RMD.Shared.Models.Receta.AlertaToma.Response
{
    public class BuscarPackageResponse
    {
        public int PackageId { get; set; }
        public required string NombreComercial { get; set; }
        public required string FormaFarmaceutica { get; set; }
        public required string Presentacion { get; set; }
        public string CodigoNacional { get; set; } = string.Empty;
    }
}
