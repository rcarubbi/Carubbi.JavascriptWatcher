using Carubbi.JavascriptWatcher.Policy;

namespace Carubbi.JavascriptWatcher.Tests.Policy;

public class ConfirmPolicyTests
{
    [Test]
    public async Task Evaluate_When_MessageMatchesAutoAccept_Then_ReturnsAutoAccept()
    {
        var policy = new ConfirmPolicy();
        policy.WhenMessageMatches(@"delete").AutoAccept();

        var decision = policy.Evaluate(new DialogContext("Delete this item?", null));

        await Assert.That(decision).IsNotNull();
        await Assert.That(decision!.Kind).IsEqualTo(DialogDecisionKind.AutoAccept);
    }

    [Test]
    public async Task Evaluate_When_MessageMatchesAutoReject_Then_ReturnsAutoReject()
    {
        var policy = new ConfirmPolicy();
        policy.WhenMessageMatches(@"persist").AutoReject();

        var decision = policy.Evaluate(new DialogContext("Persist changes?", null));

        await Assert.That(decision).IsNotNull();
        await Assert.That(decision!.Kind).IsEqualTo(DialogDecisionKind.AutoReject);
    }

    [Test]
    public async Task Evaluate_When_NoRuleMatches_Then_ReturnsNull()
    {
        var policy = new ConfirmPolicy();
        policy.WhenMessageMatches(@"delete").AutoAccept();

        var decision = policy.Evaluate(new DialogContext("unrelated prompt", null));

        await Assert.That(decision).IsNull();
    }

    [Test]
    public async Task Evaluate_When_SourceUrlMatches_Then_AppliesDecision()
    {
        var policy = new ConfirmPolicy();
        policy.WhenSourceUrlMatches(@"evil\.com").AutoReject();

        var decision = policy.Evaluate(new DialogContext("Continue?", "https://evil.com/x"));

        await Assert.That(decision).IsNotNull();
        await Assert.That(decision!.Kind).IsEqualTo(DialogDecisionKind.AutoReject);
    }
}
