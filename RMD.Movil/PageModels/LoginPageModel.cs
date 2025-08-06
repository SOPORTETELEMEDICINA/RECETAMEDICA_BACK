using System.Collections.Generic;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Login;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace RMD.Movil.PageModels
{
    public partial class LoginPageModel : INotifyPropertyChanged
    {
        private readonly IAuthControllerService _authService;
        private readonly IPacienteControllerService _pacienteService;
        private string _usuario;
        private string _password;
        private bool _isPasswordVisible;


        public LoginPageModel(IAuthControllerService authService)
        {
            this._authService = authService;
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Usuario
        {
            get => _usuario;
            set => SetProperty(ref _usuario, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set => SetProperty(ref _isPasswordVisible, value);
        }

        public ICommand TogglePasswordCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        public LoginPageModel(IAuthControllerService authService, IPacienteControllerService pacienteService)
        {
            _authService = authService;
            _pacienteService = pacienteService;
            _usuario = string.Empty;
            _password = string.Empty;

            TogglePasswordCommand = new Command(() => IsPasswordVisible = !IsPasswordVisible);

            LoginCommand = new Command(async () => await RealizarLogin());

            ForgotPasswordCommand = new Command(async () =>
            {
                if (MainPage != null)
                    await MainPage.DisplayAlert("Recuperación", "Redirigiendo a recuperación...", "OK");
            });
        }

        private static Page? MainPage => Application.Current?.Windows[0].Page;

        private async Task RealizarLogin()
        {
            if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Password))
            {
                if (MainPage != null)
                    await MainPage.DisplayAlert("Error", "Completa ambos campos", "OK");
                return;
            }

            var result = await _authService.LoginAsync(new UserCredentials
            {
                Usr = Usuario,
                Password = Password
            });

            var toast = result.Toast?.ToLower() ?? string.Empty;

            if (toast == "success")
            {
                // Guardar datos de login
                var usuarioJson = JsonSerializer.Serialize(result.Data);
                Preferences.Default.Set("UsuarioGuardado", usuarioJson);

                // Obtener paciente desde el API
                var idUsuario = result.Data.User.IdUsuario;
                var pacienteResponse = await _pacienteService.GetPacienteByIdUsuarioAsync(idUsuario);

                if (pacienteResponse.Toast?.ToLower() != "success" && pacienteResponse.Toast?.ToLower() != "info")
                {
                    if (MainPage != null)
                        await MainPage.DisplayAlert("Error", "No se pudo obtener el paciente", "OK");

                    return; // Detener flujo, no navegar
                }

                // Guardar paciente
                var pacienteJson = JsonSerializer.Serialize(pacienteResponse.Data);
                Preferences.Default.Set("Paciente", pacienteJson);

                // Mostrar mensaje de bienvenida
                if (MainPage != null)
                    await MainPage.DisplayAlert("Bienvenido", $"Hola {result.Data.User.Nombres}", "OK");

                // Navegar a Home
                await Shell.Current.GoToAsync("///HomePage");
            }
            else
            {
                var titulo = toast == "warning" ? "Advertencia" : "Error";
                var mensaje = result.Descripcion != null && result.Descripcion.Count == 0
                    ? string.Join("\n", result.Descripcion)
                    : result.Message;

                if (MainPage != null)
                    await MainPage.DisplayAlert(titulo, mensaje, "OK");
            }
        }

        //private async Task RealizarLogin()
        //{
        //    if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Password))
        //    {
        //        if (MainPage != null)
        //            await MainPage.DisplayAlert("Error", "Completa ambos campos", "OK");
        //        return;
        //    }

        //    var result = await _authService.LoginAsync(new UserCredentials
        //    {
        //        Usr = Usuario,
        //        Password = Password
        //    });

        //    var toast = result.Toast?.ToLower() ?? string.Empty;

        //    if (toast == "success")
        //    {
        //        // Guardar usuario
        //        var json = System.Text.Json.JsonSerializer.Serialize(result.Data);
        //        Preferences.Default.Set("UsuarioGuardado", json);

        //        if (MainPage != null)
        //            await MainPage.DisplayAlert("Bienvenido", $"Hola {result.Data.User.Nombres}", "OK");

        //        await Shell.Current.GoToAsync("///HomePage");
        //    }

        //    else
        //    {
        //        var titulo = toast == "warning" ? "Advertencia" : "Error";
        //        var mensaje = result.Descripcion != null && result.Descripcion.Count == 0
        //            ? string.Join("\n", result.Descripcion)
        //            : result.Message;

        //        if (MainPage != null)
        //            await MainPage.DisplayAlert(titulo, mensaje, "OK");
        //    }
        //}

        protected void SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(backingField, value))
            {
                backingField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
