//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class CollectionAssertionsTests {
    [TestMethod]
    public void Contain_Item_PassWhenPresent() {
        int[] values = [1, 2, 3];
        values.Should().Contain(2);
    }

    [TestMethod]
    public void Contain_Item_FailsWhenMissing() {
        int[] values = [1, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().Contain(9));
        Assert.AreEqual("Expected values to contain '9', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void Contain_Predicate_PassWhenAnyMatches() {
        int[] values = [1, 2, 3];
        values.Should().Contain(static x => x > 2);
    }

    [TestMethod]
    public void Contain_Predicate_FailsWhenNoneMatches() {
        int[] values = [1, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().Contain(static x => x > 5));
        Assert.AreEqual("Expected values to contain an item matching the predicate, but none was found.", ex.Message);
    }

    [TestMethod]
    public void NotContain_Item_PassWhenAbsent() {
        int[] values = [1, 2, 3];
        values.Should().NotContain(9);
    }

    [TestMethod]
    public void NotContain_Item_FailsWhenPresent() {
        int[] values = [1, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotContain(2));
        Assert.AreEqual("Expected values to not contain '2', but it was found.", ex.Message);
    }

    [TestMethod]
    public void NotContain_Predicate_PassWhenNoMatch() {
        int[] values = [1, 2];
        values.Should().NotContain(static x => x > 5);
    }

    [TestMethod]
    public void NotContain_Predicate_FailsWhenAnyMatches() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotContain(static x => x > 2));
        Assert.AreEqual("Expected values to not contain an item matching the predicate, but one was found.", ex.Message);
    }

    [TestMethod]
    public void ContainSingle_PassForSingleItem() {
        int[] values = [7];
        var which = values.Should().ContainSingle();
        Assert.AreEqual(7, which.Which);
    }

    [TestMethod]
    public void ContainSingle_PassForSingleMatchingItem() {
        int[] values = [1, 5, 9];
        var which = values.Should().ContainSingle(static x => x > 6);
        Assert.AreEqual(9, which.Which);
    }

    [TestMethod]
    public void ContainSingle_Which_SupportsFurtherAssertions() {
        int[] values = [1, 5, 9];
        values.Should().ContainSingle(static x => x > 6).Which.Should().Be(9);
    }

    [TestMethod]
    public void ContainSingle_FailsForNoMatch() {
        int[] values = [1, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().ContainSingle(static x => x > 6));
        Assert.AreEqual("Expected values to contain exactly one item, but found 0.", ex.Message);
    }

    [TestMethod]
    public void ContainSingle_FailsForMultipleMatches() {
        int[] values = [7, 8];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().ContainSingle(static x => x > 6));
        Assert.AreEqual("Expected values to contain exactly one item, but found 2.", ex.Message);
    }

    [TestMethod]
    public void ContainSubset_PassWhenAllExpectedPresent() {
        int[] values = [1, 2, 3];
        values.Should().ContainSubset([1, 3]);
    }

    [TestMethod]
    public void ContainSubset_FailsForMissingSubsetItem() {
        int[] values = [1, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().ContainSubset([1, 9]));
        Assert.AreEqual("Expected values to contain subset item '9', but it was not found.", ex.Message);
    }

    [TestMethod]
    public void ContainItemsAssignableTo_PassAndFail() {
        object[] items = ["a", 1];
        items.Should().ContainItemsAssignableTo<string>();
        items.Should().NotContainItemsAssignableTo<DateTime>();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => items.Should().ContainItemsAssignableTo<DateTime>());
        Assert.AreEqual("Expected items to contain an item assignable to DateTime, but found none.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => items.Should().NotContainItemsAssignableTo<string>());
        Assert.AreEqual("Expected items not to contain an item assignable to String, but found 'a' at index 0.", notEx.Message);

        object[]? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainItemsAssignableTo<string>());
        Assert.AreEqual("Expected missing to contain an item assignable to String, but collection is null.", nullEx.Message);
    }

    [TestMethod]
    public void HaveElementPreceding_PassAndFail() {
        int[] values = [1, 2, 3];
        values.Should().HaveElementPreceding(2, 1);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveElementPreceding(3, 1));
        Assert.AreEqual("Expected values to have '1' immediately preceding '3', but found no such pair.", ex.Message);

        int[]? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveElementPreceding(2, 1));
        Assert.AreEqual("Expected missing to have '1' immediately preceding '2', but collection is null.", nullEx.Message);
    }

    [TestMethod]
    public void HaveElementSucceeding_PassAndFail() {
        int[] values = [1, 2, 3];
        values.Should().HaveElementSucceeding(1, 2);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().HaveElementSucceeding(1, 3));
        Assert.AreEqual("Expected values to have '3' immediately succeeding '1', but found no such pair.", ex.Message);

        int[]? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveElementSucceeding(1, 2));
        Assert.AreEqual("Expected missing to have '2' immediately succeeding '1', but collection is null.", nullEx.Message);
    }

    [TestMethod]
    public void ContainEquivalentOf_PassAndFail() {
        var boxes = new[] { new BoxedSize(1), new BoxedSize(2) };
        boxes.Should().ContainEquivalentOf(new { Size = 2 });
        boxes.Should().NotContainEquivalentOf(new { Size = 9 });

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => boxes.Should().ContainEquivalentOf(new { Size = 9 }));
        Assert.AreEqual("Expected boxes to contain an item equivalent to { Size = 9 }, but none was found.", ex.Message);

        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => boxes.Should().NotContainEquivalentOf(new { Size = 1 }));
        Assert.AreEqual("Expected boxes not to contain an item equivalent to { Size = 1 }, but found one.", notEx.Message);

        BoxedSize[]? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().ContainEquivalentOf(new { Size = 1 }));
        Assert.AreEqual("Expected missing to contain an item equivalent to { Size = 1 }, but collection is null.", nullEx.Message);
    }
}
