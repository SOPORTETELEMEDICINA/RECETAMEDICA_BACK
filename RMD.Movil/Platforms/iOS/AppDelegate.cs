using Foundation;
using UIKit;

namespace RMD.Movil.Platforms.iOS;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        // Usa controlador personalizado que oculta la barra de estado
        var rootController = new FullscreenViewController();
        if (Window != null)
        {
            Window.RootViewController = rootController;
        }

        return base.FinishedLaunching(app, options);
    }

    // Controlador que oculta la barra de estado
    class FullscreenViewController : UIViewController
    {
        public override bool PrefersStatusBarHidden() => true;
    }
}