using RMD.Movil.Core.Service.Interfaces;

namespace RMD.Movil.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {

            InitializeComponent();

            // Obtener el servicio desde el contenedor (AuthControllerService)
            var authService = App.Services.GetService<IAuthControllerService>();

            if (authService == null)
                throw new Exception("No se pudo resolver IAuthControllerService");

            BindingContext = new HomePageModel(authService);
        }
    }

}