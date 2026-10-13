//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class CollectionOrderingCoverageTests {
    [TestMethod]
    public void Ordering_NullSubject_ReportsEveryVerb() {
        IEnumerable<int>? missing = null;
        using var scope = new AssertionScope();

        missing.Should().ContainInOrder(1);
        missing.Should().BeInAscendingOrder(Comparer<int>.Default);
        missing.Should().BeInAscendingOrder((left, right) => left.CompareTo(right));
        missing.Should().BeInAscendingOrder(static x => x, Comparer<int>.Default);
        missing.Should().BeInDescendingOrder(Comparer<int>.Default);
        missing.Should().BeInDescendingOrder((left, right) => left.CompareTo(right));
        missing.Should().BeInDescendingOrder(static x => x, Comparer<int>.Default);
        missing.Should().ContainInConsecutiveOrder(1, 2);
        missing.Should().NotContainInOrder([1, 2]);
        missing.Should().NotContainInConsecutiveOrder([1, 2]);
        missing.Should().NotBeInAscendingOrder(Comparer<int>.Default);
        missing.Should().NotBeInAscendingOrder((left, right) => left.CompareTo(right));
        missing.Should().NotBeInAscendingOrder(static x => x, Comparer<int>.Default);
        missing.Should().NotBeInDescendingOrder(Comparer<int>.Default);
        missing.Should().NotBeInDescendingOrder((left, right) => left.CompareTo(right));
        missing.Should().NotBeInDescendingOrder(static x => x, Comparer<int>.Default);

        var failures = scope.Discard();

        Assert.HasCount(16, failures);
        Assert.AreEqual("Expected missing to contain items in order, but collection is null.", failures[0]);
        Assert.AreEqual("Expected missing to be in ascending order, but collection is null.", failures[1]);
        Assert.AreEqual("Expected missing to be in ascending order, but collection is null.", failures[2]);
        Assert.AreEqual("Expected missing to be in ascending order, but collection is null.", failures[3]);
        Assert.AreEqual("Expected missing to be in descending order, but collection is null.", failures[4]);
        Assert.AreEqual("Expected missing to be in descending order, but collection is null.", failures[5]);
        Assert.AreEqual("Expected missing to be in descending order, but collection is null.", failures[6]);
        Assert.AreEqual("Expected missing to contain ['1', '2'] in consecutive order, but collection is null.", failures[7]);
        Assert.AreEqual("Expected missing not to contain ['1', '2'] in order, but collection is null.", failures[8]);
        Assert.AreEqual("Expected missing not to contain ['1', '2'] in consecutive order, but collection is null.", failures[9]);
        Assert.AreEqual("Expected missing not to be in ascending order, but collection is null.", failures[10]);
        Assert.AreEqual("Expected missing not to be in ascending order, but collection is null.", failures[11]);
        Assert.AreEqual("Expected missing not to be in ascending order, but collection is null.", failures[12]);
        Assert.AreEqual("Expected missing not to be in descending order, but collection is null.", failures[13]);
        Assert.AreEqual("Expected missing not to be in descending order, but collection is null.", failures[14]);
        Assert.AreEqual("Expected missing not to be in descending order, but collection is null.", failures[15]);
    }

    [TestMethod]
    public void Ordering_Guards_ThrowForNullArguments() {
        int[] values = [1, 2, 3];

        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().ContainInOrder((IEnumerable<int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().BeInAscendingOrder((IComparer<int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().BeInAscendingOrder((Func<int, int, int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().BeInAscendingOrder((Expression<Func<int, int>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().BeInAscendingOrder((Expression<Func<int, int>>)null!, Comparer<int>.Default));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().BeInDescendingOrder((IComparer<int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().BeInDescendingOrder((Func<int, int, int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().BeInDescendingOrder((Expression<Func<int, int>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().BeInDescendingOrder((Expression<Func<int, int>>)null!, Comparer<int>.Default));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().ContainInConsecutiveOrder((IEnumerable<int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().NotContainInOrder((IEnumerable<int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().NotContainInConsecutiveOrder((IEnumerable<int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().NotBeInAscendingOrder((IComparer<int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().NotBeInAscendingOrder((Func<int, int, int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().NotBeInAscendingOrder((Expression<Func<int, int>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().NotBeInAscendingOrder((Expression<Func<int, int>>)null!, Comparer<int>.Default));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().NotBeInDescendingOrder((IComparer<int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().NotBeInDescendingOrder((Func<int, int, int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().NotBeInDescendingOrder((Expression<Func<int, int>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().NotBeInDescendingOrder((Expression<Func<int, int>>)null!, Comparer<int>.Default));
    }

    [TestMethod]
    public void Ordering_ComparisonAndSelectorOverloads_PassAndFail() {
        int[] ascending = [1, 2, 3];
        int[] descending = [3, 2, 1];

        ascending.Should().BeInAscendingOrder((left, right) => left.CompareTo(right));
        ascending.Should().BeInAscendingOrder(static x => x, Comparer<int>.Default);
        descending.Should().BeInDescendingOrder((left, right) => left.CompareTo(right));
        descending.Should().BeInDescendingOrder(static x => x, Comparer<int>.Default);
        ascending.Should().BeInDescendingOrder(static x => -x);
        descending.Should().BeInAscendingOrder(static x => -x);
        ascending.Should().ContainInOrder(1, 3);

        var ascendingByComparison = Assert.ThrowsExactly<AssertionFailedException>(() => descending.Should().BeInAscendingOrder((left, right) => left.CompareTo(right)));
        Assert.AreEqual("Expected descending to be in ascending order, but found '2' after '3' at index 1.", ascendingByComparison.Message);

        var descendingBySelector = Assert.ThrowsExactly<AssertionFailedException>(() => ascending.Should().BeInDescendingOrder(static x => x, Comparer<int>.Default));
        Assert.AreEqual("Expected ascending to be in descending order, but found '2' after '1' at index 1.", descendingBySelector.Message);
    }

    [TestMethod]
    public void Ordering_OrderedCollections_FailNegativeVerbs() {
        int[] values = [1, 2, 3];
        int[] descending = [3, 2, 1];
        using var scope = new AssertionScope();

        values.Should().ContainInOrder(9);
        values.Should().ContainInConsecutiveOrder(2, 9);
        values.Should().NotContainInOrder([1, 3]);
        values.Should().NotContainInConsecutiveOrder([2, 3]);
        values.Should().NotBeInAscendingOrder(Comparer<int>.Default);
        values.Should().NotBeInAscendingOrder((left, right) => left.CompareTo(right));
        values.Should().NotBeInAscendingOrder(static x => x);
        values.Should().NotBeInAscendingOrder(static x => x, Comparer<int>.Default);
        descending.Should().NotBeInDescendingOrder(Comparer<int>.Default);
        descending.Should().NotBeInDescendingOrder((left, right) => left.CompareTo(right));
        descending.Should().NotBeInDescendingOrder(static x => x);
        descending.Should().NotBeInDescendingOrder(static x => x, Comparer<int>.Default);

        var failures = scope.Discard();

        Assert.HasCount(12, failures);
        Assert.AreEqual("Expected values to contain '9' in order, but it was not found in sequence.", failures[0]);
        Assert.AreEqual("Expected values to contain ['2', '9'] in consecutive order, but the sequence was not found.", failures[1]);
        Assert.AreEqual("Expected values not to contain ['1', '3'] in order, but found it.", failures[2]);
        Assert.AreEqual("Expected values not to contain ['2', '3'] in consecutive order, but found it.", failures[3]);
        Assert.AreEqual("Expected values not to be in ascending order, but found 1 after 3 at index 0.", failures[4]);
        Assert.AreEqual("Expected values not to be in ascending order, but found 1 after 3 at index 0.", failures[5]);
        Assert.AreEqual("Expected values not to be in ascending order, but found 1 after 3 at index 0.", failures[6]);
        Assert.AreEqual("Expected values not to be in ascending order, but found 1 after 3 at index 0.", failures[7]);
        Assert.AreEqual("Expected descending not to be in descending order, but found 3 after 1 at index 0.", failures[8]);
        Assert.AreEqual("Expected descending not to be in descending order, but found 3 after 1 at index 0.", failures[9]);
        Assert.AreEqual("Expected descending not to be in descending order, but found 3 after 1 at index 0.", failures[10]);
        Assert.AreEqual("Expected descending not to be in descending order, but found 3 after 1 at index 0.", failures[11]);
    }

    [TestMethod]
    public void Ordering_UnorderedCollections_PassNegativeVerbs() {
        int[] values = [2, 1, 4, 3];

        values.Should().NotContainInOrder([1, 2]);
        values.Should().NotContainInConsecutiveOrder([1, 2]);
        values.Should().NotBeInAscendingOrder((left, right) => left.CompareTo(right));
        values.Should().NotBeInAscendingOrder(static x => x);
        values.Should().NotBeInDescendingOrder((left, right) => left.CompareTo(right));
        values.Should().NotBeInDescendingOrder(static x => x);
    }
}
