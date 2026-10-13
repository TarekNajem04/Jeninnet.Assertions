//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class PollingTests {
    [TestMethod]
    public void SucceedWithin_PassImmediately() {
        var attempts = 0;
        Action action = () => attempts++;
        action.Should().SucceedWithin(TimeSpan.FromSeconds(5));
        Assert.AreEqual(1, attempts);
    }

    [TestMethod]
    public void SucceedWithin_PassEventually() {
        var attempts = 0;
        Action action = () => {
            attempts++;
            if(attempts < 3) {
                throw new InvalidOperationException("not yet");
            }
        };

        action.Should().SucceedWithin(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(10));
        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public void SucceedWithin_FailsAfterTimeout() {
        Action action = () => throw new InvalidOperationException("always");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => action.Should().SucceedWithin(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        Assert.Contains("Expected action to succeed within", ex.Message);
        Assert.Contains("but it kept failing: always.", ex.Message);
        Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
    }

    [TestMethod]
    public void SucceedWithin_NegativeBudgets_ThrowArgumentOutOfRange() {
        Action action = () => { };

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => action.Should().SucceedWithin(TimeSpan.FromMilliseconds(-1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => action.Should().SucceedWithin(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public async Task SucceedWithinAsync_PassEventually_Async() {
        var attempts = 0;
        Func<Task> action = () => {
            attempts++;
            return attempts < 3
                ? Task.FromException(new InvalidOperationException("not yet"))
                : Task.CompletedTask;
        };

        await action.Should().SucceedWithinAsync(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(10));
        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public async Task SucceedWithinAsync_FailsAfterTimeout_Async() {
        Func<Task> action = () => Task.FromException(new TimeoutException("slow"));
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => action.Should().SucceedWithinAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        Assert.Contains("but it kept failing: slow.", ex.Message);
    }

    [TestMethod]
    public void SucceedWithin_ExponentialBackoff_PassesEventually() {
        var attempts = 0;
        Action action = () => {
            attempts++;
            if(attempts < 3) {
                throw new InvalidOperationException("not yet");
            }
        };

        action.Should().SucceedWithin(PollingBackoff.Exponential(TimeSpan.FromMilliseconds(1), 2.0, TimeSpan.FromMilliseconds(20)), TimeSpan.FromSeconds(5));
        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public void SucceedWithin_NullBackoff_ThrowsArgumentNull() {
        Action action = () => { };

        Assert.ThrowsExactly<ArgumentNullException>(() => action.Should().SucceedWithin(null!, TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public async Task SucceedWithinAsync_ExponentialBackoff_PassesEventually() {
        var attempts = 0;
        Func<Task> action = () => {
            attempts++;
            return attempts < 2
                ? Task.FromException(new InvalidOperationException("not yet"))
                : Task.CompletedTask;
        };

        await action.Should().SucceedWithinAsync(PollingBackoff.Exponential(TimeSpan.FromMilliseconds(1), 2.0, TimeSpan.FromMilliseconds(20)), TimeSpan.FromSeconds(5));
        Assert.AreEqual(2, attempts);
    }
}
