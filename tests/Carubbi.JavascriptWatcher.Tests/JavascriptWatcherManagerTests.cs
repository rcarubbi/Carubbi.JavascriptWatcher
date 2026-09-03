using Carubbi.JavascriptWatcher.Policy;
using Microsoft.Web.WebView2.WinForms;

namespace Carubbi.JavascriptWatcher.Tests;

public class JavascriptWatcherManagerTests
{
    [Test]
    public async Task Add_When_CalledWithPolicy_Then_ReturnsWatcher()
    {
        using var webView = new WebView2();
        var alertPolicy = new AlertPolicy();
        alertPolicy.WhenMessageMatches("x").Suppress();
        var manager = new JavascriptWatcherManager(alertPolicy, null, null);

        var watcher = manager.Add(webView);

        await Assert.That(watcher).IsNotNull();
    }

    [Test]
    public async Task Add_When_WithOverridePolicies_Then_ReturnsWatcher()
    {
        using var webView = new WebView2();
        var confirmPolicy = new ConfirmPolicy();
        confirmPolicy.WhenMessageMatches("delete").AutoAccept();
        var manager = new JavascriptWatcherManager();

        var watcher = manager.Add(webView, null!, confirmPolicy, null!);

        await Assert.That(watcher).IsNotNull();
    }

    [Test]
    public async Task Remove_WhenWebViewNotRegistered_Then_DoesNotThrow()
    {
        using var webView = new WebView2();
        var manager = new JavascriptWatcherManager();

        var act = () => manager.Remove(webView);

        await Assert.That(act).ThrowsNothing();
    }

    [Test]
    public async Task StopAll_When_NoWatchers_Then_DoesNotThrow()
    {
        var manager = new JavascriptWatcherManager();

        var act = () => manager.StopAll();

        await Assert.That(act).ThrowsNothing();
    }
}
