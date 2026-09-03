namespace Carubbi.JavascriptWatcher.Tests;

public class AlertInterceptedEventArgsTests
{
    [Test]
    public async Task Constructor_When_MessageProvided_Then_MessageIsStored()
    {
        var args = new AlertInterceptedEventArgs("hello");

        await Assert.That(args.Message).IsEqualTo("hello");
    }

    [Test]
    public async Task Constructor_When_NullMessage_Then_MessageIsNull()
    {
        var args = new AlertInterceptedEventArgs(null!);

        await Assert.That(args.Message).IsNull();
    }

    [Test]
    public async Task Constructor_When_EmptyMessage_Then_MessageIsEmpty()
    {
        var args = new AlertInterceptedEventArgs("");

        await Assert.That(args.Message).IsEmpty();
    }

    [Test]
    public async Task Inherits_When_Created_Then_ExtendsEventArgs()
    {
        var args = new AlertInterceptedEventArgs("test");

        await Assert.That(args).IsAssignableTo<EventArgs>();
    }
}
