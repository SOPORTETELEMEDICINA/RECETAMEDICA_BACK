using RMD.Movil.Core.Service.Interfaces;

namespace RMD.Movil.Pages;

[QueryProperty(nameof(IdReceta), "IdReceta")]
[QueryProperty(nameof(IdDetalleReceta), "IdDetalleReceta")]
[QueryProperty(nameof(MedicamentoId), "MedicamentoId")]
[QueryProperty(nameof(MedicamentoType), "MedicamentoType")]
public partial class ActivarAlertaTomaPage : ContentPage
{
    private readonly ActivarAlertaTomaPageModel _vm;

    public ActivarAlertaTomaPage()
    {
        InitializeComponent();

        var svc = App.Services.GetService<IAlertaTomaControllerService>()
                  ?? throw new Exception("No se pudo resolver IAlertaTomaControllerService");

        _vm = new ActivarAlertaTomaPageModel(svc);
        BindingContext = _vm;
    }

    public string? IdReceta
    {
        set { _vm.IdReceta = Guid.TryParse(value, out var g) ? g : Guid.Empty; }
    }

    public string? IdDetalleReceta
    {
        set { _vm.IdDetalleReceta = Guid.TryParse(value, out var g) ? g : Guid.Empty; }
    }

    public int MedicamentoId
    {
        set { _vm.MedicamentoId = value; }
    }

    public string? MedicamentoType
    {
        set { _vm.MedicamentoType = value ?? string.Empty; }
    }
}