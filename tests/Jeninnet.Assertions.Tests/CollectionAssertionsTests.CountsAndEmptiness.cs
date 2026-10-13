//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class CollectionAssertionsTests {
    [TestMethod]
    public void HaveCount_PassForExactCount() {
        int[] values = [1, 2, 3];
        values.Should().HaveCount(3);
    }

    [TestMethod]
    public void HaveCount_FailsForDifferentCount() {
        int[] values = [1, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCount(3));
        Assert.AreEqual("Expected values to contain 3 item(s), but found 2.", ex.Message);
    }

    [TestMethod]
    public void HaveCount_FailsForNullCollection() {
        IEnumerable<int>? subject = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => subject.Should().HaveCount(0));
        Assert.AreEqual("Expected subject to contain 0 item(s), but collection is null.", ex.Message);
    }

    [TestMethod]
    public void BeEmpty_PassForEmpty() {
        int[] values = [];
        values.Should().BeEmpty();
    }

    [TestMethod]
    public void BeEmpty_FailsForNonEmpty() {
        int[] values = [1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeEmpty());
        Assert.AreEqual("Expected values to be empty, but found 1 item(s).", ex.Message);
    }

    [TestMethod]
    public void NotBeEmpty_PassForNonEmpty() {
        int[] values = [1];
        values.Should().NotBeEmpty();
    }

    [TestMethod]
    public void NotBeEmpty_FailsForEmpty() {
        int[] values = [];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotBeEmpty());
        Assert.AreEqual("Expected values not to be empty, but it was empty.", ex.Message);
    }

    [TestMethod]
    public void NotHaveCount_PassForDifferentCount() {
        int[] values = [1, 2, 3];
        values.Should().NotHaveCount(2);
    }

    [TestMethod]
    public void NotHaveCount_FailsForMatchingCount() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotHaveCount(3));
        Assert.AreEqual("Expected values not to contain 3 item(s), but found 3.", ex.Message);
    }

    [TestMethod]
    public void NotHaveSameCount_PassForDifferentCount() {
        int[] values = [1, 2, 3];
        values.Should().NotHaveSameCount([1, 2]);
    }

    [TestMethod]
    public void NotHaveSameCount_FailsForMatchingCount() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotHaveSameCount([4, 5, 6]));
        Assert.AreEqual("Expected values not to have the same count as 3 item(s), but found 3.", ex.Message);
    }

    [TestMethod]
    public void BeNullOrEmpty_PassAndFail() {
        int[]? missing = null;
        missing.Should().BeNullOrEmpty();
        Array.Empty<int>().Should().BeNullOrEmpty();

        int[] values = [1, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeNullOrEmpty());
        Assert.AreEqual("Expected values to be null or empty, but found 2 item(s).", ex.Message);
    }

    [TestMethod]
    public void NotBeNullOrEmpty_PassAndFail() {
        int[] values = [1];
        values.Should().NotBeNullOrEmpty();

        int[]? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeNullOrEmpty());
        Assert.AreEqual("Expected missing not to be null or empty, but collection is null.", nullEx.Message);

        int[] empty = [];
        var emptyEx = Assert.ThrowsExactly<AssertionFailedException>(() => empty.Should().NotBeNullOrEmpty());
        Assert.AreEqual("Expected empty not to be null or empty, but it was empty.", emptyEx.Message);
    }

    [TestMethod]
    public void HaveCountPredicate_PassFailAndNullGuard() {
        int[] values = [1, 2, 3];
        values.Should().HaveCount(static count => count > 2);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveCount(static count => count > 5));
        Assert.AreEqual("Expected values to have a count satisfying '(count > 5)', but found 3.", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().HaveCount((Expression<Func<int, bool>>)null!));

        int[]? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveCount(static count => count > 0));
        Assert.AreEqual("Expected missing to have a count satisfying '(count > 0)', but found <null>.", nullEx.Message);
    }
}
