using Carubbi.JavascriptWatcher.Auditing;
using Carubbi.JavascriptWatcher.Policy;
using Carubbi.JavascriptWatcher.Routing;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Carubbi.JavascriptWatcher;

/// <summary>
/// Intercepts native JavaScript dialogs (alert, confirm, beforeunload) and window.open calls in a WebView2 control,
/// applying declarative policies and auditing each resolution.
/// </summary>
public class JavascriptWatcher
{
    private readonly WebView2 _webView;
    private readonly IDialogPolicy? _alertPolicy;
    private readonly IDialogPolicy? _confirmPolicy;
    private readonly IDialogPolicy? _windowOpenPolicy;
    private readonly IDialogAuditSink? _auditSink;
    private bool _suppressAlert;
    private bool _suppressWindowOpen;

    /// <summary>
    /// Raised when a native alert() is intercepted (pass-through).
    /// </summary>
    public event EventHandler<AlertInterceptedEventArgs>? AlertIntercepted;

    /// <summary>
    /// Raised when a native window.open() is intercepted (pass-through).
    /// </summary>
    public event EventHandler<WindowOpenInterceptedEventArgs>? WindowOpenIntercepted;

    /// <summary>
    /// Raised when a native confirm() is intercepted (pass-through).
    /// </summary>
    public event EventHandler<ConfirmInterceptedEventArgs>? ConfirmIntercepted;

    /// <summary>
    /// Creates a watcher bound to the given WebView2 control.
    /// </summary>
    public JavascriptWatcher(WebView2 webView)
    {
        _webView = webView ?? throw new ArgumentNullException(nameof(webView));
    }

    /// <summary>
    /// Creates a watcher bound to the given WebView2 control with declarative policies and an audit sink.
    /// </summary>
    /// <param name="webView">The WebView2 control to monitor.</param>
    /// <param name="alertPolicy">Policy for alert() dialogs, or null for pass-through.</param>
    /// <param name="confirmPolicy">Policy for confirm()/beforeunload dialogs, or null for pass-through.</param>
    /// <param name="windowOpenPolicy">Policy for window.open() calls, or null for default browser behavior.</param>
    /// <param name="auditSink">Optional sink that records every dialog resolution.</param>
    public JavascriptWatcher(
        WebView2 webView,
        IDialogPolicy? alertPolicy,
        IDialogPolicy? confirmPolicy,
        IDialogPolicy? windowOpenPolicy,
        IDialogAuditSink? auditSink = null)
    {
        _webView = webView ?? throw new ArgumentNullException(nameof(webView));
        _alertPolicy = alertPolicy;
        _confirmPolicy = confirmPolicy;
        _windowOpenPolicy = windowOpenPolicy;
        _auditSink = auditSink;
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
    /// Starts monitoring the page with the configured policies.
    /// </summary>
    /// <param name="attachInstantly">True to wire handlers immediately; otherwise wire them once the CoreWebView2 is initialized.</param>
    public void Start(bool attachInstantly = false)
    {
        _suppressAlert = false;
        _suppressWindowOpen = false;

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

    private async void OnScriptDialogOpening(object? sender, CoreWebView2ScriptDialogOpeningEventArgs e)
    {
        var sourceUrl = _webView.CoreWebView2?.Source;
        switch (e.Kind)
        {
            case CoreWebView2ScriptDialogKind.Alert:
                var alertContext = new DialogContext(e.Message, sourceUrl);
                var alertDecision = _alertPolicy?.Evaluate(alertContext);
                AlertIntercepted?.Invoke(this, new AlertInterceptedEventArgs(e.Message));

                if (alertDecision?.Kind == DialogDecisionKind.Suppress || _suppressAlert)
                {
                    e.GetDeferral().Complete();
                    Record("Alert", alertContext, alertDecision);
                    return;
                }
                break;

            case CoreWebView2ScriptDialogKind.Confirm:
            case CoreWebView2ScriptDialogKind.Beforeunload:
                var confirmContext = new DialogContext(e.Message, sourceUrl);
                var confirmDecision = _confirmPolicy?.Evaluate(confirmContext);
                var confirmArgs = new ConfirmInterceptedEventArgs(e.Message);

                var accept = confirmDecision?.Kind == DialogDecisionKind.AutoAccept
                    || (confirmDecision is null && confirmArgs.Result);

                ConfirmIntercepted?.Invoke(this, confirmArgs);

                if (accept)
                {
                    e.Accept();
                }
                e.GetDeferral().Complete();
                Record("Confirm", confirmContext, confirmDecision);
                return;
        }

        e.GetDeferral().Complete();
    }

    private async void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        var url = e.Uri;
        var context = new DialogContext(url, _webView.CoreWebView2?.Source);
        var decision = _windowOpenPolicy?.Evaluate(context);

        if (decision?.Kind == DialogDecisionKind.Suppress || _suppressWindowOpen)
        {
            e.Handled = true;
            Record("WindowOpen", context, decision);
            return;
        }

        if (decision?.Kind == DialogDecisionKind.Route && decision.Payload is WindowOpenTarget target)
        {
            var deferral = e.GetDeferral();
            try
            {
                var receiver = await target.EnsureReadyAsync();
                e.NewWindow = receiver;
                e.Handled = true;
            }
            finally
            {
                deferral.Complete();
            }

            Record("WindowOpen", context, decision);
            return;
        }

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            WindowOpenIntercepted?.Invoke(this, new WindowOpenInterceptedEventArgs(uri));
        }

        Record("WindowOpen", context, decision);
    }

    private void Record(string dialogType, DialogContext context, DialogDecision? decision) =>
        _auditSink?.Record(new DialogResolvedEventArgs(
            DialogType: dialogType,
            Message: context.Message,
            SourceUrl: context.SourceUrl,
            Decision: decision,
            Timestamp: DateTimeOffset.UtcNow));
}
