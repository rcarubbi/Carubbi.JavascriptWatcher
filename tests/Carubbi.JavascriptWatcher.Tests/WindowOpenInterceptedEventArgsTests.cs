namespace Carubbi.JavascriptWatcher.Tests;

public class WindowOpenInterceptedEventArgsTests
{
    [Test]
    public async Task Constructor_When_UrlProvided_Then_UrlIsStored()
    {
        var uri = new Uri("https://example.com");
        var args = new WindowOpenInterceptedEventArgs(uri);

        await Assert.That(args.Url).IsEqualTo(uri);
    }

    [Test]
    public async Task Inherits_When_Created_Then_ExtendsEventArgs()
    {
        var args = new WindowOpenInterceptedEventArgs(new Uri("https://example.com"));

        await Assert.That(args).IsAssignableTo<EventArgs>();
    }
}
