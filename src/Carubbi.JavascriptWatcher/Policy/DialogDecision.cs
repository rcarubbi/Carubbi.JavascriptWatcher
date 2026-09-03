namespace Carubbi.JavascriptWatcher.Policy;

/// <summary>
/// The action to take for an intercepted dialog.
/// </summary>
public enum DialogDecisionKind
{
    /// <summary>Automatically accept (confirm) or expose the alert without prompting.</summary>
    AutoAccept,

    /// <summary>Automatically reject/cancel (confirm).</summary>
    AutoReject,

    /// <summary>Do not show the dialog; swallow it (alert/window.open).</summary>
    Suppress,

    /// <summary>Let the default browser behavior run (pass through).</summary>
    Passthrough,

    /// <summary>Route the request to an alternate target (window.open).</summary>
    Route
}

/// <summary>
/// The result of evaluating a dialog against a policy.
/// </summary>
public sealed record DialogDecision(DialogDecisionKind Kind, object? Payload = null)
{
    /// <summary>
    /// Convenience factory for a decision that accepts a confirm or shows an alert.
    /// </summary>
    public static DialogDecision Accept() => new(DialogDecisionKind.AutoAccept);

    /// <summary>
    /// Convenience factory for a decision that rejects/cancels a confirm.
    /// </summary>
    public static DialogDecision Reject() => new(DialogDecisionKind.AutoReject);

    /// <summary>
    /// Convenience factory for a decision that suppresses the dialog.
    /// </summary>
    public static DialogDecision SuppressDecision() => new(DialogDecisionKind.Suppress);

    /// <summary>
    /// Convenience factory for a decision that lets the default behavior run.
    /// </summary>
    public static DialogDecision PassThrough() => new(DialogDecisionKind.Passthrough);
}
