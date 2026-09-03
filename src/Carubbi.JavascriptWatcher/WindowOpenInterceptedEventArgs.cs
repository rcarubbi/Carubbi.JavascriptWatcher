namespace Carubbi.JavascriptWatcher;

/// <summary>
/// Event arguments raised when a native JavaScript window.open() is intercepted.
/// </summary>
public class WindowOpenInterceptedEventArgs(Uri url) : EventArgs
{
    /// <summary>
    /// The URL requested to be opened in a new window.
    /// </summary>
    public Uri Url { get; } = url;
}
