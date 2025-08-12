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

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var splashPage = new SplashPage();
        var window = new Window(splashPage);

        Task.Run(async () =>
        {
            string rutaDestino = nameof(LoginPage);// Fallback por defecto

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
                    //await shell.GoToAsync(rutaDestino);
                });
            });
        });

        return window;
    }

}
