namespace Carubbi.JavascriptWatcher.Policy;

/// <summary>
/// Evaluates an intercepted dialog and returns a decision.
/// </summary>
public interface IDialogPolicy
{
    /// <summary>
    /// Evaluates whether this policy matches the given context and returns a decision.
    /// Returns <see langword="null"/> when the policy does not apply, so the next policy (or pass-through) takes over.
    /// </summary>
    DialogDecision? Evaluate(DialogContext context);
}
