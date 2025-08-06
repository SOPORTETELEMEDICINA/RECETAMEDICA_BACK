using RMD.Movil.Core.Service.Interfaces;
using RMD.Shared.Models.Login;
using System.Text.Json;

namespace RMD.Movil;

public partial class App
{

    public static IServiceProvider Services { get; private set; } = default!;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent(); // Aquí revienta si hay errores en estilos
        Services = serviceProvider;
    }

    //protected override Window CreateWindow(IActivationState? activationState)
    //{

    //    var shell = new AppShell();
    //    var window = new Window(shell);

    //    shell.Loaded += async (_, _) =>
    //    {
    //        string? region = Preferences.Default.Get<string?>("RegionSeleccionada", null);

    //        if (string.IsNullOrEmpty(region))
    //        {
    //            Preferences.Default.Remove("UsuarioGuardado");
    //            await shell.GoToAsync("//RegionPage");
    //            return;
    //        }

    //        string? loginJson = Preferences.Default.Get<string?>("UsuarioGuardado", null);
    //        var usuario = !string.IsNullOrEmpty(loginJson)
    //            ? JsonSerializer.Deserialize<LoginResult>(loginJson)
    //            : null;

    //        if (usuario == null || string.IsNullOrWhiteSpace(usuario.TokenPlain))
    //        {
    //            await shell.GoToAsync("//LoginPage");
    //            return;
    //        }

    //        try
    //        {


    //            // Token expirado → Intentar renovar
    //            var authService = Services.GetRequiredService<IAuthControllerService>();
    //            var renovado = await authService.RenewTokenAsync();

    //            if (renovado.Toast?.ToLower() == "success" && renovado.Data != null)
    //            {
    //                usuario.TokenEncrypted = renovado.Data.TokenEncrypted;
    //                usuario.TokenPlain = renovado.Data.TokenPlain;
    //                Preferences.Default.Set("UsuarioGuardado", JsonSerializer.Serialize(usuario));
    //                await shell.GoToAsync("//HomePage");
    //                return;
    //            }
    //            else
    //            {
    //                // Renovación fallida
    //                Preferences.Default.Remove("UsuarioGuardado");
    //                await shell.GoToAsync("//LoginPage");
    //                return;
    //            }
    //        }
    //        catch
    //        {
    //            // Error al interpretar el token
    //            Preferences.Default.Remove("UsuarioGuardado");
    //            await shell.GoToAsync("//LoginPage");
    //        }
    //    };

    //    return window;
    //}
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var splashPage = new SplashPage();
        var window = new Window(splashPage);

        Task.Run(async () =>
        {
            Page paginaInicial = new SplashPage(); // Fallback (no debería llegar nunca)

            // Paso 1: Inicio visual
            await splashPage.ActualizarProgreso(0.2);
            await Task.Delay(300);

            // Paso 2: Validar región
            string? region = Preferences.Default.Get<string?>("RegionSeleccionada", null);
            await splashPage.ActualizarProgreso(0.4);
            await Task.Delay(300);

            if (string.IsNullOrEmpty(region))
            {
                Preferences.Default.Remove("UsuarioGuardado");
                paginaInicial = new RegionPage();
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
                    paginaInicial = new LoginPage();
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
                            paginaInicial = new HomePage();
                        }
                        else
                        {
                            Preferences.Default.Remove("UsuarioGuardado");
                            paginaInicial = new LoginPage();
                        }
                    }
                    catch
                    {
                        Preferences.Default.Remove("UsuarioGuardado");
                        paginaInicial = new LoginPage();
                    }
                }
            }

            // Paso final: 100%
            await splashPage.ActualizarProgreso(1.0);
            await Task.Delay(300);

            // Reemplazar con AppShell y página inicial deseada
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var shell = new AppShell();
                shell.CurrentItem = new ShellContent { Content = paginaInicial };
                window.Page = shell;
            });
        });

        return window;
    }
}
