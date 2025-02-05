using RMD.Interface.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json;

namespace RMD.Extensions
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEmailService _emailService;

        public ErrorHandlingMiddleware(RequestDelegate next, IEmailService emailService)
        {
            _next = next;
            _emailService = emailService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException)
            {
                // Manejo específico para errores de autorización/autenticación
                await HandleUnauthorizedAsync(context);
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleUnauthorizedAsync(HttpContext context)
        {
            // Configurar el código de estado HTTP
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            // Mensaje de error para el cliente
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "No tiene permisos para acceder a este recurso."
            }));

            Console.WriteLine("Acceso no autorizado detectado.");
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Obtener información sobre el endpoint
            var endpoint = context.GetEndpoint();
            var routePattern = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Routing.RouteEndpoint>()?.RoutePattern?.RawText ?? "N/A";
            var controllerName = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()?.ControllerName ?? "Unknown";
            var actionName = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()?.ActionName ?? "Unknown";

            // Crear el mensaje de error detallado
            var errorDetails = $@"
                Ocurrió un error:
                - Controlador: {controllerName}
                - Acción: {actionName}
                - Ruta: {routePattern}
                - Mensaje: {exception.Message}
                - Stack Trace: {exception.StackTrace}
            ";

            // Log del error
            Console.WriteLine(errorDetails);

            // Enviar correo usando el servicio de correo
            try
            {
                await _emailService.SendErrorByEmailAsync("Error en la API", errorDetails);
            }
            catch (Exception emailException)
            {
                // Manejo de fallos en el envío del correo
                Console.WriteLine($"No se pudo enviar el correo del error: {emailException.Message}");
            }

            // Respuesta estándar para el cliente
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = "Ocurrió un error inesperado. El administrador ha sido notificado.",
                controller = controllerName,
                action = actionName
            }));
        }
    }
}
