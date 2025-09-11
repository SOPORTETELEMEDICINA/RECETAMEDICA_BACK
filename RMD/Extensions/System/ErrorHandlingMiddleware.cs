using RMD.Interface.Auth;

namespace RMD.Extensions.System
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly IEmailService _emailService;

        private readonly IServiceProvider _serviceProvider;

        //public ErrorHandlingMiddleware(RequestDelegate next, IEmailService emailService)
        //{
        //    _next = next;
        //    _emailService = emailService;
        //}
        public ErrorHandlingMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException)
            {
                await HandleUnauthorizedAsync(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        private async Task HandleUnauthorizedAsync(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "No tiene permisos para acceder a este recurso."
            }));
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Crear un nuevo scope para obtener el servicio IEmailService
            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var errorDetails = $@"
                Ocurrió un error:
                - Mensaje: {exception.Message}
                - Stack Trace: {exception.StackTrace}
            ";

            // Log del error
            Console.WriteLine(errorDetails);

            // Enviar correo usando el servicio de correo
            try
            {
                await emailService.SendErrorByEmailAsync("Error en la API", errorDetails);
            }
            catch (Exception emailException)
            {
                Console.WriteLine($"No se pudo enviar el correo del error: {emailException.Message}");
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "Ocurrió un error inesperado. El administrador ha sido notificado."
            }));
        }
       
    }
}
