using CommunityToolkit.Mvvm.ComponentModel;
using RMD.Shared.Models.Login;
using System.Text.Json;

namespace RMD.Movil.PageModels.Controls;

public partial class HeaderPageModel : ObservableObject
{
    [ObservableProperty]
    private string nombreUsuario = string.Empty;

    [ObservableProperty]
    private ImageSource imagenUsuarioBase64 = ImageSource.FromFile("usuario_default.png");

    public HeaderPageModel()
    {
        CargarDatosUsuario();
    }

    private void CargarDatosUsuario()
    {
        var json = Preferences.Default.Get<string>("UsuarioGuardado", null!);

        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                var usuario = JsonSerializer.Deserialize<LoginResult>(json);
                NombreUsuario = $"{usuario?.User?.Nombres} {usuario?.User?.PrimerApellido}";

                if (!string.IsNullOrEmpty(usuario?.User?.Imagen))
                {
                    var bytes = Convert.FromBase64String(usuario.User.Imagen);
                    ImagenUsuarioBase64 = ImageSource.FromStream(() => new MemoryStream(bytes));
                }
            }
            catch
            {
                NombreUsuario = "Usuario";
            }
        }
        else
        {
            NombreUsuario = "Usuario";
        }
    }
}
