namespace Carubbi.JavascriptWatcher.Policy;

/// <summary>
/// Context passed to a dialog policy for evaluation.
/// </summary>
public sealed record DialogContext(string Message, string? SourceUrl);
