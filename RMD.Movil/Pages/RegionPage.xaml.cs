namespace RMD.Movil.Pages;

public partial class RegionPage
{
    public RegionPage()
    {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        return true; // true = no navega atrás
    }

    private async void OnConfirmarClicked(object sender, EventArgs e)
    {
        var regionSeleccionada = RegionPicker.SelectedItem as string;

        if (string.IsNullOrEmpty(regionSeleccionada))
        {
            await DisplayAlert("Aviso", "Por favor selecciona una región.", "OK");
            return;
        }

        string region = regionSeleccionada switch
        {
            "México" => "MX",
            "España" => "ES",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(region))
        {
            await DisplayAlert("Aviso", "Región no válida.", "OK");
            return;
        }

        Preferences.Default.Set("RegionSeleccionada", region);

        var stored = Preferences.Default.Get<string?>("RegionSeleccionada", null);

        if (stored != region)
        {
            await DisplayAlert("Error", "No se pudo guardar la región. Intenta de nuevo.", "OK");
        }

        
        // // ? Navegar manualmente al LoginPage porque App no volverá a evaluar
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }
}