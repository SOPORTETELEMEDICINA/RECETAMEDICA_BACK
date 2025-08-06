namespace RMD.Movil.Pages;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent();
        // Ya no se necesita: Loaded += OnLoaded;
    }

    public async Task ActualizarProgreso(double progreso)
    {
        if (barraprogreso == null)
            return;

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            barraprogreso.ProgressTo(progreso, 250, Easing.CubicInOut);
        });

        await Task.Delay(100);
    }
}