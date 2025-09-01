using CoreFoundation;
using Foundation;
using RMD.Movil.Services.Pdf;
using UIKit;
using WebKit;

namespace RMD.Movil.Platforms.iOS.Services
{
    public sealed class PdfService : NSObject, IPdfService
    {
        // A4 en puntos (iOS: 72 dpi)
        private static readonly nfloat A4WidthPt = 595f;
        private static readonly nfloat A4HeightPt = 842f;  // 11.69" * 72

        public Task<string> SaveHtmlToPdfAsync(string html, string fileNameNoExt, CancellationToken ct = default)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            // Timeout/cancel enlazado (mismo 90s que Android)
            var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(90));
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);
            CancellationTokenRegistration? cancelReg = null;

            void Fail(string message, Exception? ex = null)
            {
                if (!tcs.Task.IsCompleted)
                    tcs.TrySetException(ex ?? new Exception(message));
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                WKWebView? webView = null;

                void Cleanup()
                {
                    try
                    {
                        if (webView != null)
                        {
                            webView.NavigationDelegate = null!;
                            webView.UIDelegate = null!;
                            webView.RemoveFromSuperview();
                            webView.Dispose();
                        }
                    }
                    catch { }
                    cancelReg?.Dispose();
                }

                cancelReg = linkedCts.Token.Register(() =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Cleanup();
                        Fail("Timeout/cancel al generar PDF");
                    });
                });

                try
                {
                    var config = new WKWebViewConfiguration();
                    config.SuppressesIncrementalRendering = false;
                    config.AllowsAirPlayForMediaPlayback = false;
                    config.DefaultWebpagePreferences.PreferredContentMode = WKContentMode.Recommended;

                    // Importante: ancho de página A4 para layout consistente
                    var initialFrame = new CoreGraphics.CGRect(0, 0, A4WidthPt, 1);
                    webView = new WKWebView(initialFrame, config)
                    {
                        Opaque = true,
                        BackgroundColor = UIColor.White
                    };

                    // No es necesario presentar el webView en pantalla, pero lo agregamos a una ventana temporal si se desea
                    // UIApplication.SharedApplication.KeyWindow?.AddSubview(webView);

                    var finishedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    var readyPaintTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                    webView.NavigationDelegate = new NavDelegate(
                        onFinished: () => finishedTcs.TrySetResult(true),
                        onFail: (err) =>
                        {
                            finishedTcs.TrySetException(new Exception(err));
                            readyPaintTcs.TrySetException(new Exception(err));
                        }
                    );

                    // Cargar HTML
                    webView.LoadHtmlString(html, baseUrl: null);

                    // Esperar a que termine navegación y al menos un “paint”
                    Task.WhenAll(finishedTcs.Task, WaitCommitLikeAsync(webView, linkedCts.Token)).ContinueWith(async _ =>
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            if (linkedCts.IsCancellationRequested)
                            {
                                Cleanup();
                                return;
                            }

                            try
                            {
                                // Pequeño respiro
                                await Task.Delay(300, linkedCts.Token);

                                // Calcular la altura real del contenido
                                var js = "Math.max(document.body.scrollHeight, document.documentElement.scrollHeight).toString()";
                                var heightStr = await EvalAsync(webView!, js, linkedCts.Token);
                                if (!int.TryParse(heightStr, out var contentHeightPx))
                                    contentHeightPx = (int)A4HeightPt;

                                // Ajustar frame del webView al ancho A4 y a la altura del contenido
                                // iOS usa puntos; el web contenido escala internamente
                                var contentHeightPt = (nfloat)contentHeightPx; // se comporta bien en la mayoría de casos
                                if (contentHeightPt < A4HeightPt) contentHeightPt = A4HeightPt;

                                webView!.Frame = new CoreGraphics.CGRect(0, 0, A4WidthPt, contentHeightPt);
                                webView.SetNeedsLayout();
                                webView.LayoutIfNeeded();

                                // Configurar PDF A4
                                var pdfConfig = new WKPdfConfiguration
                                {
                                    // Esta rect define el tamaño de cada página
                                    Rect = new CoreGraphics.CGRect(0, 0, A4WidthPt, A4HeightPt)
                                };

                                // Crear PDF (iOS 14+)
                                var data = await webView.CreatePdfAsync(pdfConfig);

                                // Guardar en cache
                                var pdfPath = Path.Combine(FileSystem.CacheDirectory, $"{fileNameNoExt}.pdf");
                                NSFileManager.DefaultManager.CreateFile(pdfPath, data, new NSDictionary());

                                Cleanup();
                                if (!tcs.Task.IsCompleted)
                                    tcs.TrySetResult(pdfPath);
                            }
                            catch (OperationCanceledException)
                            {
                                Cleanup();
                                Fail("Cancelado");
                            }
                            catch (Exception ex)
                            {
                                Cleanup();
                                Fail("Error creando PDF", ex);
                            }
                        });
                    });
                }
                catch (Exception ex)
                {
                    Cleanup();
                    Fail("Error inicializando WKWebView", ex);
                }
            });

            return tcs.Task;
        }

        // Espera equivalente a “primer paint visible” en Android
        private static Task<bool> WaitCommitLikeAsync(WKWebView webView, CancellationToken ct)
        {
            // No existe OnPageCommitVisible en iOS; usamos un pequeño delay + readyState
            return Task.Run(async () =>
            {
                // Esperar a readyState === 'complete'
                for (int i = 0; i < 25; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    try
                    {
                        var state = await EvalAsync(webView, "document.readyState", ct);
                        if (string.Equals(state, "complete", StringComparison.OrdinalIgnoreCase))
                            break;
                    }
                    catch { }
                    await Task.Delay(80, ct);
                }

                // Un respiro adicional para fuentes/imágenes
                await Task.Delay(120, ct);
                return true;
            }, ct);
        }

        private static Task<string> EvalAsync(WKWebView webView, string script, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            ct.Register(() => tcs.TrySetCanceled(ct));

            MainThread.BeginInvokeOnMainThread(() =>
            {
                webView.EvaluateJavaScript(script, (result, err) =>
                {
                    if (err != null)
                        tcs.TrySetException(new NSErrorException(err));
                    else
                        tcs.TrySetResult((result?.ToString() ?? string.Empty).Trim('"'));
                });
            });

            return tcs.Task;
        }

        private sealed class NavDelegate : WKNavigationDelegate
        {
            private readonly Action _onFinished;
            private readonly Action<string> _onFail;
            private bool _finishedOnce;

            public NavDelegate(Action onFinished, Action<string> onFail)
            {
                _onFinished = onFinished;
                _onFail = onFail;
            }

            public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
            {
                if (_finishedOnce) return;
                _finishedOnce = true;

                // Pequeño delay para asegurar que terminó el render (similar a Android)
                DispatchQueue.MainQueue.DispatchAfter(
                    new DispatchTime(DispatchTime.Now, 200_000_0), // 200ms
                    _onFinished
                );
            }

            public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
                => _onFail($"Error de navegación: {error.LocalizedDescription}");

            public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
                => _onFail($"Error provisional: {error.LocalizedDescription}");
        }
    }
}
