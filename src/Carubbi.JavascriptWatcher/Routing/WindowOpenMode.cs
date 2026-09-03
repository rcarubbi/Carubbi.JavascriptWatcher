namespace Carubbi.JavascriptWatcher.Routing;

/// <summary>
/// Destination choices for a window.open() request.
/// </summary>
public enum WindowOpenMode
{
    /// <summary>Default browser behavior (external popup window).</summary>
    External,

    /// <summary>Route the content into an embedded WebView2 target.</summary>
    Route,

    /// <summary>Swallow the request; no window is opened.</summary>
    Suppress
}
