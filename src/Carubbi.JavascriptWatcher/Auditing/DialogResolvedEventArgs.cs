using Carubbi.JavascriptWatcher.Policy;

namespace Carubbi.JavascriptWatcher.Auditing;

/// <summary>
/// Captures the outcome of an intercepted dialog for telemetry and auditing.
/// </summary>
/// <param name="DialogType">The intercepted dialog kind (Alert, Confirm, WindowOpen).</param>
/// <param name="Message">The dialog message or target URL.</param>
/// <param name="SourceUrl">The page that raised the dialog, when known.</param>
/// <param name="Decision">The decision applied, when one was resolved by policy; otherwise null for pass-through.</param>
/// <param name="Timestamp">When the dialog was resolved (UTC).</param>
public sealed record DialogResolvedEventArgs(
    string DialogType,
    string Message,
    string? SourceUrl,
    DialogDecision? Decision,
    DateTimeOffset Timestamp);
