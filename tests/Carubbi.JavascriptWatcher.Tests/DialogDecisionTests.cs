using Carubbi.JavascriptWatcher.Policy;

namespace Carubbi.JavascriptWatcher.Tests;

public class DialogDecisionTests
{
    [Test]
    public async Task Accept_When_Called_Then_ReturnsAutoAccept()
    {
        var decision = DialogDecision.Accept();

        await Assert.That(decision.Kind).IsEqualTo(DialogDecisionKind.AutoAccept);
        await Assert.That(decision.Payload).IsNull();
    }

    [Test]
    public async Task Reject_When_Called_Then_ReturnsAutoReject()
    {
        var decision = DialogDecision.Reject();

        await Assert.That(decision.Kind).IsEqualTo(DialogDecisionKind.AutoReject);
    }

    [Test]
    public async Task SuppressDecision_When_Called_Then_ReturnsSuppress()
    {
        var decision = DialogDecision.SuppressDecision();

        await Assert.That(decision.Kind).IsEqualTo(DialogDecisionKind.Suppress);
    }

    [Test]
    public async Task PassThrough_When_Called_Then_ReturnsPassthrough()
    {
        var decision = DialogDecision.PassThrough();

        await Assert.That(decision.Kind).IsEqualTo(DialogDecisionKind.Passthrough);
    }
}
