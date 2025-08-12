// =================== ALIAS ANDROID ===================
using System.IO;
using RMD.Movil.Services.Pdf;
using AUtil = global::Android.Util;
using AView = global::Android.Views;
using AWeb = global::Android.Webkit;
using AWidget = global::Android.Widget;
using APdf = global::Android.Graphics.Pdf;

namespace RMD.Movil.Platforms.Android.Services
{
    public class PdfService : Java.Lang.Object, IPdfService
    {
        // A4 @ 300 dpi
        const int Dpi = 300;
        const float A4InchesW = 8.27f;
        const float A4InchesH = 11.69f;
        static readonly int PageW = (int)(A4InchesW * Dpi);   // 2481 px
        static readonly int PageH = (int)(A4InchesH * Dpi);   // 3507 px

        public Task<string> SaveHtmlToPdfAsync(string html, string fileNameNoExt, CancellationToken ct = default)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(90));
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

            IDisposable? cancelReg = linkedCts.Token.Register(() =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (!tcs.Task.IsCompleted)
                        tcs.TrySetException(new TaskCanceledException("Timeout/cancel al generar PDF"));
                });
            });

            MainThread.BeginInvokeOnMainThread(() =>
            {
                AWeb.WebView? webView = null;
                AView.ViewGroup? container = null;

                void CleanupOnMainThread()
                {
                    try
                    {
                        if (container != null && webView != null)
                            container.RemoveView(webView);
                    }
                    catch { /* ignore */ }

                    try { webView?.Destroy(); } catch { /* ignore */ }
                    cancelReg?.Dispose();
                }

                void Fail(string msg, Exception? ex = null)
                {
                    AUtil.Log.Debug("PdfService", "FAIL: " + msg + (ex != null ? " :: " + ex : ""));
                    CleanupOnMainThread();
                    if (ex != null) tcs.TrySetException(ex);
                    else tcs.TrySetException(new Exception(msg));
                }

                try
                {
                    var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity
                                   ?? throw new InvalidOperationException("No hay Activity actual.");

                    webView = new AWeb.WebView(activity);

                    // ★ claves para offscreen draw
                    AWeb.WebView.EnableSlowWholeDocumentDraw();          // ★
                    webView.Settings.OffscreenPreRaster = true;          // ★
                    webView.SetLayerType(AView.LayerType.Software, null);// ★
                    webView.SetBackgroundColor(global::Android.Graphics.Color.White);

                    webView.Settings.JavaScriptEnabled = true;
                    webView.Settings.DomStorageEnabled = true;
                    webView.Settings.LoadWithOverviewMode = true;
                    webView.Settings.UseWideViewPort = true;
                    webView.Settings.MixedContentMode = AWeb.MixedContentHandling.AlwaysAllow;

                    // contenedor "invisible", pero le damos ancho de página para evitar rarezas
                    container = new AWidget.FrameLayout(activity)
                    {
                        LayoutParameters = new AView.ViewGroup.LayoutParams(PageW, 1) // ★ ancho = PageW
                    };
                    activity.AddContentView(container, container.LayoutParameters);
                    container.AddView(webView);

                    // ----- sincronización de carga -----
                    var finishedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    var visibleTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

                    webView.SetWebViewClient(new ReadyWebViewClient(
                        onFinished: () => finishedTcs.TrySetResult(true),
                        onError: m => { finishedTcs.TrySetException(new Exception(m)); visibleTcs.TrySetException(new Exception(m)); },
                        onCommitVisible: () => visibleTcs.TrySetResult(true)            // ★ primer paint visible
                    ));

                    webView.SetWebChromeClient(new ProgressChromeClient(p =>
                    {
                        if (p >= 100) visibleTcs.TrySetResult(true);                   // ★ alternativa a commit visible
                    }));

                    AUtil.Log.Debug("PdfService", $"Cargando HTML ({html.Length} chars)");
                    webView.LoadDataWithBaseURL("about:blank", html, "text/html", "utf-8", null);

                    Task.WhenAll(finishedTcs.Task, visibleTcs.Task).ContinueWith(async _ =>
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            if (linkedCts.IsCancellationRequested) { Fail("Cancelado"); return; }

                            // pequeño respiro para imágenes / fuentes
                            await Task.Delay(300);

                            try
                            {
                                var wSpec = AView.View.MeasureSpec.MakeMeasureSpec(PageW, AView.MeasureSpecMode.Exactly);
                                var hSpec = AView.View.MeasureSpec.MakeMeasureSpec(0, AView.MeasureSpecMode.Unspecified);
                                webView!.Measure(wSpec, hSpec);

                                var contentHeight = webView.MeasuredHeight;
                                if (contentHeight <= 0)
                                {
                                    var jsH = await EvalAsync(webView, "Math.max(document.body.scrollHeight, document.documentElement.scrollHeight).toString()", linkedCts.Token);
                                    if (int.TryParse(jsH, out var cssH))
                                        contentHeight = cssH;
                                }

                                webView.Layout(0, 0, PageW, contentHeight);

                                await Task.Delay(200);        // ★ deja que prerasterice
                                webView.Invalidate();         // ★

                                int totalPages = Math.Max(1, (int)Math.Ceiling(contentHeight / (double)PageH));
                                AUtil.Log.Debug("PdfService", $"contentHeight={contentHeight} px, pages={totalPages}");

                                var pdfPath = Path.Combine(FileSystem.CacheDirectory, $"{fileNameNoExt}.pdf");
                                using var pdf = new APdf.PdfDocument();

                                for (int i = 0; i < totalPages; i++)
                                {
                                    if (linkedCts.IsCancellationRequested) { Fail("Cancelado"); return; }

                                    var info = new APdf.PdfDocument.PageInfo.Builder(PageW, PageH, i + 1).Create();
                                    using var page = pdf.StartPage(info);

                                    var canvas = page.Canvas;
                                    canvas.DrawColor(global::Android.Graphics.Color.White); // ★ fondo blanco
                                    canvas.Save();
                                    canvas.Translate(0, -i * PageH);
                                    webView.Draw(canvas); // ★ en main thread y en capa software
                                    canvas.Restore();

                                    pdf.FinishPage(page);
                                }

                                using (var fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
                                {
                                    pdf.WriteTo(fs);
                                    fs.Flush();
                                }

                                CleanupOnMainThread();
                                tcs.TrySetResult(pdfPath);
                            }
                            catch (Exception ex)
                            {
                                CleanupOnMainThread();
                                tcs.TrySetException(ex);
                            }
                        });
                    });
                }
                catch (Exception ex)
                {
                    Fail("Error inicializando WebView", ex);
                }
            });

            return tcs.Task;
        }

        // --- WebViewClient con commit visible y manejo de crash ---
        private sealed class ReadyWebViewClient : AWeb.WebViewClient
        {
            private readonly Action _onFinished;
            private readonly Action<string> _onError;
            private readonly Action _onCommitVisible;
            private bool _finishedFired;

            public ReadyWebViewClient(Action onFinished, Action<string> onError, Action onCommitVisible)
            {
                _onFinished = onFinished;
                _onError = onError;
                _onCommitVisible = onCommitVisible;
            }

            public override void OnPageFinished(AWeb.WebView? view, string? url)
            {
                base.OnPageFinished(view, url);
                if (_finishedFired) return;
                _finishedFired = true;
                _onFinished();
            }

            // ★ disponible desde API 23; en tu proyecto minSdk>=29
            public override void OnPageCommitVisible(AWeb.WebView? view, string? url)
            {
                base.OnPageCommitVisible(view, url);
                _onCommitVisible();
            }

            public override void OnReceivedError(AWeb.WebView? view, AWeb.IWebResourceRequest? req, AWeb.WebResourceError? err)
                => _onError($"Error cargando HTML: {err?.Description}");

            public override void OnReceivedHttpError(AWeb.WebView? view, AWeb.IWebResourceRequest? req, AWeb.WebResourceResponse? res)
                => _onError($"HTTP {res?.StatusCode} al cargar recurso");

            public override bool OnRenderProcessGone(AWeb.WebView? view, AWeb.RenderProcessGoneDetail? detail)
            {
                _onError("WebView renderer crash (OnRenderProcessGone)");
                return true; // ya informado
            }
        }

        private sealed class ProgressChromeClient : AWeb.WebChromeClient
        {
            private readonly Action<int> _onProgress;
            public ProgressChromeClient(Action<int> onProgress) => _onProgress = onProgress;
            public override void OnProgressChanged(AWeb.WebView? view, int newProgress) => _onProgress(newProgress);
        }

        // --- Helper: await de EvaluateJavascript ---
        private static Task<string> EvalAsync(AWeb.WebView webView, string script, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            ct.Register(() => tcs.TrySetCanceled(ct));
            webView.EvaluateJavascript(script, new JsValueCb(s => tcs.TrySetResult((s ?? "").Trim('"'))));
            return tcs.Task;
        }

        private sealed class JsValueCb : Java.Lang.Object, AWeb.IValueCallback
        {
            private readonly Action<string?> _cb;
            public JsValueCb(Action<string?> cb) => _cb = cb;
            public void OnReceiveValue(Java.Lang.Object? value) => _cb(value?.ToString());
        }
    }
}
