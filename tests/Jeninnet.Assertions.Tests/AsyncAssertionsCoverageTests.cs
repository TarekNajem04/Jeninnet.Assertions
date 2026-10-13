//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class AsyncAssertionsCoverageTests {
    [TestMethod]
    public async Task ThrowAsync_NoException_Fails() {
        Func<Task> ok = static () => Task.CompletedTask;
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => ok.Should().ThrowAsync<InvalidOperationException>());
        Assert.AreEqual("Expected ok to throw InvalidOperationException, but no exception was thrown.", ex.Message);
    }

    [TestMethod]
    public async Task ThrowExactlyAsync_WrongTypeAndNone_Fail() {
        Func<Task> boom = static () => Task.FromException(new ArgumentException("x"));
        var wrong = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => boom.Should().ThrowExactlyAsync<InvalidOperationException>());
        Assert.AreEqual("Expected boom to throw exactly InvalidOperationException, but ArgumentException was thrown.", wrong.Message);

        Func<Task> ok = static () => Task.CompletedTask;
        var none = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => ok.Should().ThrowExactlyAsync<InvalidOperationException>());
        Assert.AreEqual("Expected ok to throw exactly InvalidOperationException, but no exception was thrown.", none.Message);
    }

    [TestMethod]
    public async Task NotThrowAfterAsync_Guards_ThrowArgumentOutOfRange() {
        Func<Task> ok = static () => Task.CompletedTask;
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => ok.Should().NotThrowAfterAsync(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(10)));
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => ok.Should().NotThrowAfterAsync(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public async Task SucceedWithinAsync_Guards_ThrowForNullBackoffAndNegativeTimeout() {
        Func<Task> ok = static () => Task.CompletedTask;
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => ok.Should().SucceedWithinAsync(null!, TimeSpan.FromSeconds(1)));
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => ok.Should().SucceedWithinAsync(TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public async Task AllocateAtMostAsync_GuardsThrewAndExceeded() {
        Func<Task> ok = static () => Task.CompletedTask;
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => ok.Should().AllocateAtMostAsync(-1));

        Func<Task> boom = static () => Task.FromException(new InvalidOperationException("x"));
        var threw = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => boom.Should().AllocateAtMostAsync(1024));
        Assert.AreEqual("Expected boom to allocate at most 1 KB, but it threw InvalidOperationException.", threw.Message);

        Func<Task> alloc = static () => {
            _ = new byte[50_000];
            return Task.CompletedTask;
        };
        var exceeded = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => alloc.Should().AllocateAtMostAsync(1024));
        Assert.Contains("to allocate at most 1 KB, but allocated", exceeded.Message);
    }

    [TestMethod]
    public async Task AllocateLessThanAsync_GuardsThrewAndExceeded() {
        Func<Task> ok = static () => Task.CompletedTask;
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => ok.Should().AllocateLessThanAsync(-1));

        Func<Task> boom = static () => Task.FromException(new InvalidOperationException("x"));
        var threw = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => boom.Should().AllocateLessThanAsync(1024));
        Assert.AreEqual("Expected boom to allocate less than 1 KB, but it threw InvalidOperationException.", threw.Message);

        Func<Task> alloc = static () => {
            _ = new byte[50_000];
            return Task.CompletedTask;
        };
        var exceeded = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => alloc.Should().AllocateLessThanAsync(1024));
        Assert.Contains("to allocate less than 1 KB, but allocated", exceeded.Message);
    }

    [TestMethod]
    public async Task GenericThrowAsync_NoException_Fails() {
        Func<Task<int>> ok = static () => Task.FromResult(1);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => ok.Should().ThrowAsync<InvalidOperationException>());
        Assert.AreEqual("Expected ok to throw InvalidOperationException, but no exception was thrown.", ex.Message);
    }

    [TestMethod]
    public async Task GenericThrowExactlyAsync_ExactPassAndNoneFail() {
        Func<Task<int>> exact = static () => Task.FromException<int>(new InvalidOperationException("exact"));
        var assertions = await exact.Should().ThrowExactlyAsync<InvalidOperationException>();
        Assert.AreEqual("exact", assertions.Exception.Message);

        Func<Task<int>> ok = static () => Task.FromResult(1);
        var none = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => ok.Should().ThrowExactlyAsync<InvalidOperationException>());
        Assert.AreEqual("Expected ok to throw exactly InvalidOperationException, but no exception was thrown.", none.Message);
    }

    [TestMethod]
    public async Task GenericThrowWithinAsync_WrapperAndGuards() {
        Func<Task<int>> throwing = static () => Task.FromException<int>(new InvalidOperationException("boom"));
        var assertions = await throwing.Should().ThrowWithinAsync(TimeSpan.FromSeconds(5));
        Assert.AreEqual("boom", assertions.Exception.Message);

        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(
            () => throwing.Should().ThrowWithinAsync<InvalidOperationException>(TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public async Task GenericThrowWithinAsync_TimeoutAndNone_Fail() {
        var pending = new TaskCompletionSource<int>();
        Func<Task<int>> pendingFunc = () => pending.Task;
        var timeout = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => pendingFunc.Should().ThrowWithinAsync<InvalidOperationException>(TimeSpan.FromMilliseconds(50)));
        Assert.AreEqual("Expected pendingFunc to throw InvalidOperationException within 00:00:00.0500000, but it did not complete in time.", timeout.Message);
        Assert.IsInstanceOfType<TimeoutException>(timeout.InnerException);

        Func<Task<int>> quiet = static () => Task.FromResult(1);
        var none = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => quiet.Should().ThrowWithinAsync<InvalidOperationException>(TimeSpan.FromSeconds(5)));
        Assert.AreEqual("Expected quiet to throw InvalidOperationException within 00:00:05, but no exception was thrown.", none.Message);
    }

    [TestMethod]
    public async Task GenericNotThrowAfterAsync_GuardsAndFailure() {
        Func<Task<int>> ok = static () => Task.FromResult(1);
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(
            () => ok.Should().NotThrowAfterAsync(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(10)));
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(
            () => ok.Should().NotThrowAfterAsync(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(-1)));

        Func<Task<int>> failing = static () => Task.FromException<int>(new InvalidOperationException("always"));
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => failing.Should().NotThrowAfterAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("Expected failing not to throw within 00:00:00.1000000, but InvalidOperationException was thrown.", ex.Message);
    }

    [TestMethod]
    public async Task GenericSucceedWithinAsync_WrapperGuardsAndFailure() {
        var attempts = 0;
        Func<Task<int>> flaky = () => {
            attempts++;
            return attempts < 2
                ? Task.FromException<int>(new InvalidOperationException("not yet"))
                : Task.FromResult(attempts);
        };
        await flaky.Should().SucceedWithinAsync(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(10));
        Assert.AreEqual(2, attempts);

        Func<Task<int>> ok = static () => Task.FromResult(1);
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => ok.Should().SucceedWithinAsync(null!, TimeSpan.FromSeconds(1)));
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => ok.Should().SucceedWithinAsync(TimeSpan.FromMilliseconds(-1)));

        Func<Task<int>> failing = static () => Task.FromException<int>(new InvalidOperationException("always"));
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => failing.Should().SucceedWithinAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("Expected failing to succeed within 00:00:00.1000000, but it kept failing: always.", ex.Message);
    }

    [TestMethod]
    public async Task GenericEventuallySatisfyAsync_FailurePathsAndGuards() {
        Func<Task<int>> five = static () => Task.FromResult(5);
        var mismatch = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => five.Should().EventuallySatisfyAsync(static x => x > 10, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("Expected five to satisfy the condition within 00:00:00.0500000, but the last value was 5.", mismatch.Message);

        Func<Task<int>> throwing = static () => Task.FromException<int>(new InvalidOperationException("boom"));
        var errored = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => throwing.Should().EventuallySatisfyAsync(static x => x > 10, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("Expected throwing to satisfy the condition within 00:00:00.0500000, but the last value was <null> (last error: boom).", errored.Message);

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => five.Should().EventuallySatisfyAsync(null!, TimeSpan.FromSeconds(1)));
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => five.Should().EventuallySatisfyAsync(static x => x > 0, null!, TimeSpan.FromSeconds(1)));
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(
            () => five.Should().EventuallySatisfyAsync(static x => x > 0, TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public async Task GenericAllocateAtMostAsync_GuardsThrewAndExceeded() {
        Func<Task<int>> ok = static () => Task.FromResult(1);
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => ok.Should().AllocateAtMostAsync(-1));

        Func<Task<int>> boom = static () => Task.FromException<int>(new InvalidOperationException("x"));
        var threw = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => boom.Should().AllocateAtMostAsync(1024));
        Assert.AreEqual("Expected boom to allocate at most 1 KB, but it threw InvalidOperationException.", threw.Message);

        Func<Task<int>> alloc = static () => Task.FromResult(new byte[50_000].Length);
        var exceeded = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => alloc.Should().AllocateAtMostAsync(1024));
        Assert.Contains("to allocate at most 1 KB, but allocated", exceeded.Message);
    }

    [TestMethod]
    public async Task GenericAllocateLessThanAsync_GuardsThrewAndExceeded() {
        Func<Task<int>> ok = static () => Task.FromResult(1);
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => ok.Should().AllocateLessThanAsync(-1));

        Func<Task<int>> boom = static () => Task.FromException<int>(new InvalidOperationException("x"));
        var threw = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => boom.Should().AllocateLessThanAsync(1024));
        Assert.AreEqual("Expected boom to allocate less than 1 KB, but it threw InvalidOperationException.", threw.Message);

        Func<Task<int>> alloc = static () => Task.FromResult(new byte[50_000].Length);
        var exceeded = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => alloc.Should().AllocateLessThanAsync(1024));
        Assert.Contains("to allocate less than 1 KB, but allocated", exceeded.Message);
    }
}
