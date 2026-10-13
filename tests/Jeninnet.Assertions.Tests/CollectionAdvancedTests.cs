//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class CollectionAdvancedTests {
    [TestMethod]
    public void CountBounds_Pass() {
        int[] values = [1, 2, 3];
        values.Should().HaveCountGreaterThan(2).And.HaveCountGreaterThanOrEqualTo(3)
            .And.HaveCountLessThan(4).And.HaveCountLessThanOrEqualTo(3);
    }

    [TestMethod]
    public void CountBounds_FailWithMessages() {
        int[] values = [1, 2, 3];
        var greater = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCountGreaterThan(5));
        Assert.AreEqual("Expected values to contain more than 5 item(s), but found 3.", greater.Message);

        var greaterOrEqual = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCountGreaterThanOrEqualTo(4));
        Assert.AreEqual("Expected values to contain at least 4 item(s), but found 3.", greaterOrEqual.Message);

        var less = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCountLessThan(3));
        Assert.AreEqual("Expected values to contain fewer than 3 item(s), but found 3.", less.Message);

        var lessOrEqual = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCountLessThanOrEqualTo(2));
        Assert.AreEqual("Expected values to contain at most 2 item(s), but found 3.", lessOrEqual.Message);
    }

    [TestMethod]
    public void CountBounds_FailForNull() {
        IEnumerable<int>? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveCountGreaterThan(2));
        Assert.AreEqual("Expected missing to contain more than 2 item(s), but collection is null.", ex.Message);
    }

    [TestMethod]
    public void HaveSameCount_PassForEqualCounts() {
        int[] values = [1, 2, 3];
        values.Should().HaveSameCount(["a", "b", "c"]);
    }

    [TestMethod]
    public void HaveSameCount_FailsReportingBothCounts() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveSameCount([4, 5]));
        Assert.AreEqual("Expected values to have the same count (2) as the expected collection, but found 3.", ex.Message);
    }

    [TestMethod]
    public void BeNull_FailsForNonNull() {
        int[] values = [1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeNull());
        Assert.AreEqual("Expected values to be null, but found 1 item(s).", ex.Message);
    }

    [TestMethod]
    public void NotBeNull_FailsForNull() {
        IEnumerable<int>? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNull());
        Assert.AreEqual("Expected missing not to be null, but found null.", ex.Message);
    }

    [TestMethod]
    public void OnlyContain_FailsNamingOffender() {
        int[] values = [1, 2, 3];
        values.Should().OnlyContain(static x => x > 0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().OnlyContain(static x => x > 1));
        Assert.AreEqual("Expected values to contain only matching items, but found '1'.", ex.Message);
    }

    [TestMethod]
    public void ContainInOrder_PassForSubsequence() {
        int[] values = [1, 2, 3, 4];
        values.Should().ContainInOrder(1, 3, 4);
    }

    [TestMethod]
    public void ContainInOrder_FailsNamingMissing() {
        int[] values = [1, 2, 3, 4];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().ContainInOrder(1, 4, 3));
        Assert.AreEqual("Expected values to contain '3' in order, but it was not found in sequence.", ex.Message);
    }

    [TestMethod]
    public void StartWith_PassForPrefix() {
        int[] values = [1, 2, 3];
        values.Should().StartWith(1, 2);
    }

    [TestMethod]
    public void StartWith_FailsForMismatchAndShortfall() {
        int[] values = [1, 2, 3];
        var mismatch = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().StartWith(1, 9));
        Assert.AreEqual("Expected values to start with ['1', '9'], but item at index 1 was '2'.", mismatch.Message);

        var shortfall = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().StartWith(1, 2, 3, 4));
        Assert.AreEqual("Expected values to start with ['1', '2', '3', '4'], but found only 3 item(s).", shortfall.Message);
    }

    [TestMethod]
    public void EndWith_PassForSuffix() {
        int[] values = [1, 2, 3];
        values.Should().EndWith(2, 3);
    }

    [TestMethod]
    public void EndWith_FailsForMismatch() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().EndWith(1, 3));
        Assert.AreEqual("Expected values to end with ['1', '3'], but item at index 1 was '2'.", ex.Message);
    }

    [TestMethod]
    public void BeInAscendingOrder_PassAndFail() {
        int[] values = [1, 2, 3];
        values.Should().BeInAscendingOrder();

        int[] shuffled = [1, 3, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => shuffled.Should().BeInAscendingOrder());
        Assert.AreEqual("Expected shuffled to be in ascending order, but found '2' after '3' at index 2.", ex.Message);
    }

    [TestMethod]
    public void BeInDescendingOrder_PassAndFail() {
        int[] values = [3, 2, 1];
        values.Should().BeInDescendingOrder();

        int[] shuffled = [1, 3, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => shuffled.Should().BeInDescendingOrder());
        Assert.AreEqual("Expected shuffled to be in descending order, but found '3' after '1' at index 1.", ex.Message);
    }

    [TestMethod]
    public void Ordering_WithComparer_IgnoresCase() {
        string[] values = ["a", "B"];
        values.Should().BeInAscendingOrder(StringComparer.OrdinalIgnoreCase);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeInAscendingOrder(StringComparer.Ordinal));
        Assert.AreEqual("Expected values to be in ascending order, but found 'B' after 'a' at index 1.", ex.Message);
    }

    [TestMethod]
    public void BeSubsetOf_PassAndFail() {
        int[] values = [1, 2];
        values.Should().BeSubsetOf([1, 2, 3]);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeSubsetOf([1, 3]));
        Assert.AreEqual("Expected values to be a subset of ['1', '3'], but found '2' outside of it.", ex.Message);
    }

    [TestMethod]
    public void IntersectWith_PassAndFail() {
        int[] values = [1, 2];
        values.Should().IntersectWith([2, 3]);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().IntersectWith([3, 4]));
        Assert.AreEqual("Expected values to intersect with ['3', '4'], but no common items were found.", ex.Message);
    }

    [TestMethod]
    public void Equal_PassForSameOrder() {
        int[] values = [1, 2, 3];
        values.Should().Equal(1, 2, 3);
    }

    [TestMethod]
    public void Equal_FailsForOrderAndCount() {
        int[] values = [1, 2, 3];
        var order = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().Equal(1, 3, 2));
        Assert.AreEqual("Expected values to equal ['1', '3', '2'], but item at index 1 was '2' instead of '3'.", order.Message);

        var count = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().Equal(1, 2));
        Assert.AreEqual("Expected values to equal ['1', '2'], but found 3 item(s) instead of 2.", count.Message);
    }

    [TestMethod]
    public void BeEquivalentTo_StrictOrdering_RespectsOrder() {
        int[] values = [1, 2, 3];
        values.Should().BeEquivalentTo([1, 2, 3], static o => o.WithStrictOrdering());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeEquivalentTo([3, 2, 1], static o => o.WithStrictOrdering()));
        Assert.AreEqual("Expected values to be equivalent in the same order, but item at index 0 was '1' instead of '3'.", ex.Message);
    }

    [TestMethod]
    public void BeEquivalentTo_IgnoringCase_WorksForStrings() {
        string[] values = ["a", "b"];
        values.Should().BeEquivalentTo(["A", "B"], static o => o.IgnoringCase());
        values.Should().BeEquivalentTo(["A", "B"], static o => o.Using(StringComparer.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void BeEquivalentTo_IgnoringCase_ThrowsForNonStrings() {
        int[] values = [1];
        Assert.ThrowsExactly<InvalidOperationException>(() => values.Should().BeEquivalentTo([1], static o => o.IgnoringCase()));
    }

    [TestMethod]
    public void NotIntersectWith_PassAndFail() {
        int[] values = [1, 2];
        values.Should().NotIntersectWith([3, 4]);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotIntersectWith([2, 3]));
        Assert.AreEqual("Expected values to not intersect with ['2', '3'], but found shared item '2'.", ex.Message);
    }

    [TestMethod]
    public void NotIntersectWith_FailsForNull() {
        IEnumerable<int>? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotIntersectWith([1]));
        Assert.AreEqual("Expected missing to not intersect with ['1'], but collection is null.", ex.Message);
    }

    [TestMethod]
    public void NotIntersectWith_ThrowsForNullExpected() {
        int[] values = [1, 2];
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().NotIntersectWith(null!));
    }

    [TestMethod]
    public void OnlyHaveUniqueItems_PassAndFail() {
        int[] values = [1, 2, 3];
        values.Should().OnlyHaveUniqueItems();

        int[] repeated = [1, 2, 1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => repeated.Should().OnlyHaveUniqueItems());
        Assert.AreEqual("Expected repeated to contain only unique items, but found duplicate '1'.", ex.Message);
    }

    [TestMethod]
    public void OnlyHaveUniqueItems_FailsForNull() {
        IEnumerable<int>? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().OnlyHaveUniqueItems());
        Assert.AreEqual("Expected missing to contain only unique items, but collection is null.", ex.Message);
    }

    [TestMethod]
    public void NotContainNulls_PassAndFail() {
        string[] values = ["a", "b"];
        values.Should().NotContainNulls();

        string?[] withNull = ["a", null, "c"];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => withNull.Should().NotContainNulls());
        Assert.AreEqual("Expected withNull to contain no null items, but found null at index 1.", ex.Message);
    }

    [TestMethod]
    public void NotContainNulls_FailsForNull() {
        IEnumerable<string>? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotContainNulls());
        Assert.AreEqual("Expected missing to contain no null items, but collection is null.", ex.Message);
    }

    [TestMethod]
    public void ContainInConsecutiveOrder_PassAndFail() {
        int[] values = [1, 2, 3, 4];
        values.Should().ContainInConsecutiveOrder(2, 3);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().ContainInConsecutiveOrder(3, 2));
        Assert.AreEqual("Expected values to contain ['3', '2'] in consecutive order, but the sequence was not found.", ex.Message);
    }

    [TestMethod]
    public void ContainInConsecutiveOrder_FailsForNull() {
        IEnumerable<int>? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainInConsecutiveOrder(2));
        Assert.AreEqual("Expected missing to contain ['2'] in consecutive order, but collection is null.", ex.Message);
    }

    [TestMethod]
    public void HaveElementAt_PassAndFail() {
        int[] values = [10, 20, 30];
        values.Should().HaveElementAt(1, 20);

        var mismatch = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveElementAt(0, 20));
        Assert.AreEqual("Expected values to have '20' at index 0, but found '10'.", mismatch.Message);

        var missing = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveElementAt(5, 99));
        Assert.AreEqual("Expected values to have '99' at index 5, but found only 3 item(s).", missing.Message);
    }

    [TestMethod]
    public void HaveElementAt_FailsForNull() {
        IEnumerable<int>? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveElementAt(0, 1));
        Assert.AreEqual("Expected missing to have '1' at index 0, but collection is null.", ex.Message);
    }

    [TestMethod]
    public void HaveElementAt_ThrowsForNegativeIndex() {
        int[] values = [10, 20, 30];
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => values.Should().HaveElementAt(-1, 10));
    }

    [TestMethod]
    public void AsCollection_ElementExclusion_AppliesToEveryItem() {
        Order[] actual = [new("pen", 10), new("book", 5)];
        actual.Should().BeEquivalentTo(
            [new("pen", 999), new("book", 999)],
            static o => o.Excluding(x => x.Amount).AsCollection());
    }

    [TestMethod]
    public void AsCollection_ElementExclusion_ScopesToElementMember() {
        Order[] actual = [new("pen", 10)];
        actual.Should().BeEquivalentTo(
            [new("pen", 999)],
            static o => o.Excluding(x => x.Amount).AsCollection());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(
            [new("pen", 999)],
            static o => o.AsCollection()));
        Assert.Contains("no equivalent item", ex.Message);
    }

    [TestMethod]
    public void AsCollection_Unordered_MatchesAsMultiset() {
        Order[] actual = [new("pen", 10), new("book", 5)];
        actual.Should().BeEquivalentTo([new("book", 5), new("pen", 10)], static o => o.AsCollection());
    }

    [TestMethod]
    public void AsCollection_WithStrictOrdering_RespectsOrder() {
        Order[] actual = [new("pen", 10), new("book", 5)];
        actual.Should().BeEquivalentTo([new("pen", 10), new("book", 5)], static o => o.WithStrictOrdering().AsCollection());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(
            [new("book", 5), new("pen", 10)],
            static o => o.WithStrictOrdering().AsCollection()));
        Assert.Contains("difference", ex.Message);
    }

    [TestMethod]
    public void AsCollection_ElementOrderingRule_ScopesNestedCollections() {
        Shelf[] actual = [new() { Books = ["a", "b"] }, new() { Books = ["a", "b"] }];
        Shelf[] outOfOrder = [new() { Books = ["b", "a"] }, new() { Books = ["a", "b"] }];
        actual.Should().BeEquivalentTo(outOfOrder, static o => o.AsCollection());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeEquivalentTo(
            outOfOrder,
            static o => o.WithStrictOrderingFor(x => x.Books).AsCollection()));
        Assert.Contains("no equivalent item", ex.Message);
    }

    [TestMethod]
    public void AsCollection_ThrowsForNullConfig() {
        Order[] actual = [new("pen", 10)];
        Assert.ThrowsExactly<ArgumentNullException>(() => actual.Should().BeEquivalentTo(
            [new("pen", 10)],
            (Func<Equivalency.SelfReferenceEquivalencyOptions<Order>, Equivalency.SelfReferenceEquivalencyOptions<IEnumerable<Order>>>)null!));
    }

    private sealed class Order(string product, decimal amount) {
        public string Product { get; } = product;

        public decimal Amount { get; } = amount;
    }

    private sealed class Shelf {
        public required List<string> Books { get; init; }
    }
}
