using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using RMD.Interface.Auth;

namespace RMD.Extensions
{
    public class ValidateTokenFilter : IAsyncActionFilter
    {
        private readonly IAuthService _authService;

        public ValidateTokenFilter(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Obtener el token desde el encabezado Authorization
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                // Verificar si el token está activo
                var isActive = await _authService.IsTokenActiveAsync(token);

                if (!isActive)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }

            await next();
        }
    }
}
