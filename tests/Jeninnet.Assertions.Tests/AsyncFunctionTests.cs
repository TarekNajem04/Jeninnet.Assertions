//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class AsyncFunctionTests {
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    public async Task ThrowAsync_PassForMatchingException_Async() {
        Func<Task<int>> func = () => Task.FromException<int>(new InvalidOperationException("async"));
        var assertions = await func.Should().ThrowAsync<InvalidOperationException>();
        Assert.AreEqual("async", assertions.Exception.Message);
    }

    [TestMethod]
    public async Task ThrowAsync_FailsForWrongType_Async() {
        Func<Task<int>> func = () => Task.FromException<int>(new ArgumentException("boom"));
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => func.Should().ThrowAsync<InvalidOperationException>());
        Assert.AreEqual("Expected func to throw InvalidOperationException, but ArgumentException was thrown.", ex.Message);
        Assert.IsInstanceOfType<ArgumentException>(ex.InnerException);
    }

    [TestMethod]
    public async Task ThrowExactlyAsync_FailsForDerived_Async() {
        Func<Task<int>> func = () => Task.FromException<int>(new DivideByZeroException());
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => func.Should().ThrowExactlyAsync<ArithmeticException>());
        Assert.AreEqual("Expected func to throw exactly ArithmeticException, but DivideByZeroException was thrown.", ex.Message);
    }

    [TestMethod]
    public async Task NotThrowAsync_PassForCompletedValue_Async() {
        Func<Task<int>> func = () => Task.FromResult(42);
        await func.Should().NotThrowAsync();
    }

    [TestMethod]
    public async Task NotThrowAsync_FailsForFaulted_Async() {
        Func<Task<int>> func = () => Task.FromException<int>(new InvalidOperationException("bad"));
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => func.Should().NotThrowAsync());
        Assert.AreEqual("Expected func not to throw, but InvalidOperationException was thrown.", ex.Message);
    }

    [TestMethod]
    public async Task NotThrowAsync_FailsForCanceled_Async() {
        Func<Task<int>> func = () => Task.FromCanceled<int>(new CancellationToken(canceled: true));
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => func.Should().NotThrowAsync());
        Assert.IsInstanceOfType<OperationCanceledException>(ex.InnerException);
    }

    [TestMethod]
    public async Task NotThrowAfterAsync_PassAndFail_Async() {
        var attempts = 0;
        Func<Task> func = () => {
            attempts++;
            return attempts < 3
                ? Task.FromException(new InvalidOperationException("not yet"))
                : Task.CompletedTask;
        };

        await func.Should().NotThrowAfterAsync(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(10));
        Assert.AreEqual(3, attempts);

        Func<Task> failing = () => Task.FromException(new InvalidOperationException("always"));
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => failing.Should().NotThrowAfterAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("Expected failing not to throw within 00:00:00.1000000, but InvalidOperationException was thrown.", ex.Message);
    }

    [TestMethod]
    public async Task ThrowWithinAsync_PassAndFail_Async() {
        Func<Task> throwing = () => Task.FromException(new InvalidOperationException("boom"));
        var assertions = await throwing.Should().ThrowWithinAsync(TimeSpan.FromSeconds(5));
        Assert.AreEqual("boom", assertions.Exception.Message);

        Func<Task> quiet = static () => Task.CompletedTask;
        var noneEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => quiet.Should().ThrowWithinAsync(TimeSpan.FromSeconds(5)));
        Assert.AreEqual("Expected quiet to throw Exception within 00:00:05, but no exception was thrown.", noneEx.Message);

        var pending = new TaskCompletionSource();
        Func<Task> pendingFunc = () => pending.Task;
        var timeoutEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => pendingFunc.Should().ThrowWithinAsync(TimeSpan.FromMilliseconds(50)));
        Assert.AreEqual("Expected pendingFunc to throw Exception within 00:00:00.0500000, but it did not complete in time.", timeoutEx.Message);
        Assert.IsInstanceOfType<TimeoutException>(timeoutEx.InnerException);
    }

    [TestMethod]
    public async Task ThrowWithinAsync_Generic_PassFailAndWrongType_Async() {
        Func<Task> throwing = () => Task.FromException(new ArgumentException("boom"));
        var assertions = await throwing.Should().ThrowWithinAsync<ArgumentException>(TimeSpan.FromSeconds(5));
        Assert.AreEqual("boom", assertions.Exception.Message);

        var wrongEx = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => throwing.Should().ThrowWithinAsync<InvalidOperationException>(TimeSpan.FromSeconds(5)));
        Assert.AreEqual("Expected throwing to throw InvalidOperationException within 00:00:05, but ArgumentException was thrown.", wrongEx.Message);
        Assert.IsInstanceOfType<ArgumentException>(wrongEx.InnerException);

        Func<Task<int>> valueThrowing = () => Task.FromException<int>(new InvalidOperationException("value boom"));
        var valueAssertions = await valueThrowing.Should().ThrowWithinAsync<InvalidOperationException>(TimeSpan.FromSeconds(5));
        Assert.AreEqual("value boom", valueAssertions.Exception.Message);
    }

    [TestMethod]
    public async Task ThrowWithinAsync_NegativeTimeout_ThrowsArgumentOutOfRange_Async() {
        Func<Task> func = static () => Task.CompletedTask;
        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(() => func.Should().ThrowWithinAsync(TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public async Task CompleteWithinAsync_PassAndFail_Async() {
        Func<Task> quick = static () => Task.CompletedTask;
        await quick.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5));

        Func<Task> slow = () => Task.Delay(TimeSpan.FromSeconds(30), TestContext.CancellationToken);
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => slow.Should().CompleteWithinAsync(TimeSpan.FromMilliseconds(50)));
        Assert.AreEqual("Expected slow to complete within 00:00:00.0500000, but it did not.", ex.Message);

        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(
            () => quick.Should().CompleteWithinAsync(TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public async Task NotCompleteWithinAsync_PassAndFail_Async() {
        Func<Task> slow = () => Task.Delay(TimeSpan.FromSeconds(30), TestContext.CancellationToken);
        await slow.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50));

        Func<Task> quick = static () => Task.CompletedTask;
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => quick.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50)));
        Assert.AreEqual("Expected quick not to complete within 00:00:00.0500000, but it completed.", ex.Message);
    }

    [TestMethod]
    public async Task CompleteWithinAsync_Generic_ExposesResult_Async() {
        Func<Task<int>> quick = () => Task.FromResult(42);
        var which = await quick.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5));
        Assert.AreEqual(42, which.Which);

        Func<Task<int>> slow = async () => {
            await Task.Delay(TimeSpan.FromSeconds(30), TestContext.CancellationToken);
            return 1;
        };
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => slow.Should().CompleteWithinAsync(TimeSpan.FromMilliseconds(50)));
        Assert.AreEqual("Expected slow to complete within 00:00:00.0500000, but it did not.", ex.Message);

        var pendingSource = new TaskCompletionSource<int>();
        Func<Task<int>> pending = () => pendingSource.Task;
        await pending.Should().NotCompleteWithinAsync(TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsExactlyAsync<ArgumentOutOfRangeException>(
            () => quick.Should().CompleteWithinAsync(TimeSpan.FromMilliseconds(-1)));
    }
}
