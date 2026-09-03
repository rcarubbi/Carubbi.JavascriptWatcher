using System.Text.RegularExpressions;

namespace Carubbi.JavascriptWatcher.Policy;

/// <summary>
/// Declarative policy for confirm() (and beforeunload) dialogs driven by message and origin URLs.
/// </summary>
public sealed class ConfirmPolicy : IDialogPolicy
{
    private readonly List<ConfirmPolicyRule> _rules = [];

    /// <summary>
    /// Starts a new rule matching the confirm message.
    /// </summary>
    public ConfirmMessageMatcher WhenMessageMatches(string pattern) =>
        new(this, new Regex(pattern, RegexOptions.IgnoreCase));

    /// <summary>
    /// Starts a new rule matching the confirm source URL.
    /// </summary>
    public ConfirmMessageMatcher WhenSourceUrlMatches(string pattern) =>
        new(this, sourceUrl: new Regex(pattern, RegexOptions.IgnoreCase));

    internal void AddRule(Regex? message, Regex? sourceUrl, DialogDecision decision) =>
        _rules.Add(new ConfirmPolicyRule(message, sourceUrl, decision));

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

    private sealed record ConfirmPolicyRule(Regex? Message, Regex? SourceUrl, DialogDecision Decision)
    {
        public DialogDecision? Resolve(DialogContext context) =>
            ((Message is null || Message.IsMatch(context.Message)) &&
             (SourceUrl is null || (context.SourceUrl is not null && SourceUrl.IsMatch(context.SourceUrl))))
                ? Decision
                : null;
    }
}

/// <summary>
/// Fluent builder for a confirm matcher.
/// </summary>
public sealed class ConfirmMessageMatcher
{
    private readonly ConfirmPolicy _policy;
    private readonly Regex? _message;
    private readonly Regex? _sourceUrl;

    internal ConfirmMessageMatcher(ConfirmPolicy policy, Regex? message = null, Regex? sourceUrl = null)
    {
        _policy = policy;
        _message = message;
        _sourceUrl = sourceUrl;
    }

    /// <summary>Automatically accepts the matching confirm.</summary>
    public void AutoAccept() => _policy.AddRule(_message, _sourceUrl, DialogDecision.Accept());

    /// <summary>Automatically rejects/cancels the matching confirm.</summary>
    public void AutoReject() => _policy.AddRule(_message, _sourceUrl, DialogDecision.Reject());
}
