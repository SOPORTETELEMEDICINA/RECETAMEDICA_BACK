using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.Services.Pdf;

namespace RMD.Movil.Pages;

public partial class RecetasPage : ContentPage
{
    private readonly RecetasPageModel _viewModel;

    public RecetasPage()
    {
        InitializeComponent();

        var recetasService = App.Services.GetService<IRecetaControllerService>()
                             ?? throw new Exception("No se pudo resolver IRecetaControllerService");

        var detalleService = App.Services.GetService<IDetalleRecetasControllerService>()
                             ?? throw new Exception("No se pudo resolver IDetalleRecetasControllerService");

        var pdfService = App.Services.GetService<IPdfService>()
                         ?? throw new Exception("No se pudo resolver IPdfService");

        _viewModel = new RecetasPageModel(recetasService, detalleService, pdfService);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        MainThread.BeginInvokeOnMainThread(async () =>
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