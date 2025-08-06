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
                await Shell.Current.GoToAsync("///LoginPage");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", result.Message, "OK");
            }
        }


        [RelayCommand]
        private async Task VerRecetas() => await Shell.Current.GoToAsync("RecetasPage");

        [RelayCommand]
        private async Task EventoSalud()
        {
            GC.Collect(); // fuerza recolección de memoria
            await Task.Delay(50); // da tiempo al render
            await Shell.Current.GoToAsync("EventosPage");
        }

        [RelayCommand]
        private async Task Medicacion() => await Shell.Current.GoToAsync("AvisosMedicacionPage");
        [RelayCommand]
        private async Task PerfilSalud() => await Shell.Current.GoToAsync("PerfilPage");
    }
}