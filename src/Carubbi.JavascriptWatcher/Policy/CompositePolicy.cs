namespace Carubbi.JavascriptWatcher.Policy;

/// <summary>
/// Aggregates multiple policies and returns the first matching decision, in registration order.
/// </summary>
public sealed class CompositePolicy : IDialogPolicy
{
    private readonly IReadOnlyList<IDialogPolicy> _policies;

    /// <summary>
    /// Creates a composite of the given policies.
    /// </summary>
    public CompositePolicy(params IDialogPolicy[] policies)
    {
        _policies = policies ?? throw new ArgumentNullException(nameof(policies));
    }

    /// <summary>
    /// Creates a composite of the given policies.
    /// </summary>
    public CompositePolicy(IEnumerable<IDialogPolicy> policies)
    {
        _policies = (policies ?? throw new ArgumentNullException(nameof(policies))).ToList();
    }

    /// <inheritdoc />
    public DialogDecision? Evaluate(DialogContext context)
    {
        foreach (var policy in _policies)
        {
            if (policy.Evaluate(context) is { } decision)
            {
                return decision;
            }
        }

        return null;
    }
}
