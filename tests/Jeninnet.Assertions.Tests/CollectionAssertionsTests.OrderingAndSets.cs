//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class CollectionAssertionsTests {
    [TestMethod]
    public void NotBeSubsetOf_PassForNonSubset() {
        int[] values = [1, 2, 3];
        values.Should().NotBeSubsetOf([1, 2]);
    }

    [TestMethod]
    public void NotBeSubsetOf_FailsForSubset() {
        int[] values = [1, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotBeSubsetOf([1, 2, 3]));
        Assert.AreEqual("Expected values not to be a subset of ['1', '2', '3'], but found 1 in the superset.", ex.Message);
    }

    [TestMethod]
    public void NotContainInOrder_PassForOutOfOrder() {
        int[] values = [3, 2, 1];
        values.Should().NotContainInOrder([1, 2]);
    }

    [TestMethod]
    public void NotContainInOrder_FailsForInOrder() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotContainInOrder([1, 2]));
        Assert.AreEqual("Expected values not to contain ['1', '2'] in order, but found it.", ex.Message);
    }

    [TestMethod]
    public void NotContainInConsecutiveOrder_PassForNonConsecutive() {
        int[] values = [1, 3, 2];
        values.Should().NotContainInConsecutiveOrder([1, 2]);
    }

    [TestMethod]
    public void NotContainInConsecutiveOrder_FailsForConsecutive() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotContainInConsecutiveOrder([1, 2]));
        Assert.AreEqual("Expected values not to contain ['1', '2'] in consecutive order, but found it.", ex.Message);
    }

    [TestMethod]
    public void NotBeInAscendingOrder_PassForDescending() {
        int[] values = [3, 2, 1];
        values.Should().NotBeInAscendingOrder(Comparer<int>.Default);
    }

    [TestMethod]
    public void NotBeInAscendingOrder_FailsForAscending() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotBeInAscendingOrder(Comparer<int>.Default));
        Assert.AreEqual("Expected values not to be in ascending order, but found 1 after 3 at index 0.", ex.Message);
    }

    [TestMethod]
    public void NotBeInDescendingOrder_PassForAscending() {
        int[] values = [1, 2, 3];
        values.Should().NotBeInDescendingOrder(Comparer<int>.Default);
    }

    [TestMethod]
    public void NotBeInDescendingOrder_FailsForDescending() {
        int[] values = [3, 2, 1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotBeInDescendingOrder(Comparer<int>.Default));
        Assert.AreEqual("Expected values not to be in descending order, but found 3 after 1 at index 0.", ex.Message);
    }

    [TestMethod]
    public void NotBeInAscendingOrder_ParameterlessUsesDefaultComparer() {
        int[] values = [3, 2, 1];
        values.Should().NotBeInAscendingOrder();

        int[] ascending = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => ascending.Should().NotBeInAscendingOrder());
        Assert.AreEqual("Expected ascending not to be in ascending order, but found 1 after 3 at index 0.", ex.Message);
    }

    [TestMethod]
    public void NotBeInDescendingOrder_ParameterlessUsesDefaultComparer() {
        int[] values = [1, 2, 3];
        values.Should().NotBeInDescendingOrder();

        int[] descending = [3, 2, 1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => descending.Should().NotBeInDescendingOrder());
        Assert.AreEqual("Expected descending not to be in descending order, but found 3 after 1 at index 0.", ex.Message);
    }

    [TestMethod]
    public void ThenBeIn_ChainsSecondaryKeys() {
        var boxes = new[] { new { Size = 1 }, new { Size = 1 }, new { Size = 2 } };
        boxes.Should().BeInAscendingOrder(static b => b.Size).And.ThenBeInAscendingOrder(static b => b.Size);

        var shuffled = new[] { new { Size = 2 }, new { Size = 1 }, new { Size = 1 } };
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => shuffled.Should().BeInAscendingOrder(static b => b.Size).And.ThenBeInAscendingOrder(static b => b.Size));
        Assert.AreEqual("Expected shuffled to be in ascending order, but found '{ Size = 1 }' after '{ Size = 2 }' at index 1.", ex.Message);

        var pairs = new[] { (1, "a"), (1, "b"), (2, "c") };
        pairs.Should().BeInAscendingOrder(static p => p.Item1).And.ThenBeInAscendingOrder(static p => p.Item2);

        var badPairs = new[] { (1, "a"), (1, "b") };
        var thenEx = Assert.ThrowsExactly<AssertionFailedException>(
            () => badPairs.Should().BeInAscendingOrder(static p => p.Item1).And.ThenBeInDescendingOrder(static p => p.Item2));
        Assert.AreEqual("Expected badPairs to then be in descending order, but found '(1, b)' after '(1, a)' at index 1.", thenEx.Message);
    }

    [TestMethod]
    public void SetRelations_PassAndFail() {
        int[] values = [1, 2, 3];
        values.Should().BeSupersetOf([2, 3]);
        values.Should().BeSupersetOf([]);
        values.Should().BeProperSubsetOf([1, 2, 3, 4]);
        values.Should().BeProperSupersetOf([1, 2]);

        var missing = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeSupersetOf([2, 9]));
        Assert.AreEqual("Expected values to be a superset of ['2', '9'], but '9' was missing from it.", missing.Message);

        var extended = new[] { 1, 9 };
        var outsider = Assert.ThrowsExactly<AssertionFailedException>(() => extended.Should().BeProperSubsetOf([1, 2, 3]));
        Assert.Contains("outside of it", outsider.Message);

        var sameSub = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeProperSubsetOf([1, 2, 3]));
        Assert.AreEqual("Expected values to be a proper subset of ['1', '2', '3'], but they contain the same items.", sameSub.Message);

        var sameSuper = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeProperSupersetOf([1, 2, 3]));
        Assert.AreEqual("Expected values to be a proper superset of ['1', '2', '3'], but they contain the same items.", sameSuper.Message);

        var emptyProper = Assert.ThrowsExactly<AssertionFailedException>(() => Array.Empty<int>().Should().BeProperSupersetOf([]));
        Assert.AreEqual("Expected Array.Empty<int>() to be a proper superset of [], but they contain the same items.", emptyProper.Message);
    }
}
