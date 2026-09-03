namespace Carubbi.JavascriptWatcher;

/// <summary>
/// Event arguments raised when a native JavaScript confirm() is intercepted.
/// </summary>
public class ConfirmInterceptedEventArgs(string message) : EventArgs
{
    /// <summary>
    /// The intercepted confirm message.
    /// </summary>
    public string Message { get; } = message;

    /// <summary>
    /// The result returned to the browser (true = accept, false = cancel).
    /// </summary>
    public bool Result { get; set; }
}
