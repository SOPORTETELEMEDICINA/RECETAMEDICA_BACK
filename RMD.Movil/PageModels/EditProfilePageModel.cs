using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Login;
using RMD.Shared.Models.Usuarios;
using System.Text.Json;

namespace RMD.Movil.PageModels;

public partial class EditProfilePageModel : ObservableObject
{
    private readonly IUsuariosControllerService _usuariosService;

    // Datos de usuario actual
    private Guid _idUsuario;

    [ObservableProperty] private string displayName = string.Empty;   // solo lectura en la UI
    [ObservableProperty] private string email = string.Empty;         // solo lectura en la UI
    [ObservableProperty] private ImageSource profileImage = ImageSource.FromFile("usuario_default.png");

    // Estado
    [ObservableProperty] private bool isBusy;
    public bool IsNotBusy => !IsBusy;
    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(IsNotBusy));

    // Contraseña (USAR generator para evitar ambigüedad)
    [ObservableProperty] private string newPassword = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;

    [ObservableProperty] private string passwordError = string.Empty;
    public bool HasPasswordError => !string.IsNullOrWhiteSpace(PasswordError);
    partial void OnPasswordErrorChanged(string value) => OnPropertyChanged(nameof(HasPasswordError));

    // Imagen seleccionada (Base64) para enviar al API
    private string? _selectedImageBase64;

    public EditProfilePageModel(IUsuariosControllerService usuariosService)
    {
        _usuariosService = usuariosService;
        CargarUsuarioDesdePreferencias();
    }

    private void CargarUsuarioDesdePreferencias()
    {
        var json = Preferences.Default.Get<string>("UsuarioGuardado", null);
        if (string.IsNullOrWhiteSpace(json)) return;

        try
        {
            var login = JsonSerializer.Deserialize<LoginResult>(json);
            if (login?.User is null) return;

            _idUsuario = login.User.IdUsuario;
            DisplayName = $"{login.User.Nombres} {login.User.PrimerApellido} {login.User.SegundoApellido}".Trim();
            Email = login.User.Email ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(login.User.Imagen))
            {
                var bytes = Convert.FromBase64String(login.User.Imagen);
                ProfileImage = ImageSource.FromStream(() => new MemoryStream(bytes));
            }
        }
        catch
        {
            // ignorar parseo
        }
    }

    [RelayCommand]
    private async Task Back() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task PickPhoto()
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Selecciona una imagen"
            });
            if (result == null) return;

            using var stream = await result.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var bytes = ms.ToArray();

            _selectedImageBase64 = Convert.ToBase64String(bytes);
            ProfileImage = ImageSource.FromStream(() => new MemoryStream(bytes));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Imagen", $"No se pudo seleccionar la imagen: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            // Validar contraseña solo si hay algo escrito
            PasswordError = string.Empty;
            if (!string.IsNullOrWhiteSpace(NewPassword) || !string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    PasswordError = "Debes completar ambos campos de contraseña.";
                    return;
                }
                if (NewPassword.Length < 8)
                {
                    PasswordError = "La contraseña debe tener al menos 8 caracteres.";
                    return;
                }
                if (NewPassword != ConfirmPassword)
                {
                    PasswordError = "Las contraseñas no coinciden.";
                    return;
                }

                var pwdReq = new CambiarPasswordRequest
                {
                    NuevaPassword = NewPassword,
                    ConfirmacionPassword = ConfirmPassword
                };
                var pwdResp = await _usuariosService.CambiarPasswordAsync(pwdReq);
                var toast = pwdResp.Toast?.ToLower() ?? "error";
                var msg = (pwdResp.Descripcion?.Any() == true) ? string.Join("\n", pwdResp.Descripcion) : pwdResp.Message;

                await Shell.Current.DisplayAlert("Contraseña", msg, "OK");
                if (!(toast is "success" or "info"))
                    return;

                // Limpiar campos si fue bien
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
            }

            // Subir imagen si cambió
            if (!string.IsNullOrWhiteSpace(_selectedImageBase64))
            {
                var imgReq = new UsuarioImagenRequest
                {
                    IdUsuario = _idUsuario,
                    Imagen = _selectedImageBase64,
                    Firma = null
                };

                var imgResp = await _usuariosService.ImagenFirmaAsync(imgReq);
                var toast = imgResp.Toast?.ToLower() ?? "error";
                var msg = (imgResp.Descripcion?.Any() == true) ? string.Join("\n", imgResp.Descripcion) : imgResp.Message;

                await Shell.Current.DisplayAlert("Foto de perfil", msg, "OK");
                if (!(toast is "success" or "info"))
                    return;

                // Actualizar cache local
                var json = Preferences.Default.Get<string>("UsuarioGuardado", null);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var login = JsonSerializer.Deserialize<LoginResult>(json);
                    if (login?.User != null)
                    {
                        login.User.Imagen = _selectedImageBase64;
                        Preferences.Default.Set("UsuarioGuardado", JsonSerializer.Serialize(login));
                    }
                }

                _selectedImageBase64 = null;
            }

            await Shell.Current.DisplayAlert("Perfil", "¡Cambios guardados!", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
