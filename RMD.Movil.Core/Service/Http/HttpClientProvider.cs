namespace RMD.Movil.Core.Service.Http
{
    public static class HttpClientProvider
    {
        public static HttpClient GetClient(string region)
        {
            var handler = new DecryptingHandler
            {
                InnerHandler = new HttpClientHandler()
            };

            var baseUrl = GetBaseUrlForRegion(region);

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public static HttpClient GetClient(string region, HttpMessageHandler customHandler)
        {
            var handler = new DecryptingHandler
            {
                InnerHandler = customHandler
            };

            var baseUrl = GetBaseUrlForRegion(region);

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        private static string GetBaseUrlForRegion(string region)
        {
            return region switch
            {
                "MX" => "https://apidev.recetamedica.digital/",
                "ES" => "https://apieurope.recetamedica.digital/",
                _ => "https://apidev.recetamedica.digital/"
            };
        }
    }
}