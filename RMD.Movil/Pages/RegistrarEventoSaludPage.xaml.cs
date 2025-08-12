using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels;

namespace RMD.Movil.Pages;

public partial class RegistrarEventoSaludPage : ContentPage
{
    private readonly RegistrarEventoSaludPageModel _viewModel;

    public RegistrarEventoSaludPage()
    {
        InitializeComponent();

        var cateventosService = App.Services.GetService<ICatEventosSaludControllerService>()
                                ?? throw new Exception("No se pudo resolver ICatEventosSaludControllerService");

        var eventosService = App.Services.GetService<IEventosSaludControllerService>()
                             ?? throw new Exception("No se pudo resolver IEventosSaludControllerService");

        var detalleService = App.Services.GetService<IDetalleRecetasControllerService>()
                             ?? throw new Exception("No se pudo resolver IDetalleRecetasControllerService");

        _viewModel = new RegistrarEventoSaludPageModel(cateventosService, eventosService, detalleService);
        BindingContext = _viewModel;
    }
}