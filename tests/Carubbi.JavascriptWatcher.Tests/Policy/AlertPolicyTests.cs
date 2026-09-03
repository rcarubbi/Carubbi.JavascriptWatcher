using Carubbi.JavascriptWatcher.Policy;

namespace Carubbi.JavascriptWatcher.Tests.Policy;

public class AlertPolicyTests
{
    [Test]
    public async Task Evaluate_When_MessageMatchesSuppressRule_Then_ReturnsSuppress()
    {
        var policy = new AlertPolicy();
        policy.WhenMessageMatches("success").Suppress();

        var decision = policy.Evaluate(new DialogContext("operation success", null));

        await Assert.That(decision).IsNotNull();
        await Assert.That(decision!.Kind).IsEqualTo(DialogDecisionKind.Suppress);
    }

    [Test]
    public async Task Evaluate_When_MessageDoesNotMatch_Then_ReturnsNull()
    {
        var policy = new AlertPolicy();
        policy.WhenMessageMatches("success").Suppress();

        var decision = policy.Evaluate(new DialogContext("warning", null));

        await Assert.That(decision).IsNull();
    }

    [Test]
    public async Task Evaluate_When_SourceUrlMatchesShowRule_Then_ReturnsAccept()
    {
        var policy = new AlertPolicy();
        policy.WhenSourceUrlMatches(@"https://app\.example\.com").Show();

        var decision = policy.Evaluate(new DialogContext("hello", "https://app.example.com/page"));

        await Assert.That(decision).IsNotNull();
        await Assert.That(decision!.Kind).IsEqualTo(DialogDecisionKind.AutoAccept);
    }

    [Test]
    public async Task Evaluate_When_FirstRuleDoesNotMatch_Then_TriesNextRule()
    {
        var policy = new AlertPolicy();
        policy.WhenMessageMatches("first").Show();
        policy.WhenMessageMatches("second").Suppress();

        var decision = policy.Evaluate(new DialogContext("second", null));

        await Assert.That(decision).IsNotNull();
        await Assert.That(decision!.Kind).IsEqualTo(DialogDecisionKind.Suppress);
    }
}
