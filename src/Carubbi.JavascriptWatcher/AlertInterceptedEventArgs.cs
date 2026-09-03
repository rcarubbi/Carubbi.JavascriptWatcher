namespace Carubbi.JavascriptWatcher;

/// <summary>
/// Event arguments raised when a native JavaScript alert() is intercepted.
/// </summary>
public class AlertInterceptedEventArgs(string message) : EventArgs
{
    /// <summary>
    /// The intercepted alert message.
    /// </summary>
    public string Message { get; } = message;
}
