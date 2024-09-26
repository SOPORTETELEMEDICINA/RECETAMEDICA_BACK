using System.Net;

namespace RMD.Models.Responses
{
    public class ResponseFromService<T>
    {
        public string Message { get; set; } = string.Empty; // Inicializado con un valor por defecto
        public T Data { get; set; } = default!; // Se garantiza que Data será inicializado

        public static ResponseFromService<T> Success(T data, string message = "")
        {
            return new ResponseFromService<T>
            {
                Message = message,
                Data = data
            };
        }

        public static ResponseFromService<T> Failure(HttpStatusCode statusCode, string message = "")
        {
            return new ResponseFromService<T>
            {
                Message = message,
                Data = default! // Garantiza que Data nunca sea null
            };
        }
    }
}
