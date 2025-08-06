//using Microsoft.AspNetCore.Mvc.Filters;
//using RMD.Interface.Auth;

//namespace RMD.Extensions.System
//{
//    public class ValidateTokenFilter : IAsyncActionFilter
//    {
//        private readonly IAuthService _authService;

//        public ValidateTokenFilter(IAuthService authService)
//        {
//            _authService = authService;
//        }
//        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        {
//            // 1) Extraer controller llamado
//            var controller = context.ActionDescriptor
//                                    .RouteValues["controller"] ?? string.Empty;

//            // 2) Si no es ConsultaController, dejar pasar
//            if (!controller.Equals("Consulta", StringComparison.OrdinalIgnoreCase))
//            {
//                await next();
//                return;
//            }

//            // 3) Para ConsultaController, validar token primero
//            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
//            var token = authHeader?.Split(" ").Last();
//            if (string.IsNullOrEmpty(token))
//            {
//                context.Result = new JsonResult(new { Message = "Token no proporcionado." }) { StatusCode = 401 };
//                return;
//            }

//            // 4) Verificar activo
//            var isActive = await _authService.IsTokenActiveAsync(token);
//            if (!isActive.Data)
//            {
//                context.Result = new JsonResult(new { Message = "Token inválido o expirado." }) { StatusCode = 401 };
//                return;
//            }

//            // 6) Todo OK: ejecutar acción
//            await next();
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc.Filters;
using RMD.Interface.Auth;

namespace RMD.Extensions.System
{
    public class ValidateTokenFilter : IAsyncActionFilter
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public ValidateTokenFilter(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controllerName = context.ActionDescriptor.RouteValues["controller"] ?? string.Empty;

            // Saltar si tiene [AllowAnonymous]
            var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                .Any(em => em is AllowAnonymousAttribute);
            if (hasAllowAnonymous)
            {
                await next();
                return;
            }

            var country = _configuration["Country"]?.ToUpperInvariant();

            switch (country)
            {
                case "ES":
                    // Si ES y controlador es PuntoVenta => saltar validación
                    if (controllerName.Equals("PuntoVenta", StringComparison.OrdinalIgnoreCase))
                    {
                        await next();
                        return;
                    }
                    break;
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new JsonResult(new { Message = "Token no proporcionado." }) { StatusCode = 401 };
                return;
            }

            var isActive = await _authService.IsTokenActiveAsync(token);
            if (!isActive.Data)
            {
                context.Result = new JsonResult(new { Message = "Token inválido o expirado." }) { StatusCode = 401 };
                return;
            }

            await next();
        }
    }
}

