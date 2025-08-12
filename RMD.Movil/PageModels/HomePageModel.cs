using CommunityToolkit.Mvvm.Input;
using RMD.Movil.Core.Service.Interfaces;

namespace RMD.Movil.PageModels
{
    public partial class HomePageModel(IAuthControllerService authService)
    {
        [RelayCommand]
        async Task CerrarSesion()
        {
            var result = await authService.LogoutAsync();

            if ((result.Toast?.ToLower()) is "success" or "info")
            {
                Preferences.Default.Remove("UsuarioGuardado");
                Preferences.Default.Remove("Paciente");
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", result.Message, "OK");
            }
        }


        [RelayCommand]
        private async Task VerRecetas()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(RecetasPage));
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                await Shell.Current.DisplayAlert("Navegación", msg, "OK");
            }
        }

        [RelayCommand]
        private Task EventoSalud() => Shell.Current.GoToAsync(nameof(EventosPage));

        [RelayCommand]
        private Task Medicacion() => Shell.Current.GoToAsync(nameof(AvisosMedicacionPage));

        [RelayCommand]
        private Task PerfilSalud() => Shell.Current.GoToAsync(nameof(EditProfilePage));

    }
}