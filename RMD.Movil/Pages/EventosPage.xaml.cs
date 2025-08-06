using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels;
using Microsoft.Maui.Dispatching;

namespace RMD.Movil.Pages;

public partial class EventosPage : ContentPage
{
    private readonly EventosPageModel _viewModel;

    public EventosPage()
    {
        InitializeComponent();

        var eventosService = App.Services.GetService<IEventosSaludControllerService>();

        if (eventosService == null)
            throw new Exception("No se pudo resolver IEventosSaludControllerService");

        _viewModel = new EventosPageModel(eventosService);
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
                await DisplayAlert("Error", $"Error al cargar eventos: {ex.Message}", "OK");
            }
        });
    }
}