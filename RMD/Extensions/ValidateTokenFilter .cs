using Microsoft.AspNetCore.Mvc.Filters;
using RMD.Interface.Auth;

namespace RMD.Extensions
{
    public class ValidateTokenFilter(IAuthService authService) : IAsyncActionFilter
    {
        private readonly IAuthService _authService = authService;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Obtener el token desde el encabezado Authorization
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            // Validar si el token está presente
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new JsonResult(new { Message = "El token no fue proporcionado." }) { StatusCode = 401 };
                return;
            }

            // Verificar si el token está activo en la tabla
            var isActive = await _authService.IsTokenActiveAsync(token);

            if (!isActive)
            {
                context.Result = new JsonResult(new { Message = "El token es inválido o ha expirado." }) { StatusCode = 401 };
                return;
            }

            // Continuar con la ejecución de la acción
            await next();
        }
    }
}
