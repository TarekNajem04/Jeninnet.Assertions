//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

// MSTEST0049 waived file-wide: these tests pin explicit/controlled tokens; TestContext routing would defeat the subject.
#pragma warning disable MSTEST0049
[TestClass]
public class PredicatePollingTests {
    [TestMethod]
    public void EventuallySatisfy_PassImmediately() {
        var attempts = 0;
        Func<int> source = () => {
            attempts++;
            return 5;
        };

        source.Should().EventuallySatisfy(static value => value == 5, TimeSpan.FromSeconds(5));
        Assert.AreEqual(1, attempts);
    }

    [TestMethod]
    public void EventuallySatisfy_PassEventually() {
        var attempts = 0;
        Func<int> source = () => {
            attempts++;
            return attempts;
        };

        source.Should().EventuallySatisfy(static value => value >= 3, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(10));
        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public void EventuallySatisfy_FailsAfterTimeout() {
        Func<int> source = () => 5;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => source.Should().EventuallySatisfy(static value => value == 6, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("Expected source to satisfy the condition within 00:00:00.1000000, but the last value was 5.", ex.Message);
        Assert.IsNull(ex.InnerException);
    }

    [TestMethod]
    public void EventuallySatisfy_RetriesTransientErrors() {
        var attempts = 0;
        Func<int> source = () => {
            attempts++;
            return attempts < 3 ? throw new InvalidOperationException("warming up") : 5;
        };

        source.Should().EventuallySatisfy(static value => value == 5, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(10));
        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public void EventuallySatisfy_ReportsLastError() {
        Func<int> source = () => throw new InvalidOperationException("boom");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => source.Should().EventuallySatisfy(static value => value == 5, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("Expected source to satisfy the condition within 00:00:00.1000000, but the last value was <null> (last error: boom).", ex.Message);
        Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
    }

    [TestMethod]
    public void EventuallySatisfy_NegativeBudgets_ThrowArgumentOutOfRange() {
        Func<int> source = () => 5;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => source.Should().EventuallySatisfy(static _ => true, TimeSpan.FromMilliseconds(-1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => source.Should().EventuallySatisfy(static _ => true, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public void EventuallySatisfy_NullPredicate_ThrowsArgumentNull() {
        Func<int> source = () => 5;
        Assert.ThrowsExactly<ArgumentNullException>(() => source.Should().EventuallySatisfy(null!, TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public async Task EventuallySatisfyAsync_PassEventually() {
        var attempts = 0;
        Func<Task<int>> source = () => {
            attempts++;
            return Task.FromResult(attempts);
        };

        await source.Should().EventuallySatisfyAsync(static value => value >= 3, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(10));
        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public async Task EventuallySatisfyAsync_FailsAfterTimeout() {
        Func<Task<int>> source = () => Task.FromResult(5);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => source.Should().EventuallySatisfyAsync(static value => value == 6, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("Expected source to satisfy the condition within 00:00:00.1000000, but the last value was 5.", ex.Message);
    }

    [TestMethod]
    public async Task EventuallySatisfyAsync_NullPredicate_ThrowsArgumentNull() {
        Func<Task<int>> source = () => Task.FromResult(5);
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => source.Should().EventuallySatisfyAsync(null!, TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public async Task EventuallySatisfyAsync_PreCancelled_ThrowsImmediately() {
        var attempts = 0;
        Func<Task<int>> source = () => {
            attempts++;
            return Task.FromResult(attempts);
        };

        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            () => source.Should().EventuallySatisfyAsync(static value => value > 100, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(10), cancelled.Token));
        Assert.AreEqual(0, attempts);
    }

    [TestMethod]
    public async Task EventuallySatisfyAsync_CancelMidPoll_Throws() {
        Func<Task<int>> source = () => Task.FromResult(0);
        using var cancellation = new CancellationTokenSource();
        cancellation.CancelAfter(TimeSpan.FromMilliseconds(50));

        Exception? caught = null;
        try {
            await source.Should().EventuallySatisfyAsync(static value => value > 100, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(10), cancellation.Token);
        }
        catch(OperationCanceledException ex) {
            caught = ex;
        }

        Assert.IsNotNull(caught);
    }

    [TestMethod]
    public void EventuallySatisfy_ArabicMessage() {
        var previous = AssertionCulture.Current;
        AssertionCulture.Current = new CultureInfo("ar");
        try {
            Func<int> source = () => 5;
            var ex = Assert.ThrowsExactly<AssertionFailedException>(
                () => source.Should().EventuallySatisfy(static value => value == 6, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
            Assert.AreEqual("المتوقع من source أن يحقق الشرط خلال 00:00:00.1000000، لكن القيمة الأخيرة كانت 5.", ex.Message);
        }
        finally {
            AssertionCulture.Current = previous;
        }
    }

    [TestMethod]
    public void EventuallySatisfy_ExponentialBackoff_PassesEventually() {
        var attempts = 0;
        Func<int> source = () => {
            attempts++;
            return attempts;
        };

        source.Should().EventuallySatisfy(static value => value >= 3, PollingBackoff.Exponential(TimeSpan.FromMilliseconds(1), 2.0, TimeSpan.FromMilliseconds(20)), TimeSpan.FromSeconds(5));
        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public void EventuallySatisfy_NullBackoff_ThrowsArgumentNull() {
        Func<int> source = () => 5;

        Assert.ThrowsExactly<ArgumentNullException>(() => source.Should().EventuallySatisfy(static _ => true, null!, TimeSpan.FromSeconds(1)));
    }

    [TestMethod]
    public async Task EventuallySatisfyAsync_ExponentialBackoff_PassesEventually() {
        var attempts = 0;
        Func<Task<int>> source = () => {
            attempts++;
            return Task.FromResult(attempts);
        };

        await source.Should().EventuallySatisfyAsync(static value => value >= 2, PollingBackoff.Exponential(TimeSpan.FromMilliseconds(1)), TimeSpan.FromSeconds(5));
        Assert.AreEqual(2, attempts);
    }
}
#pragma warning restore MSTEST0049
