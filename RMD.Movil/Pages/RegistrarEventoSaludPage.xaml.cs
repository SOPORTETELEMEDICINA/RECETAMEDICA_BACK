using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels;

namespace RMD.Movil.Pages;

public partial class RegistrarEventoSaludPage : ContentPage
{
    private readonly RegistrarEventoSaludPageModel _viewModel;
    public RegistrarEventoSaludPage()
    {
        InitializeComponent();
        var cateventosService = App.Services.GetService<ICatEventosSaludControllerService>();
        if (cateventosService == null)
            throw new Exception("No se pudo resolver IEventosSaludControllerService");

        var eventosService = App.Services.GetService<IEventosSaludControllerService>();
        if (eventosService == null)
            throw new Exception("No se pudo resolver IEventosSaludControllerService");

        _viewModel = new RegistrarEventoSaludPageModel(cateventosService, eventosService);
        BindingContext = _viewModel;
    }

}

