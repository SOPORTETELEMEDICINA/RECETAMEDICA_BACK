using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using RMD.Movil.Core.Service.Http;
using RMD.Movil.Core.Service.Implementations;
using RMD.Movil.Core.Service.Interfaces;
using RMD.Movil.PageModels.Controls;
using RMD.Movil.Pages.Controls;
using RMD.Movil.Services.Http;
using RMD.Movil.Utilities;
using Syncfusion.Maui.Toolkit.Hosting;

namespace RMD.Movil;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
       // Preferences.Default.Clear();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                //fonts.AddFont("fa-solid-900.ttf", "FASolid");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                fonts.AddFont("Font Awesome 7 Free-Solid-900.otf", "FASolid");

            });

        // Vistas
        builder.Services.AddTransient<SplashPage>();
        builder.Services.AddTransient<RegionPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<EventosPage>(); 
        builder.Services.AddTransient<RegistrarEventoSaludPage>();

        // ViewModels
        builder.Services.AddTransient<LoginPageModel>();
        builder.Services.AddTransient<BottomNavBarModel>();
        builder.Services.AddTransient<UserInfoComponent>();
        builder.Services.AddTransient<EventosPageModel>();
        builder.Services.AddTransient<RegistrarEventoSaludPageModel>();
        builder.Services.AddTransient<HeaderPage>();

        builder.Services.AddTransient<HomePageModel>();
        // Handlers
        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.Services.AddTransient<RenewableAuthHandler>();

        // Región actual
        var region = Preferences.Default.Get<string>("RegionSeleccionada", "MX");

        // Helper para construir el HttpClient con todos los handlers
        HttpClient BuildHttpClient(IServiceProvider sp)
        {
            var baseHandler = new HttpClientHandler();

#if DEBUG
            baseHandler.ServerCertificateCustomValidationCallback = (_, cert, _, errors) => cert != null && cert.Subject.Contains("CN=*.recetamedica.digital")
                                                                                                      || errors == System.Net.Security.SslPolicyErrors.None;
#endif
            var authHandler = sp.GetRequiredService<AuthHeaderHandler>();
            authHandler.InnerHandler = baseHandler;

            var renewHandler = sp.GetRequiredService<RenewableAuthHandler>();
            renewHandler.InnerHandler = authHandler;

            var decryptHandler = new DecryptingHandler
            {
                InnerHandler = renewHandler
            };

            return new HttpClient(decryptHandler)
            {
                BaseAddress = ApiConfigurationHelper.GetBaseAddressForRegion(region)
            };
        }

        // Helper genérico para registrar servicios
        void BuildControllerService<TService, TImpl>() where TService : class where TImpl : class, TService
        {
            builder.Services.AddSingleton<TService>(sp => Activator.CreateInstance(typeof(TImpl), BuildHttpClient(sp)) as TService
                ?? throw new InvalidOperationException($"No se pudo instanciar {typeof(TImpl).Name}"));
        }

        // Registrar todos los servicios
        BuildControllerService<IAuthControllerService, AuthControllerService>();
        BuildControllerService<IEventosSaludControllerService, EventosSaludControllerService>();
        BuildControllerService<IUsuariosControllerService, UsuariosControllerService>();
        BuildControllerService<IRecetaControllerService, RecetaControllerService>();
        BuildControllerService<IDetalleRecetasControllerService, DetalleRecetasControllerService>();
        BuildControllerService<IAlertaTomaControllerService, AlertaTomaControllerService>();
        BuildControllerService<IPacienteControllerService, PacienteControllerService>();
        BuildControllerService<ICatEventosSaludControllerService, CatEventosSaludControllerService>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
