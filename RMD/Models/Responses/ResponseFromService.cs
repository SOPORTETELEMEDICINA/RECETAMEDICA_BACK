using RMD.Models.CatalogoErrors;

namespace RMD.Models.Responses
{
    // Enum para que el front sepa qué toast mostrar
    public enum ToastType
    {
        SUCCESS,
        INFO,
        WARNING,
        ERROR
    }

    public class ResponseFromService<T>
    {
        // Un código numérico para identificar el error o éxito
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; } = default!;
        // Detalles adicionales de error si hace falta, para que el front no se quede con la duda
        public List<string> Descripcion { get; set; } = [];
        // Tipo de toast para facilitarle la vida al front (y a quien tenga que ver las notificaciones)
        public string Toast { get; set; } = "info";



        public static ResponseFromService<T> Success(T data, CatalogoNotificacion MensajeCatalog)
        {
            return new ResponseFromService<T>
            {
                Code = MensajeCatalog.CodigoNotificacion, // 0 significa que todo salió bien
                Message = MensajeCatalog.Mensaje,
                Toast = MensajeCatalog.ToastType.ToLowerInvariant(),
                Descripcion = new List<string> { MensajeCatalog.Descripcion },
                Data = data
            };
        }

        public static ResponseFromService<T> Failure(CatalogoNotificacion MensajeCatalog)
        {
            return new ResponseFromService<T>
            {
                Code = MensajeCatalog.CodigoNotificacion, // 0 significa que todo salió bien
                Message = MensajeCatalog.Mensaje,
                Toast = MensajeCatalog.ToastType.ToLowerInvariant(),
                Descripcion = new List<string> { MensajeCatalog.Descripcion },
                Data = default!
            };
        }
        public static ResponseFromService<T> Exeption(Exception exception, CatalogoNotificacion MensajeCatalog)
        {
            return new ResponseFromService<T>
            {
                Code = MensajeCatalog.CodigoNotificacion, // 0 significa que todo salió bien
                Message = MensajeCatalog.Mensaje,
                Toast = MensajeCatalog.ToastType.ToLowerInvariant(),
                Descripcion = new List<string> { exception.Message },
                Data = default!
            };
        }
        public static ResponseFromService<T> HttpRequestException(HttpRequestException exception, CatalogoNotificacion MensajeCatalog)
        {
            return new ResponseFromService<T>
            {
                Code = MensajeCatalog.CodigoNotificacion, // 0 significa que todo salió bien
                Message = MensajeCatalog.Mensaje,
                Toast = MensajeCatalog.ToastType.ToLowerInvariant(),
                Descripcion = new List<string> { exception.Message },
                Data = default!
            };
        }
        
    }
}
