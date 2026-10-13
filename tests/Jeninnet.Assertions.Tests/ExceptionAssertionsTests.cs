//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class ExceptionAssertionsTests {
    [TestMethod]
    public void Throw_PassForMatchingException() {
        Action action = () => throw new InvalidOperationException("boom");
        var assertions = action.Should().Throw<InvalidOperationException>();
        Assert.AreEqual("boom", assertions.Exception.Message);
    }

    [TestMethod]
    public void Throw_Action_ReturnsConcreteException() {
        Action action = static () => throw new ArgumentException("arg");
        action.Should().Throw<ArgumentException>().Exception.Should().BeOfType<ArgumentException>();
    }

    [TestMethod]
    public void Throw_PassForDerivedExceptionMatch() {
        Action action = static () => throw new DivideByZeroException();
        action.Should().Throw<ArithmeticException>();
    }

    [TestMethod]
    public void Throw_FailsWhenNoExceptionThrown() {
        Action action = () => { };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>());
        Assert.AreEqual("Expected action to throw InvalidOperationException, but no exception was thrown.", ex.Message);
    }

    [TestMethod]
    public void Throw_FailsForWrongExceptionType() {
        Action action = () => throw new ArgumentException("boom");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>());
        Assert.AreEqual("Expected action to throw InvalidOperationException, but ArgumentException was thrown.", ex.Message);
    }

    [TestMethod]
    public void Throw_WrongType_CapturesInnerException() {
        Action action = () => throw new ArgumentException("boom");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>());
        Assert.IsInstanceOfType<ArgumentException>(ex.InnerException);
    }

    [TestMethod]
    public void Throw_Func_PassForMatchingException() {
        Func<int> func = static () => throw new InvalidOperationException("boom");
        var assertions = func.Should().Throw<InvalidOperationException>();
        Assert.AreEqual("boom", assertions.Exception.Message);
    }

    [TestMethod]
    public void Throw_Func_FailsForWrongExceptionType() {
        Func<int> func = static () => throw new ArgumentException("boom");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => func.Should().Throw<InvalidOperationException>());
        Assert.AreEqual("Expected func to throw InvalidOperationException, but ArgumentException was thrown.", ex.Message);
    }

    [TestMethod]
    public void ThrowAsync_PassForMatchingException() {
        Func<Task> func = () => Task.FromException(new InvalidOperationException("async boom"));
        var assertions = func.Should().ThrowAsync<InvalidOperationException>().Result;
        Assert.AreEqual("async boom", assertions.Exception.Message);
    }

    [TestMethod]
    public async Task ThrowAsync_Operator_Awaited_Async() {
        Func<Task> func = () => Task.FromException(new ArgumentException("arg"));
        var assertions = await func.Should().ThrowAsync<ArgumentException>();
        Assert.AreEqual("arg", assertions.Exception.Message);
    }

    [TestMethod]
    public async Task ThrowAsync_FailsWhenNoExceptionThrown_Async() {
        Func<Task> func = static () => Task.CompletedTask;
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => func.Should().ThrowAsync<InvalidOperationException>());
        Assert.AreEqual("Expected func to throw InvalidOperationException, but no exception was thrown.", ex.Message);
    }

    [TestMethod]
    public async Task ThrowAsync_FailsForWrongExceptionType_Async() {
        Func<Task> func = () => Task.FromException(new ArgumentException("boom"));
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(
            () => func.Should().ThrowAsync<InvalidOperationException>());
        Assert.AreEqual("Expected func to throw InvalidOperationException, but ArgumentException was thrown.", ex.Message);
    }

    [TestMethod]
    public async Task ThrowAsync_DoesNotSwallowFaultedTask_Async() {
        Func<Task> func = () => Task.FromException(new ArgumentException("deep"));
        var assertions = await func.Should().ThrowAsync<ArgumentException>();
        Assert.AreEqual("deep", assertions.Exception.Message);
    }

    [TestMethod]
    public void FluentActions_Invoking_PassAndNull() {
        FluentActions.Invoking(static () => throw new InvalidOperationException("fluent")).Should().Throw<InvalidOperationException>();
        Assert.ThrowsExactly<ArgumentNullException>(() => FluentActions.Invoking(null!));
    }

    [TestMethod]
    public async Task FluentActions_Awaiting_PassAndNull_Async() {
        await FluentActions.Awaiting(() => Task.FromException(new InvalidOperationException("fluent async"))).Should().ThrowAsync<InvalidOperationException>();
        Assert.ThrowsExactly<ArgumentNullException>(() => FluentActions.Awaiting(null!));
    }

    [TestMethod]
    public void FluentActions_Enumerating_ForcesDeferredExecution() {
        static IEnumerable<int> Values() {
            yield return 1;
            throw new InvalidOperationException("deferred");
        }

        Func<IEnumerable<int>> func = Values;
        FluentActions.Enumerating(func).Should().Throw<InvalidOperationException>();
        Assert.ThrowsExactly<ArgumentNullException>(() => FluentActions.Enumerating<int>(null!));
    }
}
