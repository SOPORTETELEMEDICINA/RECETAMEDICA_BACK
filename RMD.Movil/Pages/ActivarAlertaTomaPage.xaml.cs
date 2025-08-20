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

        var alertaSvc = App.Services.GetService<IAlertaTomaControllerService>()
                        ?? throw new Exception("No se pudo resolver IAlertaTomaControllerService");

        _vm = new ActivarAlertaTomaPageModel(alertaSvc);
        BindingContext = _vm;
    }

    public string? IdReceta
    {
        get => _vm.IdReceta == Guid.Empty ? null : _vm.IdReceta.ToString();
        set { if (Guid.TryParse(value, out var g)) _vm.IdReceta = g; }
    }

    public string? IdDetalleReceta
    {
        get => _vm.IdDetalleReceta == Guid.Empty ? null : _vm.IdDetalleReceta.ToString();
        set { if (Guid.TryParse(value, out var g)) _vm.IdDetalleReceta = g; }
    }

    public string? MedicamentoId
    {
        get => _vm.MedicamentoId.ToString();
        set { if (int.TryParse(value, out var id)) _vm.MedicamentoId = id; }
    }

    public string? MedicamentoType
    {
        get => _vm.MedicamentoType;
        set => _vm.MedicamentoType = value ?? string.Empty;
    }
}