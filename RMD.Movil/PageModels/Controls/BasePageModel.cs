using CommunityToolkit.Mvvm.ComponentModel;
using RMD.Shared.Models.GlobalResponse; // <- ResponseFromService<T>

namespace RMD.Movil.PageModels.Controls;

public partial class BasePageModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    // SUCCESS / INFO se consideran OK
    protected static bool EsOkToast(string? toast)
    {
        if (string.IsNullOrWhiteSpace(toast)) return false;
        var t = toast.ToLowerInvariant();
        return t == "success" || t == "info";
    }

    protected static string TituloPorToast(string? toast) =>
        (toast ?? string.Empty).ToLowerInvariant() switch
        {
            "success" => "Éxito",
            "info" => "Información",
            "warning" => "Advertencia",
            "error" => "Error",
            _ => "Mensaje"
        };

    protected static string UnirDescripcion(List<string>? descripciones)
    {
        if (descripciones == null || descripciones.Count == 0) return string.Empty;
        return string.Join("\n", descripciones.Where(s => !string.IsNullOrWhiteSpace(s)));
    }

    protected static async Task MostrarAlertAsync(string titulo, string mensaje)
    {
        var page = Shell.Current?.CurrentPage;
        if (page != null)
            await page.DisplayAlert(titulo, string.IsNullOrWhiteSpace(mensaje) ? "Sin detalle." : mensaje, "OK");
    }

    // Muestra un alert usando el título según el Toast y el cuerpo con Message/Descripcion
    protected static async Task MostrarAlertPorRespuestaAsync<T>(ResponseFromService<T> resp)
    {
        var titulo = TituloPorToast(resp.Toast);
        var cuerpo = !string.IsNullOrWhiteSpace(resp.Message) ? resp.Message : UnirDescripcion(resp.Descripcion);
        await MostrarAlertAsync(titulo, cuerpo);
    }
}

