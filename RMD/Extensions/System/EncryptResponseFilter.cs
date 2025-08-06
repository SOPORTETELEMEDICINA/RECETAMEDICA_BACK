using System.Text.Json.Serialization;
using System.Xml.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Filters;
using RMD.Interface.Security;
using RMD.Shared.Utils.Interface;

namespace RMD.Extensions.System
{
    public class EncryptResponseFilter : IAsyncResultFilter
    {
        private readonly IEncryptionService _encryption;

        public EncryptResponseFilter(IEncryptionService encryption)
            => _encryption = encryption;

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objResult
                && objResult.Value is not null
                && objResult.Value.GetType().IsGenericType
                && objResult.Value.GetType().GetGenericTypeDefinition() == typeof(ResponseFromService<>))
            {
                var originalType = objResult.Value.GetType();
                var dataProp = originalType.GetProperty("Data")!;
                var msgProp = originalType.GetProperty("Message")!;
                var codeProp = originalType.GetProperty("Code")!;
                var toastProp = originalType.GetProperty("Toast")!;
                var descProp = originalType.GetProperty("Descripcion")!;

                var dataValue = dataProp.GetValue(objResult.Value);
                if (dataValue is null)
                {
                    await next();
                    return;
                }

                string plainJson;

                try
                {
                    // 1) Si el Data es directamente un XDocument
                    if (dataValue is XDocument xml)
                    {
                        plainJson = xml.ToString();
                    }
                    // 2) Si Data contiene una propiedad que es un XDocument
                    else
                    {
                        var props = dataValue.GetType().GetProperties();
                        var xmlProp = props.FirstOrDefault(p => p.PropertyType == typeof(XDocument));

                        // NUEVA VALIDACIÓN:
                        // Si el tipo del modelo contiene un XDocument pero NO ES un XDocument directamente → serializa todo el modelo como JSON
                        if (xmlProp != null)
                        {
                            var jsonOpts = new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                            };

                            plainJson = JsonSerializer.Serialize(dataValue, dataValue.GetType(), jsonOpts);
                        }
                        // 3) Si es HTML
                        else if (dataValue is HtmlString html)
                        {
                            plainJson = html.ToString();
                        }
                        // 4) Si es string que aparenta ser HTML
                        else if (dataValue is string str &&
                                 (str.TrimStart().StartsWith("<html", StringComparison.OrdinalIgnoreCase) ||
                                  str.TrimStart().StartsWith("<!DOCTYPE html", StringComparison.OrdinalIgnoreCase) ||
                                  str.TrimStart().StartsWith("<!DOCTYPE HTML", StringComparison.OrdinalIgnoreCase) ||
                                  str.TrimStart().StartsWith("<!DOCTYPE HTML PUBLIC", StringComparison.OrdinalIgnoreCase)))
                        {
                            plainJson = str;
                        }
                        // 5) Caso normal: objeto serializable
                        else
                        {
                            var jsonOpts = new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                            };

                            plainJson = JsonSerializer.Serialize(dataValue, dataValue.GetType(), jsonOpts);
                        }
                    }
                }
                catch
                {
                    await next();
                    return;
                }

                // Cifrado
                var encrypted = _encryption.EncryptString(plainJson);

                // Reensamblar respuesta
                var newResp = new ResponseFromService<string>
                {
                    Code = (int)codeProp.GetValue(objResult.Value)!,
                    Message = (string)msgProp.GetValue(objResult.Value)!,
                    Toast = (string)toastProp.GetValue(objResult.Value)!,
                    Descripcion = (List<string>)descProp.GetValue(objResult.Value)!,
                    Data = encrypted
                };

                context.Result = new ObjectResult(newResp)
                {
                    StatusCode = objResult.StatusCode
                };
            }

            await next();
        }
    }
}
