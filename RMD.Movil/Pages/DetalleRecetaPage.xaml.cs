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

        var svcDetalles = App.Services.GetService<IDetalleRecetasControllerService>()
                          ?? throw new Exception("No se pudo resolver IDetalleRecetasControllerService");

        var svcAlertasProg = App.Services.GetService<IAlertasProgramadasControllerService>()
                             ?? throw new Exception("No se pudo resolver IAlertasProgramadasControllerService");

        _vm = new DetalleRecetaPageModel(svcDetalles, svcAlertasProg);
        BindingContext = _vm;
    }

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