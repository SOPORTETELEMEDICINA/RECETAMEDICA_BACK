//using Android.App;
//using Android.Content.PM;
//using Android.OS;
//using Android.Views;
//using Android.Graphics;
//using AWindow = Android.Views.Window;
//using Color = Android.Graphics.Color;

//namespace RMD.Movil.Platforms.Android;

//[Activity(
//    Label = "RMD.Movil",
//    Theme = "@style/Maui.SplashTheme",
//    MainLauncher = true,
//    LaunchMode = LaunchMode.SingleTop,
//    ConfigurationChanges = ConfigChanges.ScreenSize |
//                           ConfigChanges.Orientation |
//                           ConfigChanges.UiMode |
//                           ConfigChanges.ScreenLayout |
//                           ConfigChanges.SmallestScreenSize |
//                           ConfigChanges.Density,
//    ScreenOrientation = ScreenOrientation.Portrait
//)]
//public class MainActivity : MauiAppCompatActivity
//{
//    protected override void OnCreate(Bundle? savedInstanceState)
//    {
//        base.OnCreate(savedInstanceState);

//        Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
//        Window.SetStatusBarColor(Color.Transparent);

//        Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
//            SystemUiFlags.LayoutStable |
//            SystemUiFlags.LayoutFullscreen |
//            SystemUiFlags.Fullscreen |
//            SystemUiFlags.HideNavigation |
//            SystemUiFlags.ImmersiveSticky
//        );

//#if ANDROID
//        AWindow? window = Window;

//#pragma warning disable CA1416
//        window?.DecorView.Post(() =>
//        {
//            try
//            {
//                var controller = window.InsetsController;
//                controller?.Hide(WindowInsets.Type.NavigationBars() | WindowInsets.Type.StatusBars());
//                if (controller != null)
//                    controller.SystemBarsBehavior = (int)WindowInsetsControllerBehavior.ShowTransientBarsBySwipe;

//                // Forzar siempre fullscreen sin padding
//                window.SetDecorFitsSystemWindows(false);
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine("Error ocultando barras: " + ex.Message);
//            }
//        });
//#pragma warning restore CA1416
//#endif
//    }

//    public override void OnBackPressed()
//    {
//        // Minimiza la app en lugar de cerrarla
//        MoveTaskToBack(true);
//    }
//}



//using Android.App;
//using Android.Content.PM;
//using Android.OS;
//using Android.Views;
//using AWindow = Android.Views.Window;

//namespace RMD.Movil.Platforms.Android;

//[Activity(
//    Label = "RMD.Movil",
//    Theme = "@style/Maui.SplashTheme",
//    MainLauncher = true,
//    LaunchMode = LaunchMode.SingleTop,
//    ConfigurationChanges = ConfigChanges.ScreenSize |
//                           ConfigChanges.Orientation |
//                           ConfigChanges.UiMode |
//                           ConfigChanges.ScreenLayout |
//                           ConfigChanges.Density,
//    ScreenOrientation = ScreenOrientation.Portrait // ← BLOQUEO DE ORIENTACIÓN
//)]
//public class MainActivity : MauiAppCompatActivity
//{
//    protected override void OnCreate(Bundle? savedInstanceState)
//    {
//        base.OnCreate(savedInstanceState);

//#if ANDROID
//        if (OperatingSystem.IsAndroidVersionAtLeast(30))
//        {
//            AWindow? window = Window;

//#pragma warning disable CA1416
//            window?.DecorView.Post(() =>
//            {
//                try
//                {
//                    var controller = window.InsetsController;
//                    controller?.Hide(WindowInsets.Type.NavigationBars() | WindowInsets.Type.StatusBars());
//                    if (controller != null)
//                        controller.SystemBarsBehavior = (int)WindowInsetsControllerBehavior.ShowTransientBarsBySwipe;

//                    if (Build.VERSION.SdkInt >= BuildVersionCodes.R && Build.VERSION.SdkInt < BuildVersionCodes.VanillaIceCream)
//                    {
//                        window.SetDecorFitsSystemWindows(false);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    System.Diagnostics.Debug.WriteLine("Error ocultando barras: " + ex.Message);
//                }
//            });
//#pragma warning restore CA1416
//        }
//#endif
//    }
//}



//using Android.App;
//using Android.Content.PM;
//using Android.OS;
//using Android.Views;
//using Color = Android.Graphics.Color;

//namespace RMD.Movil.Platforms.Android;

//[Activity(
//    Label = "RMD.Movil",
//    Theme = "@style/Maui.SplashTheme",
//    MainLauncher = true,
//    LaunchMode = LaunchMode.SingleTop,
//    ConfigurationChanges = ConfigChanges.ScreenSize |
//                           ConfigChanges.Orientation |
//                           ConfigChanges.UiMode |
//                           ConfigChanges.ScreenLayout |
//                           ConfigChanges.SmallestScreenSize |
//                           ConfigChanges.Density,
//    ScreenOrientation = ScreenOrientation.Portrait
//)]
//public class MainActivity : MauiAppCompatActivity
//{
//    protected override void OnCreate(Bundle? savedInstanceState)
//    {
//        base.OnCreate(savedInstanceState);

//        // Quita la barra de estado
//        Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
//        Window.SetStatusBarColor(Color.Transparent);
//        Window.SetNavigationBarColor(Color.Transparent);

//        Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
//            SystemUiFlags.LayoutStable |
//            SystemUiFlags.LayoutFullscreen |
//            SystemUiFlags.Fullscreen |
//            SystemUiFlags.HideNavigation |
//            SystemUiFlags.ImmersiveSticky
//        );

//        // Aplicar esto siempre, no solo en Android 30+
//        Window.SetDecorFitsSystemWindows(false);

//        // Adicional: para Android 11+ (SDK 30) se usa WindowInsetsController
//#if ANDROID
//        if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
//        {
//#pragma warning disable CA1416
//            Window?.DecorView?.Post(() =>
//            {
//                try
//                {
//                    var controller = Window.InsetsController;
//                    controller?.Hide(WindowInsets.Type.NavigationBars() | WindowInsets.Type.StatusBars());
//                    if (controller != null)
//                    {
//                        controller.SystemBarsBehavior = (int)WindowInsetsControllerBehavior.ShowTransientBarsBySwipe;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    System.Diagnostics.Debug.WriteLine("Error ocultando barras: " + ex.Message);
//                }
//            });
//#pragma warning restore CA1416
//        }
//#endif
//    }

//    public override void OnBackPressed()
//    {
//        // Minimiza la app (no la cierra)
//        MoveTaskToBack(true);
//    }
//}
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace RMD.MoVil
{
    [Activity(
        Theme = "@style/Maui.MainTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize |
                               ConfigChanges.Orientation |
                               ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout |
                               ConfigChanges.SmallestScreenSize |
                               ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Esto quita el color de la status bar y fuerza fullscreen
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            Window.SetStatusBarColor(Android.Graphics.Color.Transparent);

            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                SystemUiFlags.LayoutStable |
                SystemUiFlags.LayoutFullscreen |
                SystemUiFlags.Fullscreen |
                SystemUiFlags.HideNavigation |
                SystemUiFlags.ImmersiveSticky);
        }
    }
}
