namespace Carubbi.JavascriptWatcher.Tests;

public class ConfirmInterceptedEventArgsTests
{
    [Test]
    public async Task Constructor_When_MessageProvided_Then_MessageIsStored()
    {
        var args = new ConfirmInterceptedEventArgs("confirm this?");

        await Assert.That(args.Message).IsEqualTo("confirm this?");
    }

    [Test]
    public async Task Result_When_Default_Then_IsFalse()
    {
        var args = new ConfirmInterceptedEventArgs("test");

        await Assert.That(args.Result).IsFalse();
    }

    [Test]
    public async Task Result_When_SetToTrue_Then_ReturnsTrue()
    {
        var args = new ConfirmInterceptedEventArgs("test")
        {
            Result = true
        };

        await Assert.That(args.Result).IsTrue();
    }

    [Test]
    public async Task Inherits_When_Created_Then_ExtendsEventArgs()
    {
        var args = new ConfirmInterceptedEventArgs("test");

        await Assert.That(args).IsAssignableTo<EventArgs>();
    }
}
