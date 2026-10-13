//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class FailureMessageTests {
    [TestMethod]
    public void BeTrue_UsesCustomMessage() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => false.Should().BeTrue("custom bool message"));
        Assert.AreEqual("Expected false to be true, but found false because custom bool message.", ex.Message);
    }

    [TestMethod]
    public void BeFalse_UsesCustomMessage() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => true.Should().BeFalse("custom false message"));
        Assert.AreEqual("Expected true to be false, but found true because custom false message.", ex.Message);
    }

    [TestMethod]
    public void StringBe_UsesCustomMessage() {
        const string subject = "a";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be("b", "custom string message"));
        Assert.AreEqual("Expected subject to be 'b', but found 'a' because custom string message.", ex.Message);
    }

    [TestMethod]
    public void StringNull_Contain_UsesCustomMessage() {
        const string? subject = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Contain("x", "custom null message"));
        Assert.AreEqual("Expected subject to contain 'x', but it was null because custom null message.", ex.Message);
    }

    [TestMethod]
    public void ObjectBe_UsesCustomMessage() {
        object subject = 1;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().Be(2, "custom object message"));
        Assert.AreEqual("Expected subject to be '2', but got '1' because custom object message.", ex.Message);
    }

    [TestMethod]
    public void ObjectBeNull_UsesCustomMessage() {
        object subject = new();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeNull("custom object null message"));
        Assert.AreEqual("Expected subject to be null, but it was not null because custom object null message.", ex.Message);
    }

    [TestMethod]
    public void CollectionHaveCount_UsesCustomMessage() {
        int[] values = [1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCount(2, "custom count message"));
        Assert.AreEqual("Expected values to contain 2 item(s), but found 1 because custom count message.", ex.Message);
    }

    [TestMethod]
    public void CollectionNull_UsesCustomMessage() {
        IEnumerable<int>? subject = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().HaveCount(0, "custom collection null message"));
        Assert.AreEqual("Expected subject to contain 0 item(s), but collection is null because custom collection null message.", ex.Message);
    }

    [TestMethod]
    public void BeEquivalentTo_UsesCustomMessage() {
        int[] values = [1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeEquivalentTo((IEnumerable<int>)[2], "custom equivalent message"));
        Assert.AreEqual("Expected values to be equivalent, but item '2' was found 0 time(s); expected 1 because custom equivalent message.", ex.Message);
    }

    [TestMethod]
    public void ContainSingle_UsesCustomMessage() {
        int[] values = [1, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().ContainSingle(because: "custom single message"));
        Assert.AreEqual("Expected values to contain exactly one item, but found 2 because custom single message.", ex.Message);
    }

    [TestMethod]
    public void Throw_UsesCustomMessage() {
        Action action = () => { };

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>("custom throw message"));
        Assert.AreEqual("Expected action to throw InvalidOperationException, but no exception was thrown because custom throw message.", ex.Message);
    }

    [TestMethod]
    public void Throw_UsesCustomMessageWithInnerException() {
        Action action = () => throw new ArgumentException("boom");
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => action.Should().Throw<InvalidOperationException>("custom throw message"));
        Assert.AreEqual("Expected action to throw InvalidOperationException, but ArgumentException was thrown because custom throw message.", ex.Message);
        Assert.IsInstanceOfType<ArgumentException>(ex.InnerException);
    }

    [TestMethod]
    public void Throw_NoMessage_DefaultReused() {
        Action action = () => { };

        var first = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>());
        var second = Assert.ThrowsExactly<AssertionFailedException>(() => action.Should().Throw<InvalidOperationException>());
        Assert.AreEqual(first.Message, second.Message);
    }

    [TestMethod]
    public void Be_FailureMessage_ContainsActualAndExpected() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => ((object)"actual").Should().Be("expected"));
        Assert.Contains("'expected'", ex.Message);
        Assert.Contains("'actual'", ex.Message);
    }
}
