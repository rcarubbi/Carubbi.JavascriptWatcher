using Microsoft.Web.WebView2.WinForms;

namespace Carubbi.JavascriptWatcher.Tests;

public class JavascriptWatcherTests
{
    [Test]
    public async Task Constructor_When_NullWebView_Then_ThrowsArgumentNullException()
    {
        await Assert.That(() => new JavascriptWatcher(null!))
            .ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    public async Task Constructor_When_ValidWebView_Then_CreatesInstance()
    {
        using var webView = new WebView2();
        var watcher = new JavascriptWatcher(webView);

        await Assert.That(watcher).IsNotNull();
    }

    [Test]
    public async Task Start_When_CalledWithInstantAttach_Then_ThrowsInvalidOperationException()
    {
        using var webView = new WebView2();
        var watcher = new JavascriptWatcher(webView);

        await Assert.That(() => watcher.Start(false, false, attachInstantly: true))
            .ThrowsExactly<InvalidOperationException>();
    }

    [Test]
    public async Task Stop_When_CoreWebView2NotInitialized_Then_DoesNotThrow()
    {
        using var webView = new WebView2();
        var watcher = new JavascriptWatcher(webView);

        var act = () => watcher.Stop();
        act();

        await Assert.That(watcher).IsNotNull();
    }
}
