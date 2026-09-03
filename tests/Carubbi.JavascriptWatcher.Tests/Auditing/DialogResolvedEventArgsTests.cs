using Carubbi.JavascriptWatcher.Auditing;
using Carubbi.JavascriptWatcher.Policy;

namespace Carubbi.JavascriptWatcher.Tests.Auditing;

public class DialogResolvedEventArgsTests
{
    [Test]
    public async Task Record_When_Constructed_Then_PreservesAllProperties()
    {
        var timestamp = DateTimeOffset.UtcNow;
        var decision = DialogDecision.SuppressDecision();
        var args = new DialogResolvedEventArgs("Alert", "hello", "https://app.example.com", decision, timestamp);

        await Assert.That(args.DialogType).IsEqualTo("Alert");
        await Assert.That(args.Message).IsEqualTo("hello");
        await Assert.That(args.SourceUrl).IsEqualTo("https://app.example.com");
        await Assert.That(args.Decision).IsNotNull();
        await Assert.That(args.Decision!.Kind).IsEqualTo(DialogDecisionKind.Suppress);
        await Assert.That(args.Timestamp).IsEqualTo(timestamp);
    }

    [Test]
    public async Task Record_When_DecisionIsNull_Then_AllowsNullDecision()
    {
        var args = new DialogResolvedEventArgs("WindowOpen", "https://x.com", null, null, DateTimeOffset.UtcNow);

        await Assert.That(args.Decision).IsNull();
        await Assert.That(args.SourceUrl).IsNull();
    }
}
