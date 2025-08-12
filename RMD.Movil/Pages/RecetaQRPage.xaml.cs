using RMD.Movil.Core.Service.Interfaces;

namespace RMD.Movil.Pages;

[QueryProperty(nameof(IdReceta), "IdReceta")]
public partial class RecetaQRPage : ContentPage
{
    private readonly RecetaQRPageModel _vm;
    private Guid _idReceta;

    public RecetaQRPage()
    {
        InitializeComponent();

        var recetasService = App.Services.GetService<IRecetaControllerService>()
                             ?? throw new Exception("No se pudo resolver IRecetaControllerService");

        _vm = new RecetaQRPageModel(recetasService);
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
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await _vm.InitAsync(_idReceta);
                });
            }
        }
    }
}