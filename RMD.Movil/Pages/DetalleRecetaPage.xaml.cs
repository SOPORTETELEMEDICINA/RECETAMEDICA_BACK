using RMD.Movil.Core.Service.Interfaces;

namespace RMD.Movil.Pages;

[QueryProperty(nameof(IdReceta), "IdReceta")]
public partial class DetalleRecetaPage : ContentPage
{
    private readonly DetalleRecetaPageModel _vm;
    private Guid _idReceta;

    public DetalleRecetaPage()
    {
        InitializeComponent();

        var detallesService = App.Services.GetService<IDetalleRecetasControllerService>()
                              ?? throw new Exception("No se pudo resolver IDetalleRecetasControllerService");

        var alertasProgService = App.Services.GetService<IAlertasProgramadasControllerService>()
                                 ?? throw new Exception("No se pudo resolver IAlertasProgramadasControllerService");

        var alertaTomaService = App.Services.GetService<IAlertaTomaControllerService>()
                                ?? throw new Exception("No se pudo resolver IAlertaTomaControllerService");

        _vm = new DetalleRecetaPageModel(detallesService, alertasProgService, alertaTomaService);
        BindingContext = _vm;
    }

    // Recibe el parámetro desde Shell y dispara la carga
    public string? IdReceta
    {
        get => _idReceta == Guid.Empty ? null : _idReceta.ToString();
        set
        {
            if (Guid.TryParse(value, out var id))
            {
                _idReceta = id;
                MainThread.BeginInvokeOnMainThread(async () => await _vm.InitAsync(_idReceta));
            }
        }
    }
}
