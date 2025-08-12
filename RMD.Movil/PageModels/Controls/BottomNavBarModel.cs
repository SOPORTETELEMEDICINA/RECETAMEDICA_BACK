using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;

namespace RMD.Movil.PageModels.Controls
{
    public partial class BottomNavBarModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public BottomNavBarModel()
        {
            if (Shell.Current != null)
                Shell.Current.Navigated += OnShellNavigated;
        }

        private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
            => RaiseAllIconChanges();

        private void RaiseAllIconChanges()
        {
            OnPropertyChanged(nameof(CurrentRoute));
            OnPropertyChanged(nameof(HomeIcon));
            OnPropertyChanged(nameof(RecetasIcon));
            OnPropertyChanged(nameof(EventosIcon));
            OnPropertyChanged(nameof(MedicacionIcon));
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Ruta segura para URIs relativos
        private static string GetCurrentRouteName()
        {
            var state = Shell.Current?.CurrentState;
            if (state == null) return string.Empty;

            var loc = state.Location;
            if (loc == null) return string.Empty;

            var raw = loc.OriginalString ?? string.Empty;

            var q = raw.IndexOf('?');
            if (q >= 0) raw = raw[..q];
            var hash = raw.IndexOf('#');
            if (hash >= 0) raw = raw[..hash];

            var trimmed = raw.Trim('/');
            if (string.IsNullOrEmpty(trimmed)) return string.Empty;

            var parts = trimmed.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0 ? string.Empty : parts[^1];
        }

        public string CurrentRoute => GetCurrentRouteName();

        // Icons
        public string HomeIcon => CurrentRoute.Equals("HomePage", StringComparison.OrdinalIgnoreCase)
            ? "Images/NavBar/button_home_active.png"
            : "Images/NavBar/button_home.png";

        public string RecetasIcon => CurrentRoute.Equals("RecetasPage", StringComparison.OrdinalIgnoreCase)
            ? "Images/NavBar/button_receta_active.png"
            : "Images/NavBar/button_receta.png";

        public string EventosIcon => CurrentRoute.Equals("EventosPage", StringComparison.OrdinalIgnoreCase)
            ? "Images/NavBar/button_eventos_salud_active.png"
            : "Images/NavBar/button_eventos_salud.png";

        public string MedicacionIcon => CurrentRoute.Equals("AvisosMedicacionPage", StringComparison.OrdinalIgnoreCase)
            ? "Images/NavBar/button_medicamentos_active.png"
            : "Images/NavBar/button_medicamentos.png";



        // Commands que tu XAML está pidiendo
        [RelayCommand]
        private Task NavigateHome() => Shell.Current.GoToAsync(nameof(HomePage));

        [RelayCommand]
        private Task NavigateRecetas() => Shell.Current.GoToAsync(nameof(RecetasPage));

        [RelayCommand]
        private Task NavigateMedicacion() => Shell.Current.GoToAsync(nameof(AvisosMedicacionPage));

        [RelayCommand]
        private Task NavigateEventos() => Shell.Current.GoToAsync(nameof(EventosPage));


        public void Refresh() => RaiseAllIconChanges();

        public void Dispose()
        {
            if (Shell.Current != null)
                Shell.Current.Navigated -= OnShellNavigated;
        }
    }
}
