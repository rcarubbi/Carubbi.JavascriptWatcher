using System.Text.RegularExpressions;
using Carubbi.JavascriptWatcher.Routing;

namespace Carubbi.JavascriptWatcher.Policy;

/// <summary>
/// Declarative policy for window.open() requests driven by URL rules.
/// </summary>
public sealed class WindowOpenPolicy : IDialogPolicy
{
    private readonly List<WindowOpenRule> _rules = [];

    /// <summary>
    /// Starts a new rule matching the target URL.
    /// </summary>
    public WindowOpenMatcher WhenUrlMatches(string pattern) =>
        new(this, new Regex(pattern, RegexOptions.IgnoreCase));

    internal void AddRule(Regex url, WindowOpenMode mode, object? payload) =>
        _rules.Add(new WindowOpenRule(url, mode, payload));

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

    private sealed record WindowOpenRule(Regex Url, WindowOpenMode Mode, object? Payload)
    {
        public DialogDecision? Resolve(DialogContext context)
        {
            if (Url.IsMatch(context.Message))
            {
                return Mode switch
                {
                    WindowOpenMode.Route => new DialogDecision(DialogDecisionKind.Route, Payload),
                    WindowOpenMode.Suppress => DialogDecision.SuppressDecision(),
                    _ => DialogDecision.PassThrough()
                };
            }

            return null;
        }
    }
}

/// <summary>
/// Fluent builder for a window.open URL matcher.
/// </summary>
public sealed class WindowOpenMatcher
{
    private readonly WindowOpenPolicy _policy;
    private readonly Regex _url;

    internal WindowOpenMatcher(WindowOpenPolicy policy, Regex url)
    {
        _policy = policy;
        _url = url;
    }

    /// <summary>Routes matching URLs into the given embedded target.</summary>
    public void RouteTo(WindowOpenTarget target) => _policy.AddRule(_url, WindowOpenMode.Route, target);

    /// <summary>Suppresses matching window.open requests.</summary>
    public void Suppress() => _policy.AddRule(_url, WindowOpenMode.Suppress, null);

    /// <summary>Allows matching URLs to open in an external popup.</summary>
    public void OpenExternally() => _policy.AddRule(_url, WindowOpenMode.External, null);
}
