// Filters/EncryptResponseFilter.cs
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RMD.Interface.Security;
using RMD.Models.Responses;

public class EncryptResponseFilter : IAsyncResultFilter
{
    private readonly IEncryptionService _encryption;
    public EncryptResponseFilter(IEncryptionService encryption) => _encryption = encryption;

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        // Solo nos interesan los ObjectResult que contengan un ResponseFromService<*>
        if (context.Result is ObjectResult objResult
            && objResult.Value != null
            && objResult.Value.GetType().IsGenericType
            && objResult.Value.GetType().GetGenericTypeDefinition() == typeof(ResponseFromService<>))
        {
            var originalType = objResult.Value.GetType();                         // ResponseFromService<OriginalT>
            var dataProp = originalType.GetProperty("Data")!;
            var msgProp = originalType.GetProperty("Message")!;
            var codeProp = originalType.GetProperty("Code")!;
            var toastProp = originalType.GetProperty("Toast")!;
            var descProp = originalType.GetProperty("Descripcion")!;

            // extraemos el objeto Data
            var dataValue = dataProp.GetValue(objResult.Value);
            // serializamos a JSON
            var plainJson = JsonSerializer.Serialize(dataValue);
            // ciframos ("IV:cipherText")
            var encrypted = _encryption.EncryptString(plainJson);

            // construimos la nueva ResponseFromService<string>
            var newResp = new ResponseFromService<string>
            {
                Code = (int)codeProp.GetValue(objResult.Value)!,
                Message = (string)msgProp.GetValue(objResult.Value)!,
                Toast = (string)toastProp.GetValue(objResult.Value)!,
                Descripcion = (List<string>)descProp.GetValue(objResult.Value)!,
                Data = encrypted
            };

            // reemplazamos el resultado
            context.Result = new ObjectResult(newResp)
            {
                StatusCode = objResult.StatusCode
            };
        }

        await next();
    }
}
