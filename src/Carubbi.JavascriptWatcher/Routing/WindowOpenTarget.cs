using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Carubbi.JavascriptWatcher.Routing;

/// <summary>
/// Represents an alternate destination for window.open() requests, typically an embedded WebView2.
/// </summary>
public sealed class WindowOpenTarget
{
    private readonly WebView2 _receiver;

    /// <summary>
    /// Creates a target backed by the given WebView2 receiver.
    /// </summary>
    public WindowOpenTarget(WebView2 receiver)
    {
        _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
    }

    /// <summary>
    /// Ensures the receiver's CoreWebView2 is initialized, returning it ready for window.open routing.
    /// </summary>
    public async Task<CoreWebView2> EnsureReadyAsync()
    {
        if (_receiver.CoreWebView2 is null)
        {
            await _receiver.EnsureCoreWebView2Async();
        }

        return _receiver.CoreWebView2 ?? throw new InvalidOperationException("Failed to initialize the receiver's CoreWebView2.");
    }
}
