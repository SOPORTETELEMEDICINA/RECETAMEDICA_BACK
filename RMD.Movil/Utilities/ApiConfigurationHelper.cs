namespace RMD.Movil.Utilities
{
    public static class ApiConfigurationHelper
    {
        public static Uri GetBaseAddressForRegion(string region) =>
            region switch
            {
                "MX" => new Uri("https://apidev.recetamedica.digital/"),
                "ES" => new Uri("https://apieurope.recetamedica.digital/"),
                "AR" => new Uri("https://apiargentina.recetamedica.digital/"),
                "CL" => new Uri("https://apichile.recetamedica.digital/"),
                _ => new Uri("https://apidev.recetamedica.digital/")
            };
    }
}