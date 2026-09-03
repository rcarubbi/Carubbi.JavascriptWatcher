using Carubbi.JavascriptWatcher.Policy;
using Microsoft.Web.WebView2.WinForms;

namespace Carubbi.JavascriptWatcher;

/// <summary>
/// Manages a set of <see cref="JavascriptWatcher"/> instances under a central policy and optional audit sink.
/// </summary>
public sealed class JavascriptWatcherManager
{
    private readonly List<(WebView2 WebView, JavascriptWatcher Watcher)> _watchers = [];
    private readonly IDialogPolicy? _centralAlertPolicy;
    private readonly IDialogPolicy? _centralConfirmPolicy;
    private readonly IDialogPolicy? _centralWindowOpenPolicy;

    /// <summary>
    /// Creates a manager that applies the given central policies to every registered watcher.
    /// </summary>
    public JavascriptWatcherManager(
        IDialogPolicy? centralAlertPolicy = null,
        IDialogPolicy? centralConfirmPolicy = null,
        IDialogPolicy? centralWindowOpenPolicy = null)
    {
        _centralAlertPolicy = centralAlertPolicy;
        _centralConfirmPolicy = centralConfirmPolicy;
        _centralWindowOpenPolicy = centralWindowOpenPolicy;
    }

    /// <summary>
    /// Registers a WebView2 with the central policies and starts monitoring it.
    /// </summary>
    public JavascriptWatcher Add(WebView2 webView)
    {
        var watcher = new JavascriptWatcher(
            webView,
            _centralAlertPolicy,
            _centralConfirmPolicy,
            _centralWindowOpenPolicy);
        _watchers.Add((webView, watcher));
        watcher.Start();
        return watcher;
    }

    /// <summary>
    /// Registers a WebView2 with per-watcher override policies and starts monitoring it.
    /// </summary>
    public JavascriptWatcher Add(WebView2 webView, IDialogPolicy alertPolicy, IDialogPolicy confirmPolicy, IDialogPolicy windowOpenPolicy)
    {
        var watcher = new JavascriptWatcher(webView, alertPolicy, confirmPolicy, windowOpenPolicy);
        _watchers.Add((webView, watcher));
        watcher.Start();
        return watcher;
    }

    /// <summary>
    /// Removes and stops a registered watcher.
    /// </summary>
    public void Remove(WebView2 webView)
    {
        var entry = _watchers.FirstOrDefault(w => ReferenceEquals(w.WebView, webView));
        if (entry == default)
        {
            return;
        }

        entry.Watcher.Stop();
        _watchers.Remove(entry);
    }

    /// <summary>
    /// Stops all registered watchers.
    /// </summary>
    public void StopAll()
    {
        foreach (var (_, watcher) in _watchers)
        {
            watcher.Stop();
        }

        _watchers.Clear();
    }
}
