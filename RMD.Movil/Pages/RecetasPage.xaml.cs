using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.Services.Pdf;

namespace RMD.Movil.Pages;

public partial class RecetasPage : ContentPage
{
    private readonly RecetasPageModel _viewModel;

    public RecetasPage()
    {
        InitializeComponent();

        var recetasService = App.Services.GetService<IRecetaControllerService>();
        if (recetasService == null)
            throw new Exception("No se pudo resolver IRecetaControllerService");

        var pdfService = App.Services.GetService<IPdfService>();
        if (pdfService == null)
            throw new Exception("No se pudo resolver IRecetaControllerService");

        _viewModel = new RecetasPageModel(recetasService, pdfService);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        MainThread.BeginInvokeOnMainThread(async void () =>
        {
            try
            {
                await _viewModel.InitAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar recetas: {ex.Message}", "OK");
            }
        });
    }
}