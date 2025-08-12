using CoreFoundation;
using Foundation;
using RMD.Movil.Services.Pdf;
using UIKit;
using WebKit;

namespace RMD.Movil.Platforms.iOS.Services
{
    public class PdfService : NSObject, IPdfService
    {
        public Task<string> SaveHtmlToPdfAsync(string html, string fileNameNoExt, CancellationToken ct = default)
        {
            var tcs = new TaskCompletionSource<string>();

            var webView = new WKWebView(UIScreen.MainScreen.Bounds, new WKWebViewConfiguration());
            webView.NavigationDelegate = new NavDelegate(async void () =>
            {
                try
                {
                    var config = new WKPdfConfiguration();
                    var data = await webView.CreatePdfAsync(config);

                    var pdfPath = Path.Combine(FileSystem.CacheDirectory, $"{fileNameNoExt}.pdf");
                    NSFileManager.DefaultManager.CreateFile(pdfPath, data, new NSDictionary());
                    tcs.TrySetResult(pdfPath);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
                finally
                {
                    webView.NavigationDelegate = null!;
                    webView.Dispose();
                }
            });

            webView.LoadHtmlString(html, baseUrl: null!);
            return tcs.Task;
        }

        private sealed class NavDelegate : WKNavigationDelegate
        {
            private readonly Action _onFinished;
            public NavDelegate(Action onFinished) => _onFinished = onFinished;

            public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
            {
                // pequeño delay para asegurar que terminó el render
                DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, 200_000_0), _onFinished);
            }
        }
    }
}
