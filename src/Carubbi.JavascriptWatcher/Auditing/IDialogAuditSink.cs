using Carubbi.JavascriptWatcher.Policy;

namespace Carubbi.JavascriptWatcher.Auditing;

/// <summary>
/// Receives a record of every dialog resolution for telemetry/auditing purposes.
/// </summary>
public interface IDialogAuditSink
{
    /// <summary>
    /// Records a resolved dialog.
    /// </summary>
    void Record(DialogResolvedEventArgs resolved);
}
