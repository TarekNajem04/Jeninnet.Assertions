//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class AndChainingTests {
    [TestMethod]
    public void BoolAssertions_And_ReturnsSameParent() {
        var parent = true.Should();
        Assert.AreSame(parent, parent.BeTrue().And);
    }

    [TestMethod]
    public void StringAssertions_SupportsChaining() => "hello".Should().Contain("ell").And.Be("hello").And.NotBeNull();

    [TestMethod]
    public void ObjectAssertions_SupportsChaining() {
        object value = 42;
        value.Should().NotBeNull().And.Be(42);
    }

    [TestMethod]
    public void CollectionAssertions_SupportsChaining() {
        int[] values = [1, 2, 3];
        values.Should().HaveCount(3).And.Contain(2).And.NotBeEmpty();
    }

    [TestMethod]
    public void NotThrow_ReturnsChainableConstraint() {
        Action action = () => { };

        action.Should().NotThrow().And.NotThrow();
    }

    [TestMethod]
    public void ChainedAssertion_ReportsSecondFailure() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCount(3).And.Contain(9));
        Assert.AreEqual("Expected values to contain '9', but it was not found.", ex.Message);
    }
}
