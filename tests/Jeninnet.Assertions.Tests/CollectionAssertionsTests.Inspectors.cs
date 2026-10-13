//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class CollectionAssertionsTests {
    [TestMethod]
    public void SatisfyRespectively_PassFullMatch() {
        int[] values = [2, 4];
        values.Should().SatisfyRespectively(x => x.Should().Be(2), x => x.Should().Be(4));
    }

    [TestMethod]
    public void SatisfyRespectively_FailsAggregatedWithIndexes() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().SatisfyRespectively(
            x => x.Should().Be(1),
            x => x.Should().Be(99),
            x => x.Should().Be(88)));
        Assert.AreEqual("Expected values to satisfy 3 inspector(s) respectively, but 2 inspection(s) failed:\n2: Expected x to be 99, but found 2.\n3: Expected x to be 88, but found 3.", ex.Message);
    }

    [TestMethod]
    public void SatisfyRespectively_FewerInspectorsCheckFirstElements() {
        int[] values = [1, 2, 3];
        values.Should().SatisfyRespectively(x => x.Should().Be(1));
    }

    [TestMethod]
    public void SatisfyRespectively_TooManyInspectorsFailsWithCount() {
        int[] values = [1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().SatisfyRespectively(x => x.Should().Be(1), x => x.Should().Be(2)));
        Assert.AreEqual("Expected values to satisfy 2 inspector(s) respectively, but found 1 item(s).", ex.Message);
    }

    [TestMethod]
    public void SatisfyRespectively_NullOrEmptyGuards() {
        int[] values = [1];
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().SatisfyRespectively((Action<int>[])null!));
        Assert.ThrowsExactly<ArgumentException>(() => values.Should().SatisfyRespectively());
        Assert.ThrowsExactly<ArgumentException>(() => values.Should().SatisfyRespectively(x => x.Should().Be(1), null!));

        int[]? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().SatisfyRespectively(x => x.Should().Be(1)));
        Assert.AreEqual("Expected missing to satisfy 1 inspector(s) respectively, but collection is null.", ex.Message);
    }

    [TestMethod]
    public void SatisfyRespectively_ValueReturningInspectorAggregated() {
        int[] values = [1, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().SatisfyRespectively(
            x => new[] { x }.Should().ContainSingle(v => v > 100),
            x => new[] { x }.Should().ContainSingle(v => v > 100)));
        Assert.AreEqual("Expected values to satisfy 2 inspector(s) respectively, but 2 inspection(s) failed:\n1: Expected new[] { x } to contain exactly one item, but found 0.\n2: Expected new[] { x } to contain exactly one item, but found 0.", ex.Message);
    }

    [TestMethod]
    public void SatisfyRespectively_InsideScopeRecordsSingleFailure() {
        int[] values = [1, 2];
        using var scope = new AssertionScope();
        values.Should().SatisfyRespectively(x => x.Should().Be(99), x => x.Should().Be(88));
        var discarded = scope.Discard();
        Assert.HasCount(1, discarded);
        Assert.Contains("2 inspection(s) failed:", discarded[0]);
    }

    [TestMethod]
    public void AllSatisfy_PassAndVacuousEmpty() {
        int[] values = [2, 4];
        values.Should().AllSatisfy(static x => x.Should().BeGreaterThan(0));

        int[] empty = [];
        empty.Should().AllSatisfy(static x => x.Should().BeGreaterThan(100));
    }

    [TestMethod]
    public void AllSatisfy_FailsAggregated() {
        int[] values = [5, -1, -2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().AllSatisfy(static x => x.Should().BeGreaterThan(0)));
        Assert.AreEqual("Expected values to satisfy the inspector for all 3 item(s), but 2 item(s) failed:\n2: Expected x to be greater than 0, but found -1.\n3: Expected x to be greater than 0, but found -2.", ex.Message);
    }

    [TestMethod]
    public void AllSatisfy_BecauseAppearsInReport() {
        int[] values = [1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().AllSatisfy(static x => x.Should().BeGreaterThan(100), "all large"));
        Assert.AreEqual("Expected values to satisfy the inspector for all 1 item(s), but 1 item(s) failed:\n1: Expected x to be greater than 100, but found 1 because all large.", ex.Message);
    }

    [TestMethod]
    public void AllSatisfy_NullGuards() {
        int[] values = [1];
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().AllSatisfy(null!));

        int[]? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().AllSatisfy(static x => x.Should().BeGreaterThan(0)));
        Assert.AreEqual("Expected missing to satisfy the inspector for all items, but collection is null.", ex.Message);
    }

    [TestMethod]
    public void Satisfy_PredicatesUnordered() {
        int[] values = [1, 2, 3];
        values.Should().Satisfy(x => x > 1, x => x < 3);
    }

    [TestMethod]
    public void Satisfy_UnmatchedPredicateFails() {
        int[] values = [1, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().Satisfy(x => x > 1, x => x > 100));
        Assert.AreEqual("Expected values to satisfy 2 predicate(s), but 1 matched no element:\n2: x => (x > 100)", ex.Message);
    }

    [TestMethod]
    public void Satisfy_NullOrEmptyGuards() {
        int[] values = [1];
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().Satisfy((Expression<Func<int, bool>>[])null!));
        Assert.ThrowsExactly<ArgumentException>(() => values.Should().Satisfy());

        int[]? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Satisfy(x => x > 0));
        Assert.AreEqual("Expected missing to satisfy 1 predicate(s), but collection is null.", ex.Message);
    }

    [TestMethod]
    public void AllBeOfType_PassAndWhich() {
        object[] items = ["a", "b"];
        items.Should().AllBeOfType<string>().Which.Should().Be("a");
    }

    [TestMethod]
    public void AllBeOfType_FailsWithIndexAndEmptyWhich() {
        object[] items = ["a", 1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => items.Should().AllBeOfType<string>());
        Assert.AreEqual("Expected items for all elements to be of type String, but found '1' at index 1.", ex.Message);

        object[] empty = [];
        empty.Should().AllBeOfType<string>().Which.Should().BeNull();

        object[]? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().AllBeOfType<string>());
        Assert.AreEqual("Expected missing for all elements to be of type String, but collection is null.", nullEx.Message);
    }

    [TestMethod]
    public void AllBeAssignableTo_PassAndWhich() {
        object[] items = ["a", 1];
        items.Should().AllBeAssignableTo<object>().Which.Should().Be("a");
    }

    [TestMethod]
    public void AllBeAssignableTo_FailsWithIndex() {
        object[] items = ["a", 1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => items.Should().AllBeAssignableTo<string>());
        Assert.AreEqual("Expected items for all elements to be assignable to String, but found '1' at index 1.", ex.Message);

        object[]? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().AllBeAssignableTo<string>());
        Assert.AreEqual("Expected missing for all elements to be assignable to String, but collection is null.", nullEx.Message);
    }

    [TestMethod]
    public void ProjectedUniquenessAndNulls_PassAndFail() {
        var items = new[] { new DeliberatelyEqualItem("a"), new DeliberatelyEqualItem("b") };
        items.Should().OnlyHaveUniqueItems(static item => item.Name);
        items.Should().NotContainNulls(static item => item.Name);

        var dupes = new[] { new DeliberatelyEqualItem("a"), new DeliberatelyEqualItem("a") };
        var dupeEx = Assert.ThrowsExactly<AssertionFailedException>(() => dupes.Should().OnlyHaveUniqueItems(static item => item.Name));
        Assert.AreEqual("Expected dupes to contain only unique items, but found duplicate 'a'.", dupeEx.Message);

        string?[] blanks = ["a", null];
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => blanks.Should().NotContainNulls(static s => s));
        Assert.AreEqual("Expected blanks to contain no null items, but found null at index 1.", nullEx.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => items.Should().OnlyHaveUniqueItems((Expression<Func<DeliberatelyEqualItem, string>>)null!));
    }
}
