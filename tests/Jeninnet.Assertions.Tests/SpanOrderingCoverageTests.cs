//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class SpanOrderingCoverageTests {
    [TestMethod]
    public void Span_OrderingOverloads_Pass() {
        int[] ascending = [1, 2, 3];
        int[] descending = [3, 2, 1];
        int[] unordered = [2, 1, 4, 3];

        ascending.AsSpan().AsAssertions().BeInAscendingOrder();
        ascending.AsSpan().AsAssertions().BeInAscendingOrder(Comparer<int>.Default);
        ascending.AsSpan().AsAssertions().BeInAscendingOrder(static (left, right) => left.CompareTo(right));
        ascending.AsSpan().AsAssertions().BeInAscendingOrder(static x => x);
        ascending.AsSpan().AsAssertions().BeInAscendingOrder(static x => x, Comparer<int>.Default);
        descending.AsSpan().AsAssertions().BeInDescendingOrder();
        descending.AsSpan().AsAssertions().BeInDescendingOrder(Comparer<int>.Default);
        descending.AsSpan().AsAssertions().BeInDescendingOrder(static (left, right) => left.CompareTo(right));
        descending.AsSpan().AsAssertions().BeInDescendingOrder(static x => x);
        descending.AsSpan().AsAssertions().BeInDescendingOrder(static x => x, Comparer<int>.Default);

        unordered.AsSpan().AsAssertions().NotBeInAscendingOrder(static (left, right) => left.CompareTo(right));
        unordered.AsSpan().AsAssertions().NotBeInAscendingOrder(static x => x);
        unordered.AsSpan().AsAssertions().NotBeInAscendingOrder(static x => x, Comparer<int>.Default);
        unordered.AsSpan().AsAssertions().NotBeInDescendingOrder(static (left, right) => left.CompareTo(right));
        unordered.AsSpan().AsAssertions().NotBeInDescendingOrder(static x => x);
        unordered.AsSpan().AsAssertions().NotBeInDescendingOrder(static x => x, Comparer<int>.Default);

        Assert.HasCount(3, ascending);
        Assert.HasCount(3, descending);
        Assert.HasCount(4, unordered);
    }

    [TestMethod]
    public void Span_ContainmentOrderOverloads_Pass() {
        int[] values = [1, 2, 3, 4];

        values.AsSpan().AsAssertions().ContainInOrder([1, 3]);
        values.AsSpan().AsAssertions().NotContainInOrder([4, 1]);
        values.AsSpan().AsAssertions().ContainInConsecutiveOrder([2, 3]);
        values.AsSpan().AsAssertions().NotContainInConsecutiveOrder([1, 3]);

        Assert.HasCount(4, values);
    }

    [TestMethod]
    public void Span_NegativeOrdering_FailsForOrderedContent() {
        int[] ascending = [1, 2, 3];
        int[] descending = [3, 2, 1];

        Assert.ThrowsExactly<AssertionFailedException>(() => ascending.AsSpan().AsAssertions().NotBeInAscendingOrder(static x => x));
        Assert.ThrowsExactly<AssertionFailedException>(() => ascending.AsSpan().AsAssertions().NotBeInAscendingOrder(static (left, right) => left.CompareTo(right)));
        Assert.ThrowsExactly<AssertionFailedException>(() => ascending.AsSpan().AsAssertions().NotBeInAscendingOrder(static x => x, Comparer<int>.Default));
        Assert.ThrowsExactly<AssertionFailedException>(() => descending.AsSpan().AsAssertions().NotBeInDescendingOrder(static x => x));
        Assert.ThrowsExactly<AssertionFailedException>(() => descending.AsSpan().AsAssertions().NotBeInDescendingOrder(static (left, right) => left.CompareTo(right)));
        Assert.ThrowsExactly<AssertionFailedException>(() => descending.AsSpan().AsAssertions().NotBeInDescendingOrder(static x => x, Comparer<int>.Default));
    }
}
