using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Login;
using System.Text.Json;
using OneSignalSDK.DotNet;

#if ANDROID
using RMD.Movil.Platforms.Android.Utils; // NotificationPermissionHelper
#endif

#if IOS
using RMD.Movil.Platforms.iOS.Utils;     // iOSNotificationHelper
#endif

namespace RMD.Movil;

public partial class App
{
    public static IServiceProvider Services { get; private set; } = default!;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        Services = serviceProvider;

        // Nota: Este Initialize es del SDK .NET (REST). No pide permisos del SO móvil.
        OneSignal.Initialize("e4dcd415-b4cc-4497-8462-a1241f6ee9a7");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var splashPage = new SplashPage();
        var window = new Window(splashPage);

        Task.Run(async () =>
        {
            string rutaDestino = nameof(LoginPage); // Fallback

            // Paso 1: inicio visual
            await splashPage.ActualizarProgreso(0.15);

            // Paso 1.1: pedir permisos y arrancar segundo plano SIN esperar login
            await SolicitarPermisosYArrancarSegundoPlanoAsync();

            await splashPage.ActualizarProgreso(0.2);
            await Task.Delay(300);

            // Paso 2: Validar región
            string? region = Preferences.Default.Get<string?>("RegionSeleccionada", null);
            await splashPage.ActualizarProgreso(0.4);
            await Task.Delay(300);

            if (string.IsNullOrEmpty(region))
            {
                Preferences.Default.Remove("UsuarioGuardado");
                rutaDestino = nameof(RegionPage);
            }
            else
            {
                // Paso 3: Validar usuario
                string? loginJson = Preferences.Default.Get<string?>("UsuarioGuardado", null);
                var usuario = !string.IsNullOrEmpty(loginJson)
                    ? JsonSerializer.Deserialize<LoginResult>(loginJson)
                    : null;

                await splashPage.ActualizarProgreso(0.6);
                await Task.Delay(300);

                if (usuario == null || string.IsNullOrWhiteSpace(usuario.TokenPlain))
                {
                    rutaDestino = nameof(LoginPage);
                }
                else
                {
                    // Paso 4: Intentar renovar token
                    try
                    {
                        await splashPage.ActualizarProgreso(0.8);
                        var authService = Services.GetRequiredService<IAuthControllerService>();
                        var renovado = await authService.RenewTokenAsync();

                        if (renovado.Toast?.ToLower() == "success" && renovado.Data != null)
                        {
                            usuario.TokenEncrypted = renovado.Data.TokenEncrypted;
                            usuario.TokenPlain = renovado.Data.TokenPlain;
                            Preferences.Default.Set("UsuarioGuardado", JsonSerializer.Serialize(usuario));
                            rutaDestino = nameof(HomePage);
                        }
                        else
                        {
                            Preferences.Default.Remove("UsuarioGuardado");
                            rutaDestino = nameof(LoginPage);
                        }
                    }
                    catch
                    {
                        Preferences.Default.Remove("UsuarioGuardado");
                        rutaDestino = nameof(LoginPage);
                    }
                }
            }

            // Paso final: 100%
            await splashPage.ActualizarProgreso(1.0);
            await Task.Delay(300);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                var shell = new AppShell();
                window.Page = shell;

                shell.Dispatcher.Dispatch(async () =>
                {
                    await Shell.Current.GoToAsync(rutaDestino);
                });
            });
        });

        return window;
    }

    private static async Task SolicitarPermisosYArrancarSegundoPlanoAsync()
    {
        // Solo la primera vez muestra prompts “molestos”
        bool firstRun = !Preferences.Default.ContainsKey("FirstRunDone");

#if IOS
        // iOS: pedir autorización de notificaciones + registrar APNs
        await iOSNotificationHelper.EnsureAsync();
#endif

#if ANDROID
        // Android: 13+ -> pedir permiso de notificaciones para poder mostrar la notificación persistente
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await NotificationPermissionHelper.EnsureAsync();
        });

        // Arrancar/asegurar el servicio en primer plano
        var alwaysOn = Services.GetRequiredService<IAlwaysOnService>();

        if (firstRun)
        {
            // Sugerir ignorar optimizaciones de batería (abre Settings; el usuario decide)
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await alwaysOn.RequestIgnoreBatteryOptimizationsAsync();
            });
        }

        await alwaysOn.StartAsync();
#endif

        if (firstRun)
            Preferences.Default.Set("FirstRunDone", true);
    }
}
