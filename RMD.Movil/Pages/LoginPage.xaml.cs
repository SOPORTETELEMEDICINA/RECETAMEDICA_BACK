using RMD.Movil.Core.Service.Interfaces;

namespace RMD.Movil.Pages;

public partial class LoginPage
{
    public LoginPage()
    {
        InitializeComponent();

        var service = App.Services.GetService<IAuthControllerService>()
                      ?? throw new Exception("IAuthControllerService no está disponible");
        var pacienteService = App.Services.GetService<IPacienteControllerService>()
                      ?? throw new Exception("IAuthControllerService no está disponible");
        BindingContext = new LoginPageModel(service, pacienteService);

        // Animaciones al cargar
        this.Loaded += (_, _) =>
        {
            AnimateLogo();
            AnimateCampos();
        };
    }

    private async void AnimateLogo()
    {
        await LogoImage.ScaleTo(0.9, 1);
        await LogoImage.FadeTo(0, 0);
        await LogoImage.FadeTo(1, 600, Easing.CubicInOut);
        await LogoImage.TranslateTo(0, -20, 600, Easing.CubicInOut);
        await LogoImage.ScaleTo(1, 300, Easing.CubicInOut);
    }

    private async void AnimateCampos()
    {
        var elementos = new[]
        {
            Content.FindByName<VisualElement>("UsuarioLayout"),
            Content.FindByName<VisualElement>("PasswordLayout"),
            Content.FindByName<VisualElement>("IngresarBtn"),
            Content.FindByName<VisualElement>("LblOlvido"),
            Content.FindByName<VisualElement>("LblInferior")
        };

        int delay = 200;
        foreach (var el in elementos)
        {
            if (el != null)
            {
                el.Opacity = 0;
                el.TranslationY = 30;
                await el.FadeTo(1, 400, Easing.CubicInOut);
                await el.TranslateTo(0, 0, 400, Easing.CubicInOut);
                await Task.Delay(delay);
                delay += 100;
            }
        }
    }
}