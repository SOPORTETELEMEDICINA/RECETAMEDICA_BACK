using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using RMD.Movil.Core.Service.Http;
using RMD.Movil.Core.Service.Implementations;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.Services.Http;
using RMD.Movil.Services.Pdf;
using RMD.Movil.Utilities;
using Syncfusion.Maui.Toolkit.Hosting;

namespace RMD.Movil;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                fonts.AddFont("FontAwesome.otf", "FASolid");
                fonts.AddFont("Roboto-Regular.ttf", "Roboto");
                fonts.AddFont("Roboto-Bold.ttf", "RobotoBold");
            });

        // Vistas
        builder.Services.AddTransient<SplashPage>();

        // Handlers (pipeline de HttpClient)
        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.Services.AddTransient<RenewableAuthHandler>();
        builder.Services.AddTransient<DecryptingHandler>();

        // Región actual (si cambias la región en runtime tendrás que reconstruir el HttpClient)
        var region = Preferences.Default.Get<string>("RegionSeleccionada", "MX");
        // Al final de tu CreateMauiApp(), junto al IPdfService
#if ANDROID
        builder.Services.AddSingleton<RMD.Movil.Core.Service.Interfaces.IAlwaysOnService, RMD.Movil.Platforms.Android.Services.AlwaysOnServiceImpl>();
#elif IOS
        builder.Services.AddSingleton<RMD.Movil.Core.Service.Interfaces.IAlwaysOnService, RMD.Movil.Platforms.iOS.Services.AlwaysOnServiceImpl>();
#endif

        // HttpClient global (única instancia) con pipeline de handlers
        builder.Services.AddSingleton<HttpClient>(sp =>
        {
            var baseHandler = new HttpClientHandler();

#if DEBUG
            baseHandler.ServerCertificateCustomValidationCallback = (_, cert, _, errors) =>
                (cert != null && cert.Subject.Contains("CN=*.recetamedica.digital"))
                || errors == System.Net.Security.SslPolicyErrors.None;
#endif
            var authHandler = sp.GetRequiredService<AuthHeaderHandler>();
            authHandler.InnerHandler = baseHandler;

            var renewHandler = sp.GetRequiredService<RenewableAuthHandler>();
            renewHandler.InnerHandler = authHandler;

            var decryptHandler = sp.GetRequiredService<DecryptingHandler>();
            decryptHandler.InnerHandler = renewHandler;

            var client = new HttpClient(decryptHandler)
            {
                BaseAddress = ApiConfigurationHelper.GetBaseAddressForRegion(region)
            };

            return client;
        });

        // Helper genérico: cada ControllerService recibe SIEMPRE el HttpClient global
        void RegisterControllerService<TService, TImpl>()
            where TService : class
            where TImpl : class, TService
        {
            builder.Services.AddSingleton<TService>(sp =>
            {
                var http = sp.GetRequiredService<HttpClient>();
                return (TService)Activator.CreateInstance(typeof(TImpl), http)!;
            });
        }

        // Registrar todos los servicios (uno por controlador de tu API)
        RegisterControllerService<IAlertasProgramadasControllerService, AlertasProgramadasControllerService>();
        RegisterControllerService<IAlertaTomaControllerService, AlertaTomaControllerService>();
        RegisterControllerService<IAuthControllerService, AuthControllerService>();
        RegisterControllerService<ICatEventosSaludControllerService, CatEventosSaludControllerService>();
        RegisterControllerService<IConsultaControllerService, ConsultaControllerService>();
        RegisterControllerService<IDetalleRecetasControllerService, DetalleRecetasControllerService>();
        RegisterControllerService<IEventosSaludControllerService, EventosSaludControllerService>();
        RegisterControllerService<IPacienteControllerService, PacienteControllerService>();
        RegisterControllerService<INotificacionesControllerService, NotificacionesControllerService>();
        RegisterControllerService<IRecetaCatalogosControllerService, RecetaCatalogosControllerService>();
        RegisterControllerService<IRecetaControllerService, RecetaControllerService>();
        RegisterControllerService<IUsuariosControllerService, UsuariosControllerService>();

        // Servicio de PDF por plataforma
#if ANDROID
        builder.Services.AddSingleton<IPdfService, RMD.Movil.Platforms.Android.Services.PdfService>();
#elif IOS
        builder.Services.AddSingleton<IPdfService, RMD.Movil.Platforms.iOS.Services.PdfService>();
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
