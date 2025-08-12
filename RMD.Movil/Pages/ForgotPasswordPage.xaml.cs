using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels;

namespace RMD.Movil.Pages;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage()
    {
        InitializeComponent();

        var auth = App.Services.GetService<IAuthControllerService>();
        if (auth is null)
            throw new Exception("No se pudo resolver IAuthControllerService");

        BindingContext = new ForgotPasswordPageModel(auth);
    }
}