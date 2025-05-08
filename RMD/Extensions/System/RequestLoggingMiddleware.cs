using Microsoft.AspNetCore.Mvc;
using RMD.Models.Login;
using RMD.Models.Sucursales;
using RMD.Models.Usuarios;

namespace RMD.Extensions.System
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            string controller = context.GetRouteValue("controller")?.ToString() ?? "Desconocido";

            // **1️⃣ EXCLUIR `AuthController` PARA EVITAR ERRORES**
            if (controller.Equals("Auth", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            // 🔹 Declarar las variables antes del `try`
         
            string endpoint = "Desconocido";
            string idGEMP = string.Empty;
            string idSucursal = string.Empty;
            string idUsuario = string.Empty;
            string idRol = string.Empty;
            string parametros = string.Empty;

            try
            {
                if (request.Method == HttpMethods.Get)
                {
                    // 🔹 Capturar solo los valores relevantes de RouteValues, excluyendo "controller" y "action"
                    var routeParams = context.Request.RouteValues
                        .Where(kv => kv.Key != "controller" && kv.Key != "action")
                        .ToDictionary(kv => kv.Key, kv => kv.Value);

                    parametros = routeParams.Count > 0
                        ? string.Join(", ", routeParams.Select(kv => $"{kv.Key}: {kv.Value}"))
                        : "Sin parámetros en RouteValues";
                }
                else
                {
                    // 🔹 Leer el cuerpo (Body) solo si no es GET
                    parametros = await ReadRequestBody(context);
                }

                controller = context.GetRouteValue("controller")?.ToString() ?? "Desconocido";
                endpoint = context.GetRouteValue("action")?.ToString() ?? request.Path.ToString();
        

                var userClaims = context.User;
                idGEMP = GetClaimValue(userClaims, "GEMP");
                idSucursal = GetClaimValue(userClaims, "IdSucursal");
                idUsuario = GetClaimValue(userClaims, "IdUsuario");
                idRol = GetClaimValue(userClaims, "IdRol");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var log = new RegistroPeticion
                    {
                        Fecha = DateTime.Now,
                        Controller = controller,
                        Endpoint = endpoint,
                        IdGEMP = Guid.TryParse(idGEMP, out var gemp) ? gemp : (Guid?)null,
                        IdSucursal = Guid.TryParse(idSucursal, out var sucursal) ? sucursal : (Guid?)null,
                        IdUsuario = Guid.TryParse(idUsuario, out var usuario) ? usuario : (Guid?)null,
                        IdRol = Guid.TryParse(idRol, out var rol) ? rol : (Guid?)null,  // 🔹 CORRECCIÓN AQUÍ
                        Parametros = parametros
                    };
                    dbContext.RegistroPeticiones.Add(log);
                    await dbContext.SaveChangesAsync();
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"❌ ERROR AL GUARDAR EN BD: {innerException}");
                Console.WriteLine($"🔹 Controller: {controller}");
                Console.WriteLine($"🔹 Endpoint: {endpoint}");
                Console.WriteLine($"🔹 IdGEMP: {idGEMP}");
                Console.WriteLine($"🔹 IdSucursal: {idSucursal}");
                Console.WriteLine($"🔹 IdUsuario: {idUsuario}");
                Console.WriteLine($"🔹 IdRol: {idRol}");
                Console.WriteLine($"🔹 Parametros: {parametros}");

                throw;
            }
        }

        private async Task<string> ReadRequestBody(HttpContext context)
        {
            if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
                return null;

            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
                return body;
            }
        }

        private string GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            return user.Claims.FirstOrDefault(c => c.Type == claimType)?.Value ?? string.Empty;
        }
    }
}
