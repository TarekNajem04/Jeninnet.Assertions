//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class NotThrowAssertionsTests {
    [TestMethod]
    public void NotThrow_PassForActionThatCompletes() {
        Action action = () => { };

        action.Should().NotThrow();
    }

    [TestMethod]
    public void NotThrow_FailsForThrowingAction() {
        Action action = () => throw new ArgumentException("boom");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().NotThrow());
        Assert.AreEqual("Expected action not to throw, but ArgumentException was thrown.", ex.Message);
        Assert.IsInstanceOfType<ArgumentException>(ex.InnerException);
    }

    [TestMethod]
    public void NotThrow_UsesCustomMessage() {
        Action action = () => throw new ArgumentException("boom");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().NotThrow("custom not throw message"));
        Assert.AreEqual("Expected action not to throw, but ArgumentException was thrown because custom not throw message.", ex.Message);
    }

    [TestMethod]
    public async Task NotThrowAsync_PassForCompletedTask_Async() {
        Func<Task> func = static () => Task.CompletedTask;
        await func.Should().NotThrowAsync();
    }

    [TestMethod]
    public async Task NotThrowAsync_PassForAsyncThatCompletes_Async() {
        Func<Task> func = () => Task.Delay(1, TestContext.CancellationToken);
        await func.Should().NotThrowAsync();
    }

    [TestMethod]
    public async Task NotThrowAsync_FailsForFaultedTask_Async() {
        Func<Task> func = () => Task.FromException(new InvalidOperationException("async boom"));
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => func.Should().NotThrowAsync());
        Assert.AreEqual("Expected func not to throw, but InvalidOperationException was thrown.", ex.Message);
        Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
    }

    [TestMethod]
    public async Task NotThrowAsync_FailsForTaskThatThrowsSynchronously_Async() {
        Func<Task> func = () => throw new ArgumentException("sync boom");
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => func.Should().NotThrowAsync());
        Assert.AreEqual("Expected func not to throw, but ArgumentException was thrown.", ex.Message);
        Assert.IsInstanceOfType<ArgumentException>(ex.InnerException);
    }

    [TestMethod]
    public async Task NotThrowAsync_FailsForCanceledTask_Async() {
        Func<Task> func = () => Task.FromCanceled(new CancellationToken(canceled: true));
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => func.Should().NotThrowAsync());
        Assert.IsInstanceOfType<OperationCanceledException>(ex.InnerException);
    }

    [TestMethod]
    public async Task NotThrowAsync_UsesCustomMessage_Async() {
        Func<Task> func = () => Task.FromException(new InvalidOperationException("boom"));
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => func.Should().NotThrowAsync("custom not throw async message"));
        Assert.AreEqual("Expected func not to throw, but InvalidOperationException was thrown because custom not throw async message.", ex.Message);
    }

    [TestMethod]
    public void NotThrowAfter_PassWhenQuietImmediately() {
        Action action = () => { };
        action.Should().NotThrowAfter(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10));
    }

    [TestMethod]
    public void NotThrowAfter_PassEventually() {
        var attempts = 0;
        Action action = () => {
            attempts++;
            if(attempts < 3) {
                throw new InvalidOperationException("not yet");
            }
        };

        action.Should().NotThrowAfter(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(10));
        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public void NotThrowAfter_FailsWhenStillThrowing() {
        Action action = () => throw new InvalidOperationException("always");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => action.Should().NotThrowAfter(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(10)));
        Assert.AreEqual("Expected action not to throw within 00:00:00.1000000, but InvalidOperationException was thrown.", ex.Message);
        Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);
    }

    [TestMethod]
    public void NotThrowAfter_NegativeBudgets_ThrowArgumentOutOfRange() {
        Action action = () => { };
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => action.Should().NotThrowAfter(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(10)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => action.Should().NotThrowAfter(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(-1)));
    }

    [TestMethod]
    public void NotThrowAfter_GenericFunc_PassAndFail() {
        var attempts = 0;
        Func<int> func = () => {
            attempts++;
            if(attempts < 2) {
                throw new InvalidOperationException("not yet");
            }

            return attempts;
        };

        func.Should().NotThrowAfter(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(10));
        Assert.AreEqual(2, attempts);

        Func<int> failing = static () => throw new InvalidOperationException("always");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => failing.Should().NotThrowAfter(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(10)));
        Assert.Contains("Expected failing not to throw within", ex.Message);
    }

    [TestMethod]
    public void ActionAssertions_NullAction_ThrowsArgumentNullException() {
        Action? action = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => action!.Should());
    }

    [TestMethod]
    public void ActionAssertionsOfT_NullFunc_ThrowsArgumentNullException() {
        Func<int>? func = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => func!.Should());
    }

    [TestMethod]
    public void AsyncActionAssertions_NullFunc_ThrowsArgumentNullException_Async() {
        Func<Task>? func = null;
        Assert.ThrowsExactly<ArgumentNullException>(() => func!.Should());
    }

    // MSTest injects the instance after construction; the null-forgiving initializer only silences CS8618.
    public TestContext TestContext { get; set; } = null!;
}
