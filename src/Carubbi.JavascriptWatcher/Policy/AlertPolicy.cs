using System.Text.RegularExpressions;

namespace Carubbi.JavascriptWatcher.Policy;

/// <summary>
/// A matcher for a single alert() rule.
/// </summary>
public sealed class AlertPolicyRule
{
    private readonly Regex? _message;
    private readonly Regex? _sourceUrl;
    private readonly DialogDecision _decision;

    internal AlertPolicyRule(Regex? message, Regex? sourceUrl, DialogDecision decision)
    {
        _message = message;
        _sourceUrl = sourceUrl;
        _decision = decision;
    }

    internal bool Matches(DialogContext context) =>
        (_message is null || _message.IsMatch(context.Message)) &&
        (_sourceUrl is null || (context.SourceUrl is not null && _sourceUrl.IsMatch(context.SourceUrl)));

    /// <summary>
    /// Returns the resolved decision for the given context, or <see langword="null"/> when this rule does not match.
    /// </summary>
    public DialogDecision? Resolve(DialogContext context) => Matches(context) ? _decision : null;
}

/// <summary>
/// Declarative policy for alert() dialogs driven by message and origin URLs.
/// </summary>
public sealed class AlertPolicy : IDialogPolicy
{
    private readonly List<AlertPolicyRule> _rules = [];

    /// <summary>
    /// Starts a new rule matching the alert message.
    /// </summary>
    public AlertMessageMatcher WhenMessageMatches(string pattern) =>
        new(this, new Regex(pattern, RegexOptions.IgnoreCase));

    /// <summary>
    /// Starts a new rule matching the alert source URL.
    /// </summary>
    public AlertMessageMatcher WhenSourceUrlMatches(string pattern) =>
        new(this, null, new Regex(pattern, RegexOptions.IgnoreCase));

    internal void AddRule(Regex? message, Regex? sourceUrl, DialogDecision decision) =>
        _rules.Add(new AlertPolicyRule(message, sourceUrl, decision));

    /// <inheritdoc />
    public DialogDecision? Evaluate(DialogContext context)
    {
        foreach (var rule in _rules)
        {
            if (rule.Resolve(context) is { } decision)
            {
                return decision;
            }
        }

        return null;
    }
}

/// <summary>
/// Fluent builder for an alert/source matcher.
/// </summary>
public sealed class AlertMessageMatcher
{
    private readonly AlertPolicy _policy;
    private readonly Regex? _message;
    private readonly Regex? _sourceUrl;

    internal AlertMessageMatcher(AlertPolicy policy, Regex? message, Regex? sourceUrl = null)
    {
        _policy = policy;
        _message = message;
        _sourceUrl = sourceUrl;
    }

    /// <summary>Suppresses the matching alert (no default dialog).</summary>
    public void Suppress() => _policy.AddRule(_message, _sourceUrl, DialogDecision.SuppressDecision());

    /// <summary>Allows the matching alert to show normally.</summary>
    public void Show() => _policy.AddRule(_message, _sourceUrl, DialogDecision.Accept());
}
