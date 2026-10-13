//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class AdditionalTypeTests {
    [TestMethod]
    public void WhichConstraint_ExposesMatchedValue() {
        var which = new WhichConstraint<string>("value");
        Assert.AreEqual("value", which.Which);
    }

    [TestMethod]
    public void WhichConstraint_FluentChain_WithString() => "a;b;c".Split(';').Should().ContainSingle(static s => s == "b").Which.Should().Be("b");

    [TestMethod]
    public void WhichConstraint_PropagatesNull() {
        var which = new WhichConstraint<string?>(null);
        Assert.IsNull(which.Which);
    }

    [TestMethod]
    public void ExceptionAssertions_ExposesCaughtException() {
        Action action = () => throw new InvalidOperationException("inner");
        var assertions = action.Should().Throw<InvalidOperationException>();
        Assert.AreEqual("inner", assertions.Exception.Message);
    }

    [TestMethod]
    public void ExceptionAssertions_ExceptionProperty_NotNullAfterThrow() {
        Action action = () => throw new ArgumentException("boom");
        var assertions = action.Should().Throw<ArgumentException>();
        Assert.IsNotNull(assertions.Exception);
    }

    [TestMethod]
    public void AssertionFailedException_DefaultConstructor() {
        var ex = new AssertionFailedException();
        Assert.IsNotNull(ex.Message);
    }

    [TestMethod]
    public void AssertionFailedException_MessageConstructor() {
        var ex = new AssertionFailedException("failure");
        Assert.AreEqual("failure", ex.Message);
        Assert.IsNull(ex.InnerException);
    }

    [TestMethod]
    public void AssertionFailedException_MessageAndInnerConstructor() {
        var inner = new InvalidOperationException("cause");
        var ex = new AssertionFailedException("failure", inner);
        Assert.AreEqual("failure", ex.Message);
        Assert.AreSame(inner, ex.InnerException);
    }

    [TestMethod]
    public void AssertionFailedException_IsSerializable() {
        var attribute = typeof(AssertionFailedException).GetCustomAttributes(typeof(SerializableAttribute), false);
        Assert.HasCount(1, attribute);
    }

    [TestMethod]
    public void AssertionFailedException_DerivesFromException() => Assert.IsTrue(typeof(Exception).IsAssignableFrom(typeof(AssertionFailedException)));
}
