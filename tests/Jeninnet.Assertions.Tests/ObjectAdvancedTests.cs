//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class ObjectAdvancedTests {
    [TestMethod]
    public void NotBe_PassForDifferentValue() {
        object value = 1;
        value.Should().NotBe(2);
    }

    [TestMethod]
    public void NotBe_FailsForEqualValue() {
        object value = 1;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().NotBe(1));
        Assert.AreEqual("Expected value not to be '1', but found '1'.", ex.Message);
    }

    [TestMethod]
    public void BeSameAs_PassForSameReference() {
        object value = new();
        value.Should().BeSameAs(value);
    }

    [TestMethod]
    public void BeSameAs_FailsForDifferentReference() {
        object first = new();
        object second = new();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => first.Should().BeSameAs(second));
        Assert.Contains("to be the same instance as", ex.Message);
        Assert.Contains("but found a different reference.", ex.Message);
    }

    [TestMethod]
    public void NotBeSameAs_FailsForSameReference() {
        object value = new();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().NotBeSameAs(value));
        Assert.Contains("not to be the same instance as", ex.Message);
    }

    [TestMethod]
    public void BeAssignableTo_PassForBaseType() {
        object subject = new MemoryStream();
        subject.Should().BeAssignableTo<Stream>();
    }

    [TestMethod]
    public void BeAssignableTo_FailsForUnrelatedAndNull() {
        object subject = "text";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().BeAssignableTo<MemoryStream>());
        Assert.AreEqual("Expected subject to be assignable to MemoryStream, but found String.", ex.Message);

        object? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeAssignableTo<Stream>());
        Assert.AreEqual("Expected missing to be assignable to Stream, but found null.", nullEx.Message);
    }

    [TestMethod]
    public void NotBeAssignableTo_FailsForAssignable() {
        object subject = new MemoryStream();
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeAssignableTo<Stream>());
        Assert.AreEqual("Expected subject not to be assignable to Stream, but found MemoryStream.", ex.Message);
    }

    [TestMethod]
    public void NotBeOfType_FailsForExactType() {
        object subject = "text";
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().NotBeOfType<string>());
        Assert.AreEqual("Expected subject not to be of type String, but found String.", ex.Message);
    }

    [TestMethod]
    public void Match_PassForSatisfiedPredicate() {
        object value = 5;
        value.Should().Match(static x => x.Equals(5));
    }

    [TestMethod]
    public void Match_FailsForUnsatisfiedPredicateAndNull() {
        object value = 5;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().Match(static x => x.Equals(6)));
        Assert.Contains("Expected value to match", ex.Message);
        Assert.Contains("but it did not.", ex.Message);

        object? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Match(static x => x != null));
        Assert.Contains("but it did not.", nullEx.Message);
    }

    [TestMethod]
    public void Match_NullPredicate_ThrowsArgumentNull() {
        object value = 5;
        Assert.ThrowsExactly<ArgumentNullException>(() => value.Should().Match(null!));
    }

    [TestMethod]
    public void SatisfyAllConditions_PassWhenAllPass() {
        var order = new Order(Total: 100, Items: 2);
        order.SatisfyAllConditions(
            static o => o.Total.Should().BePositive(),
            static o => o.Items.Should().Be(2));
    }

    [TestMethod]
    public void SatisfyAllConditions_AggregatesEveryFailure() {
        var order = new Order(Total: -5, Items: 2);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => order.SatisfyAllConditions(
            static o => o.Total.Should().BePositive(),
            static o => o.Items.Should().Be(3),
            static o => o.Total.Should().BeNegative()));
        Assert.AreEqual(
            "Expected value to satisfy all conditions, but the following failed:\n1. Expected o.Total to be positive, but found -5.\n2. Expected o.Items to be 3, but found 2.",
            ex.Message);
    }

    [TestMethod]
    public void SatisfyAllConditions_EmptySet_PassesVacuously() {
        var order = new Order(Total: 1, Items: 1);
        order.SatisfyAllConditions();
        order.Items.Should().Be(1);
    }

    [TestMethod]
    public void SatisfyAllConditions_NullSetOrNullCondition_ThrowsArgument() {
        var order = new Order(Total: 1, Items: 1);
        Assert.ThrowsExactly<ArgumentNullException>(() => order.SatisfyAllConditions((Action<Order>[])null!));
        Assert.ThrowsExactly<ArgumentException>(() => order.SatisfyAllConditions(static o => o.Total.Should().BePositive(), null!));
    }

    [TestMethod]
    public void SatisfyAllConditions_NonAssertionExceptions_Propagate() {
        var order = new Order(Total: 1, Items: 1);
        Assert.ThrowsExactly<InvalidOperationException>(() => order.SatisfyAllConditions(static _ => throw new InvalidOperationException()));
    }

    private sealed record Order(int Total, int Items);
}
