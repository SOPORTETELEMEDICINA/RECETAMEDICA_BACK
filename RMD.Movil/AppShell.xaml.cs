namespace RMD.Movil;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(RegionPage), typeof(RegionPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(EventosPage), typeof(EventosPage));
        Routing.RegisterRoute(nameof(RecetasPage), typeof(RecetasPage));
        Routing.RegisterRoute(nameof(RecetaQRPage), typeof(RecetaQRPage));
        Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
        Routing.RegisterRoute(nameof(EditProfilePage), typeof(EditProfilePage));
        Routing.RegisterRoute(nameof(RegistrarEventoSaludPage), typeof(RegistrarEventoSaludPage));
        Routing.RegisterRoute(nameof(AvisosMedicacionPage), typeof(AvisosMedicacionPage));
        Routing.RegisterRoute(nameof(DetalleRecetaPage), typeof(DetalleRecetaPage));
        Routing.RegisterRoute(nameof(ActivarAlertaTomaPage), typeof(ActivarAlertaTomaPage));
        Routing.RegisterRoute(nameof(ActivarAlertaTomaManualPage), typeof(ActivarAlertaTomaManualPage));
    }
}