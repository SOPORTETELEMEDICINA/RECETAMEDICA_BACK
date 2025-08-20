using RMD.Movil.Core.Service.Interfaces;

namespace RMD.Movil.Pages
{
    public partial class ActivarAlertaTomaManualPage : ContentPage
    {
        private readonly ActivarAlertaTomaManualPageModel _vm;

        public ActivarAlertaTomaManualPage()
        {
            InitializeComponent();

            var alertasService = App.Services.GetService<IAlertaTomaControllerService>()
                                 ?? throw new Exception("No se pudo resolver IAlertaTomaControllerService");

            var catalogosService = App.Services.GetService<IRecetaCatalogosControllerService>()
                                   ?? throw new Exception("No se pudo resolver IRecetaCatalogosControllerService");
            var consultaService = App.Services.GetService<IConsultaControllerService>()
                                   ?? throw new Exception("No se pudo resolver IRecetaCatalogosControllerService");

            _vm = new ActivarAlertaTomaManualPageModel(alertasService, catalogosService, consultaService);
            BindingContext = _vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MainThread.BeginInvokeOnMainThread(async () => await _vm.InitAsync());
        }
    }
}