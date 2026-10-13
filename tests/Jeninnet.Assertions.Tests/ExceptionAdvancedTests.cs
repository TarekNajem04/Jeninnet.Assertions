//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class ExceptionAdvancedTests {
    [TestMethod]
    public void WithMessage_PassForWildcard() {
        Action action = () => throw new InvalidOperationException("boom happened");
        action.Should().Throw<InvalidOperationException>().WithMessage("boom*").And.WithMessage("*happened").And.WithMessage("b??m happened");
    }

    [TestMethod]
    public void WithMessage_FailsForMismatch() {
        Action action = () => throw new InvalidOperationException("boom happened");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>().WithMessage("bam*"));
        Assert.AreEqual("Expected exception message to match 'bam*', but found 'boom happened'.", ex.Message);
    }

    [TestMethod]
    public void WithInnerException_PassForDerived() {
        Action action = () => throw new InvalidOperationException("outer", new ArgumentException("inner"));
        action.Should().Throw<InvalidOperationException>().WithInnerException<ArgumentException>();
    }

    [TestMethod]
    public void WithInnerException_FailsForWrongTypeAndMissing() {
        Action action = () => throw new InvalidOperationException("outer", new ArgumentException("inner"));
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => action.Should().Throw<InvalidOperationException>().WithInnerException<TimeoutException>());
        Assert.AreEqual("Expected inner exception to be TimeoutException, but found ArgumentException.", ex.Message);

        Action plain = () => throw new InvalidOperationException("outer");
        var missing = Assert.ThrowsExactly<AssertionFailedException>(
            () => plain.Should().Throw<InvalidOperationException>().WithInnerException<ArgumentException>());
        Assert.AreEqual("Expected inner exception to be ArgumentException, but found <null>.", missing.Message);
    }

    [TestMethod]
    public void WithoutMessage_PassAndFail() {
        Action action = () => throw new InvalidOperationException("boom happened");
        action.Should().Throw<InvalidOperationException>().WithoutMessage("bam*");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => action.Should().Throw<InvalidOperationException>().WithoutMessage("boom*"));
        Assert.AreEqual("Expected exception message not to match 'boom*', but found 'boom happened'.", ex.Message);
    }

    [TestMethod]
    public void WithInnerExceptionExactly_PassFailAndTypeOverload() {
        Action action = () => throw new InvalidOperationException("outer", new ArgumentException("inner"));
        var expectedType = typeof(ArgumentException);
        action.Should().Throw<InvalidOperationException>().WithInnerExceptionExactly<ArgumentException>();
        action.Should().Throw<InvalidOperationException>().WithInnerExceptionExactly(expectedType);

        Action derived = () => throw new InvalidOperationException("outer", new DivideByZeroException());
        var derivedEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => derived.Should().Throw<InvalidOperationException>().WithInnerExceptionExactly<ArithmeticException>());
        Assert.AreEqual("Expected inner exception to be exactly ArithmeticException, but found DivideByZeroException.", derivedEx.Message);

        Action plain = () => throw new InvalidOperationException("outer");
        var missingEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => plain.Should().Throw<InvalidOperationException>().WithInnerExceptionExactly<ArgumentException>());
        Assert.AreEqual("Expected inner exception to be exactly ArgumentException, but found <null>.", missingEx.Message);

        Assert.ThrowsExactly<ArgumentNullException>(
            () => action.Should().Throw<InvalidOperationException>().WithInnerExceptionExactly((Type)null!));
        Assert.ThrowsExactly<ArgumentException>(
            () => action.Should().Throw<InvalidOperationException>().WithInnerExceptionExactly(typeof(string)));
    }

    [TestMethod]
    public void ExceptionAssertions_WhichExposesCaughtException() {
        Action action = () => throw new InvalidOperationException("boom");
        var assertions = action.Should().Throw<InvalidOperationException>();
        Assert.AreSame(assertions.Exception, assertions.Which);
        assertions.Which.Should().BeOfType<InvalidOperationException>();
    }

    [TestMethod]
    public void Where_PassAndFail() {
        Action action = () => throw new InvalidOperationException("boom happened");
        action.Should().Throw<InvalidOperationException>().Where(static ex => ex.Message.Contains("boom"));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => action.Should().Throw<InvalidOperationException>().Where(static e => e.Message.Contains("zzz")));
        Assert.Contains("but it did not.", ex.Message);
    }

    [TestMethod]
    public void WithParameterName_PassAndFail() {
        Action action = static () => ThrowArgument("myParam");
        action.Should().Throw<ArgumentException>().WithParameterName("myParam");

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => action.Should().Throw<ArgumentException>().WithParameterName("other"));
        Assert.AreEqual("Expected exception parameter name to be 'other', but found 'myParam'.", ex.Message);

        Action plain = () => throw new InvalidOperationException("boom");
        var missing = Assert.ThrowsExactly<AssertionFailedException>(
            () => plain.Should().Throw<InvalidOperationException>().WithParameterName("param"));
        Assert.AreEqual("Expected exception parameter name to be 'param', but found <null>.", missing.Message);
    }

    private static void ThrowArgument(string myParam) => throw new ArgumentException("bad", myParam);

    [TestMethod]
    public void ThrowExactly_PassForExactType() {
        Action action = () => throw new ArgumentException("boom");
        var assertions = action.Should().ThrowExactly<ArgumentException>();
        Assert.AreEqual("boom", assertions.Exception.Message);
    }

    [TestMethod]
    public void ThrowExactly_FailsForDerivedType() {
        Action action = static () => throw new DivideByZeroException();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().ThrowExactly<ArithmeticException>());
        Assert.AreEqual("Expected action to throw exactly ArithmeticException, but DivideByZeroException was thrown.", ex.Message);
        Assert.IsInstanceOfType<DivideByZeroException>(ex.InnerException);
    }

    [TestMethod]
    public void ThrowExactly_FailsWhenNothingThrown() {
        Action action = () => { };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().ThrowExactly<InvalidOperationException>());
        Assert.AreEqual("Expected action to throw exactly InvalidOperationException, but no exception was thrown.", ex.Message);
    }

    [TestMethod]
    public void ThrowExactly_Func_PassAndFail() {
        Func<int> func = static () => throw new ArgumentException("arg");
        func.Should().ThrowExactly<ArgumentException>();

        Func<int> derived = static () => throw new DivideByZeroException();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => derived.Should().ThrowExactly<ArithmeticException>());
        Assert.AreEqual("Expected derived to throw exactly ArithmeticException, but DivideByZeroException was thrown.", ex.Message);
    }

    [TestMethod]
    public async Task ThrowExactlyAsync_PassAndFail_Async() {
        Func<Task> func = () => Task.FromException(new ArgumentException("arg"));
        await func.Should().ThrowExactlyAsync<ArgumentException>();

        Func<Task> derived = () => Task.FromException(new DivideByZeroException());
        var ex = await Assert.ThrowsExactlyAsync<AssertionFailedException>(() => derived.Should().ThrowExactlyAsync<ArithmeticException>());
        Assert.AreEqual("Expected derived to throw exactly ArithmeticException, but DivideByZeroException was thrown.", ex.Message);
    }
}
