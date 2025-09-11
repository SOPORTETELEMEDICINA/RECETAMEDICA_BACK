using RMD.Movil.Core.Service.Interfaces;

namespace RMD.Movil.Pages;

public partial class AvisosMedicacionPage : ContentPage
{
    private readonly AvisosMedicacionPageModel _vm;

    public AvisosMedicacionPage()
    {
        InitializeComponent();

        var alertasService = App.Services.GetRequiredService<IAlertasProgramadasControllerService>();
        _vm = new AvisosMedicacionPageModel(alertasService);
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.LoadCommand.CanExecute(null))
            await _vm.LoadCommand.ExecuteAsync(null);
    }

    private void CalendarioPopup_FechaSeleccionada(DateTime fecha)
    {
        
    }
}