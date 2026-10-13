//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class DictionaryOrderingCoverageTests {
    private static Dictionary<string, int> Ascending() => new() { ["a"] = 1, ["b"] = 2, ["c"] = 3 };

    private static Dictionary<string, int> Descending() => new() { ["c"] = 3, ["b"] = 2, ["a"] = 1 };

    [TestMethod]
    public void DefaultComparer_OnSingleEntry_Passes() {
        Dictionary<string, int> single = new() { ["a"] = 1 };

        single.Should().BeInAscendingOrder();
        single.Should().BeInDescendingOrder();
    }

    [TestMethod]
    public void Ordering_NullDictionary_ReportsEveryVerb() {
        Dictionary<string, int>? missing = null;
        using var scope = new AssertionScope();

        missing.Should().BeInAscendingOrder(Comparer<KeyValuePair<string, int>>.Default);
        missing.Should().BeInAscendingOrder(static (left, right) => string.CompareOrdinal(left.Key, right.Key));
        missing.Should().BeInAscendingOrder(static pair => pair.Key, StringComparer.Ordinal);
        missing.Should().BeInDescendingOrder(Comparer<KeyValuePair<string, int>>.Default);
        missing.Should().BeInDescendingOrder(static (left, right) => string.CompareOrdinal(left.Key, right.Key));
        missing.Should().BeInDescendingOrder(static pair => pair.Key, StringComparer.Ordinal);
        missing.Should().NotBeInAscendingOrder(static (left, right) => string.CompareOrdinal(left.Key, right.Key));
        missing.Should().NotBeInAscendingOrder(static pair => pair.Key, StringComparer.Ordinal);
        missing.Should().NotBeInDescendingOrder(static (left, right) => string.CompareOrdinal(left.Key, right.Key));
        missing.Should().NotBeInDescendingOrder(static pair => pair.Key, StringComparer.Ordinal);

        var failures = scope.Discard();

        Assert.HasCount(10, failures);
        Assert.AreEqual("Expected missing to be in ascending order, but dictionary is null.", failures[0]);
        Assert.AreEqual("Expected missing to be in ascending order, but dictionary is null.", failures[1]);
        Assert.AreEqual("Expected missing to be in ascending order, but dictionary is null.", failures[2]);
        Assert.AreEqual("Expected missing to be in descending order, but dictionary is null.", failures[3]);
        Assert.AreEqual("Expected missing to be in descending order, but dictionary is null.", failures[4]);
        Assert.AreEqual("Expected missing to be in descending order, but dictionary is null.", failures[5]);
        Assert.AreEqual("Expected missing not to be in ascending order, but collection is null.", failures[6]);
        Assert.AreEqual("Expected missing not to be in ascending order, but collection is null.", failures[7]);
        Assert.AreEqual("Expected missing not to be in descending order, but collection is null.", failures[8]);
        Assert.AreEqual("Expected missing not to be in descending order, but collection is null.", failures[9]);
    }

    [TestMethod]
    public void Ordering_Guards_ThrowForNullArguments() {
        var dictionary = Ascending();

        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().BeInAscendingOrder((IComparer<KeyValuePair<string, int>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().BeInAscendingOrder((Func<KeyValuePair<string, int>, KeyValuePair<string, int>, int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().BeInAscendingOrder((Expression<Func<KeyValuePair<string, int>, string>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().BeInAscendingOrder((Expression<Func<KeyValuePair<string, int>, string>>)null!, StringComparer.Ordinal));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().BeInDescendingOrder((IComparer<KeyValuePair<string, int>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().BeInDescendingOrder((Func<KeyValuePair<string, int>, KeyValuePair<string, int>, int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().BeInDescendingOrder((Expression<Func<KeyValuePair<string, int>, string>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().BeInDescendingOrder((Expression<Func<KeyValuePair<string, int>, string>>)null!, StringComparer.Ordinal));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().NotBeInAscendingOrder((Func<KeyValuePair<string, int>, KeyValuePair<string, int>, int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().NotBeInAscendingOrder((Expression<Func<KeyValuePair<string, int>, string>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().NotBeInAscendingOrder((Expression<Func<KeyValuePair<string, int>, string>>)null!, StringComparer.Ordinal));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().NotBeInDescendingOrder((Func<KeyValuePair<string, int>, KeyValuePair<string, int>, int>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().NotBeInDescendingOrder((Expression<Func<KeyValuePair<string, int>, string>>)null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => dictionary.Should().NotBeInDescendingOrder((Expression<Func<KeyValuePair<string, int>, string>>)null!, StringComparer.Ordinal));
    }

    [TestMethod]
    public void Ordering_ComparisonAndSelectorOverloads_PassAndFail() {
        var ascending = Ascending();
        var descending = Descending();

        ascending.Should().BeInAscendingOrder(static (left, right) => string.CompareOrdinal(left.Key, right.Key));
        ascending.Should().BeInAscendingOrder(static pair => pair.Key);
        ascending.Should().BeInAscendingOrder(static pair => pair.Key, StringComparer.Ordinal);
        descending.Should().BeInDescendingOrder(static (left, right) => string.CompareOrdinal(left.Key, right.Key));
        descending.Should().BeInDescendingOrder(static pair => pair.Key);
        descending.Should().BeInDescendingOrder(static pair => pair.Key, StringComparer.Ordinal);
        ascending.Should().NotBeInDescendingOrder(static (left, right) => string.CompareOrdinal(left.Key, right.Key));
        ascending.Should().NotBeInDescendingOrder(static pair => pair.Key);
        ascending.Should().NotBeInDescendingOrder(static pair => pair.Key, StringComparer.Ordinal);
        descending.Should().NotBeInAscendingOrder(static (left, right) => string.CompareOrdinal(left.Key, right.Key));
        descending.Should().NotBeInAscendingOrder(static pair => pair.Key);
        descending.Should().NotBeInAscendingOrder(static pair => pair.Key, StringComparer.Ordinal);

        var ascendingByComparison = Assert.ThrowsExactly<AssertionFailedException>(
            () => descending.Should().BeInAscendingOrder(static (left, right) => string.CompareOrdinal(left.Key, right.Key)));
        Assert.AreEqual("Expected descending to be in ascending order, but found '[b, 2]' after '[c, 3]' at index 1.", ascendingByComparison.Message);

        var descendingBySelector = Assert.ThrowsExactly<AssertionFailedException>(
            () => ascending.Should().BeInDescendingOrder(static pair => pair.Key, StringComparer.Ordinal));
        Assert.AreEqual("Expected ascending to be in descending order, but found '[b, 2]' after '[a, 1]' at index 1.", descendingBySelector.Message);
    }

    [TestMethod]
    public void Ordering_OrderedDictionaries_FailNegativeVerbs() {
        var ascending = Ascending();
        var descending = Descending();
        using var scope = new AssertionScope();

        ascending.Should().NotBeInAscendingOrder(static (left, right) => string.CompareOrdinal(left.Key, right.Key));
        ascending.Should().NotBeInAscendingOrder(static pair => pair.Key);
        ascending.Should().NotBeInAscendingOrder(static pair => pair.Key, StringComparer.Ordinal);
        descending.Should().NotBeInDescendingOrder(static (left, right) => string.CompareOrdinal(left.Key, right.Key));
        descending.Should().NotBeInDescendingOrder(static pair => pair.Key);
        descending.Should().NotBeInDescendingOrder(static pair => pair.Key, StringComparer.Ordinal);

        var failures = scope.Discard();

        Assert.HasCount(6, failures);
        Assert.AreEqual("Expected ascending not to be in ascending order, but found '[a, 1]' after '[c, 3]' at index '0'.", failures[0]);
        Assert.AreEqual("Expected ascending not to be in ascending order, but found '[a, 1]' after '[c, 3]' at index '0'.", failures[1]);
        Assert.AreEqual("Expected ascending not to be in ascending order, but found '[a, 1]' after '[c, 3]' at index '0'.", failures[2]);
        Assert.AreEqual("Expected descending not to be in descending order, but found '[c, 3]' after '[a, 1]' at index '0'.", failures[3]);
        Assert.AreEqual("Expected descending not to be in descending order, but found '[c, 3]' after '[a, 1]' at index '0'.", failures[4]);
        Assert.AreEqual("Expected descending not to be in descending order, but found '[c, 3]' after '[a, 1]' at index '0'.", failures[5]);
    }
}
