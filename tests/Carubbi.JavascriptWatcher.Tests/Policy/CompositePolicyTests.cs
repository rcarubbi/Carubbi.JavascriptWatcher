using Carubbi.JavascriptWatcher.Policy;

namespace Carubbi.JavascriptWatcher.Tests.Policy;

public class CompositePolicyTests
{
    [Test]
    public async Task Evaluate_When_Empty_Then_ReturnsNull()
    {
        var policy = new CompositePolicy();

        var decision = policy.Evaluate(new DialogContext("hello", null));

        await Assert.That(decision).IsNull();
    }

    [Test]
    public async Task Evaluate_When_FirstPolicyMatches_Then_ReturnsItsDecision()
    {
        var first = new AlertPolicy();
        first.WhenMessageMatches("hello").Suppress();
        var second = new AlertPolicy();
        second.WhenMessageMatches("hello").Show();
        var policy = new CompositePolicy(first, second);

        var decision = policy.Evaluate(new DialogContext("hello", null));

        await Assert.That(decision).IsNotNull();
        await Assert.That(decision!.Kind).IsEqualTo(DialogDecisionKind.Suppress);
    }

    [Test]
    public async Task Evaluate_When_OnlyLaterPolicyMatches_Then_ReturnsItsDecision()
    {
        var first = new AlertPolicy();
        first.WhenMessageMatches("other").Show();
        var second = new ConfirmPolicy();
        second.WhenMessageMatches("delete").AutoReject();
        var policy = new CompositePolicy(first, second);

        var decision = policy.Evaluate(new DialogContext("delete item", null));

        await Assert.That(decision).IsNotNull();
        await Assert.That(decision!.Kind).IsEqualTo(DialogDecisionKind.AutoReject);
    }
}
