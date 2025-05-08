using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using RMD.Data;
using RMD.Interface.Auth;

namespace RMD.Extensions.System
{
    public class ValidateTokenFilter : IAsyncActionFilter
    {
        private readonly IAuthService _authService;
        private readonly UsuariosDBContext _dbContext;

        public ValidateTokenFilter(IAuthService authService, UsuariosDBContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }
        //public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        //{
        //    // Obtener el token desde el encabezado Authorization
        //    var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        //    // Validar si el token está presente
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        context.Result = new JsonResult(new { Message = "El token no fue proporcionado." }) { StatusCode = 401 };
        //        return;
        //    }

        //    // Verificar si el token está activo en la tabla
        //    var isActive = await _authService.IsTokenActiveAsync(token);

        //    if (!isActive.Data)
        //    {
        //        context.Result = new JsonResult(new { Message = "El token es inválido o ha expirado." }) { StatusCode = 401 };
        //        return;
        //    }

        //    // Continuar con la ejecución de la acción
        //    await next();
        //}
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1) Extraer controller llamado
            var controller = context.ActionDescriptor
                                    .RouteValues["controller"] ?? string.Empty;

            // 2) Si no es ConsultaController, dejar pasar
            if (!controller.Equals("Consulta", StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            // 3) Para ConsultaController, validar token primero
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new JsonResult(new { Message = "Token no proporcionado." }) { StatusCode = 401 };
                return;
            }

            // 4) Verificar activo
            var isActive = await _authService.IsTokenActiveAsync(token);
            if (!isActive.Data)
            {
                context.Result = new JsonResult(new { Message = "Token inválido o expirado." }) { StatusCode = 401 };
                return;
            }

            // 5) Chequear último 2FA
            var registro = await _dbContext.AuthTokens
                .Where(t => t.Token == token)
                .Select(t => t.LastAuth2F)
                .FirstOrDefaultAsync();

            if (registro == null || registro.Value.AddMinutes(59) < DateTime.UtcNow)
            {
                context.Result = new JsonResult(new { Message = "Debe re-autentificarse (2FA expirado)." })
                { StatusCode = 401 };
                return;
            }

            // 6) Todo OK: ejecutar acción
            await next();
        }
    }
}

