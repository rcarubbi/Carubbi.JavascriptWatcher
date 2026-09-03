using Carubbi.JavascriptWatcher.Policy;
using Carubbi.JavascriptWatcher.Routing;
using Microsoft.Web.WebView2.WinForms;

namespace Carubbi.JavascriptWatcher.Tests.Policy;

public class WindowOpenPolicyTests
{
    [Test]
    public async Task Evaluate_When_UrlMatchesSuppress_Then_ReturnsSuppress()
    {
        var policy = new WindowOpenPolicy();
        policy.WhenUrlMatches(@"ads\.example\.com").Suppress();

        var decision = policy.Evaluate(new DialogContext("https://ads.example.com/banner", null));

        await Assert.That(decision).IsNotNull();
        await Assert.That(decision!.Kind).IsEqualTo(DialogDecisionKind.Suppress);
    }

    [Test]
    public async Task Evaluate_When_UrlMatchesOpenExternally_Then_ReturnsPassthrough()
    {
        var policy = new WindowOpenPolicy();
        policy.WhenUrlMatches(@"oauth\.example\.com").OpenExternally();

        var decision = policy.Evaluate(new DialogContext("https://oauth.example.com/auth", null));

        await Assert.That(decision).IsNotNull();
        await Assert.That(decision!.Kind).IsEqualTo(DialogDecisionKind.Passthrough);
    }

    [Test]
    public async Task Evaluate_When_UrlMatchesRoute_Then_ReturnsRouteWithTargetPayload()
    {
        using var receiver = new WebView2();
        var target = new WindowOpenTarget(receiver);
        var policy = new WindowOpenPolicy();
        policy.WhenUrlMatches(@"docs\.example\.com").RouteTo(target);

        var decision = policy.Evaluate(new DialogContext("https://docs.example.com/guide", null));

        await Assert.That(decision).IsNotNull();
        await Assert.That(decision!.Kind).IsEqualTo(DialogDecisionKind.Route);
        await Assert.That(ReferenceEquals(decision.Payload, target)).IsTrue();
    }

    [Test]
    public async Task Evaluate_When_NoUrlMatches_Then_ReturnsNull()
    {
        var policy = new WindowOpenPolicy();
        policy.WhenUrlMatches(@"docs\.example\.com").Suppress();

        var decision = policy.Evaluate(new DialogContext("https://other.com/x", null));

        await Assert.That(decision).IsNull();
    }
}
