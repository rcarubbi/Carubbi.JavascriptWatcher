using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Carubbi.JavascriptWatcher;

/// <summary>
/// Intercepts native JavaScript dialog and window.open calls in a WebView2 control.
/// </summary>
public class JavascriptWatcher
{
    private readonly WebView2 _webView;
    private bool _suppressAlert;
    private bool _suppressWindowOpen;

    /// <summary>
    /// Raised when a native alert() is intercepted.
    /// </summary>
    public event EventHandler<AlertInterceptedEventArgs>? AlertIntercepted;

    /// <summary>
    /// Raised when a native window.open() is intercepted.
    /// </summary>
    public event EventHandler<WindowOpenInterceptedEventArgs>? WindowOpenIntercepted;

    /// <summary>
    /// Raised when a native confirm() is intercepted.
    /// </summary>
    public event EventHandler<ConfirmInterceptedEventArgs>? ConfirmIntercepted;

    /// <summary>
    /// Creates a watcher bound to the given WebView2 control.
    /// </summary>
    /// <param name="webView">The WebView2 control to monitor.</param>
    public JavascriptWatcher(WebView2 webView)
    {
        _webView = webView ?? throw new ArgumentNullException(nameof(webView));
    }

    /// <summary>
    /// Starts monitoring the page.
    /// </summary>
    /// <param name="suppressAlert">True to suppress the default browser alert().</param>
    /// <param name="suppressWindowOpen">True to suppress the default browser window.open().</param>
    public void Start(bool suppressAlert, bool suppressWindowOpen) => Start(suppressAlert, suppressWindowOpen, false);

    /// <summary>
    /// Starts monitoring the page.
    /// </summary>
    /// <param name="suppressAlert">True to suppress the default browser alert().</param>
    /// <param name="suppressWindowOpen">True to suppress the default browser window.open().</param>
    /// <param name="attachInstantly">True to wire handlers immediately; otherwise wire them once the CoreWebView2 is initialized.</param>
    public void Start(bool suppressAlert, bool suppressWindowOpen, bool attachInstantly)
    {
        _suppressAlert = suppressAlert;
        _suppressWindowOpen = suppressWindowOpen;

        if (attachInstantly)
        {
            AttachHandlers();
        }
        else
        {
            _webView.CoreWebView2InitializationCompleted += OnCoreWebView2InitializationCompleted;
        }
    }

    /// <summary>
    /// Stops monitoring the page.
    /// </summary>
    public void Stop()
    {
        if (_webView.CoreWebView2 is null)
        {
            return;
        }

        _webView.CoreWebView2InitializationCompleted -= OnCoreWebView2InitializationCompleted;
        _webView.CoreWebView2.ScriptDialogOpening -= OnScriptDialogOpening;
        _webView.CoreWebView2.NewWindowRequested -= OnNewWindowRequested;
    }

    /// <summary>
    /// Wires the interception handlers to the CoreWebView2.
    /// </summary>
    public void AttachHandlers()
    {
        if (_webView.CoreWebView2 is null)
        {
            throw new InvalidOperationException(
                "CoreWebView2 is not initialized. Call Start() with attachInstantly=false and wait for " +
                "initialization, or initialize CoreWebView2 manually before calling AttachHandlers().");
        }

        _webView.CoreWebView2.ScriptDialogOpening += OnScriptDialogOpening;
        _webView.CoreWebView2.NewWindowRequested += OnNewWindowRequested;
    }

    private void OnCoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
    {
        _webView.CoreWebView2InitializationCompleted -= OnCoreWebView2InitializationCompleted;

        if (e.IsSuccess)
        {
            AttachHandlers();
        }
    }

    private void OnScriptDialogOpening(object? sender, CoreWebView2ScriptDialogOpeningEventArgs e)
    {
        switch (e.Kind)
        {
            case CoreWebView2ScriptDialogKind.Alert:
                AlertIntercepted?.Invoke(this, new AlertInterceptedEventArgs(e.Message));
                if (_suppressAlert)
                {
                    e.GetDeferral().Complete();
                    return;
                }
                break;

            case CoreWebView2ScriptDialogKind.Confirm:
            case CoreWebView2ScriptDialogKind.Beforeunload:
                var confirmArgs = new ConfirmInterceptedEventArgs(e.Message);
                ConfirmIntercepted?.Invoke(this, confirmArgs);
                if (confirmArgs.Result)
                {
                    e.Accept();
                }
                e.GetDeferral().Complete();
                return;
        }

        e.GetDeferral().Complete();
    }

    private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        if (Uri.TryCreate(e.Uri, UriKind.Absolute, out var uri))
        {
            WindowOpenIntercepted?.Invoke(this, new WindowOpenInterceptedEventArgs(uri));
        }

        if (_suppressWindowOpen)
        {
            e.Handled = true;
        }
    }
}
