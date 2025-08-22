using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Login;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using OneSignalSDK.DotNet;

// 👇 para usar nameof(ForgotPasswordPage)

namespace RMD.Movil.PageModels
{
    public class LoginPageModel : INotifyPropertyChanged
    {
        private readonly IAuthControllerService _authService;
        private readonly IPacienteControllerService _pacienteService;
        private string _usuario = string.Empty;
        private string _password = string.Empty;
        private bool _isPasswordVisible;

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

            TogglePasswordCommand = new Command(() => IsPasswordVisible = !IsPasswordVisible);

            LoginCommand = new Command(async () => await RealizarLogin());

            // 👉 Navega a la nueva vista
            ForgotPasswordCommand = new Command(async void () =>
            {
                await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
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
                var usuarioJson = JsonSerializer.Serialize(result.Data);
                Preferences.Default.Set("UsuarioGuardado", usuarioJson);

                var idUsuario = result.Data.User.IdUsuario;
                var pacienteResponse = await _pacienteService.GetPacienteByIdUsuarioAsync(idUsuario);

                if (pacienteResponse.Toast?.ToLower() is not ("success" or "info"))
                {
                    if (MainPage != null)
                        await MainPage.DisplayAlert("Error", "No se pudo obtener el paciente", "OK");
                    return;
                }

                var pacienteJson = JsonSerializer.Serialize(pacienteResponse.Data);
                Preferences.Default.Set("Paciente", pacienteJson);

                var idPaciente = pacienteResponse.Data.IdPaciente;
                if (idPaciente != Guid.Empty)
                {
                    OneSignal.Login(idPaciente.ToString());
                }

                if (MainPage != null)
                    await MainPage.DisplayAlert("Bienvenido", $"Hola {result.Data.User.Nombres}", "OK");

                await Shell.Current.GoToAsync(nameof(HomePage));
            }
            else
            {
                var titulo = toast == "warning" ? "Advertencia" : "Error";
                var mensaje = (result.Descripcion != null && result.Descripcion.Count > 0)
                    ? string.Join("\n", result.Descripcion)
                    : result.Message;

                if (MainPage != null)
                    await MainPage.DisplayAlert(titulo, mensaje, "OK");
            }
        }

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
