using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RMD.Movil.PageModels.Controls;

public partial class BottomNavBarModel : ObservableObject
{
    private string CurrentRoute => Shell.Current?.CurrentState?.Location.ToString() ?? string.Empty;

    public BottomNavBarModel()
    {
        Shell.Current.Navigated += (_, _) =>
        {
            OnPropertyChanged(nameof(HomeIcon));
            OnPropertyChanged(nameof(RecetasIcon));
            OnPropertyChanged(nameof(MedicacionIcon));
            OnPropertyChanged(nameof(EventosIcon));
        };
    }

    public string HomeIcon => CurrentRoute.Contains("HomePage")
        ? "Images/NavBar/button_home_active.png"
        : "Images/NavBar/button_home.png";

    public string RecetasIcon => CurrentRoute.Contains("RecetasPage")
        ? "Images/NavBar/button_receta_active.png"
        : "Images/NavBar/button_receta.png";

    public string MedicacionIcon => CurrentRoute.Contains("AvisosMedicacionPage")
        ? "Images/NavBar/button_medicamentos_active.png"
        : "Images/NavBar/button_medicamentos.png";

    public string EventosIcon => CurrentRoute.Contains("EventosPage")
        ? "Images/NavBar/button_eventos_salud_active.png"
        : "Images/NavBar/button_eventos_salud.png";

    [RelayCommand]
    async Task NavigateHome()
    {
        await Shell.Current.GoToAsync("///HomePage");
    }

    [RelayCommand]
    async Task NavigateRecetas()
    {
        await Shell.Current.GoToAsync("RecetasPage");
    }

    [RelayCommand]
    async Task NavigateMedicacion()
    {
        await Shell.Current.GoToAsync("AvisosMedicacionPage");
    }

    [RelayCommand]
    async Task NavigateEventos()
    {
        await Shell.Current.GoToAsync("///EventosPage");
    }
}