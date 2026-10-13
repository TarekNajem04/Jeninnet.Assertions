//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class CollectionAssertionsTests {
    [TestMethod]
    public void BeEquivalentTo_PassIgnoringOrder() {
        int[] values = [1, 2, 3];
        values.Should().BeEquivalentTo(3, 1, 2);
    }

    [TestMethod]
    public void BeEquivalentTo_PassForDuplicatesWithSameCardinality() {
        int[] values = [1, 1, 2];
        values.Should().BeEquivalentTo(2, 1, 1);
    }

    [TestMethod]
    public void BeEquivalentTo_FailsForDifferentCardinality() {
        int[] values = [1, 1];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeEquivalentTo(1, 2));
        Assert.AreEqual("Expected values to be equivalent, but item '1' was found 2 time(s); expected 1.", ex.Message);
    }

    [TestMethod]
    public void BeEquivalentTo_FailsForMissingItem() {
        int[] values = [1, 2];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeEquivalentTo(1, 9));
        Assert.AreEqual("Expected values to be equivalent, but item '9' was found 0 time(s); expected 1.", ex.Message);
    }

    [TestMethod]
    public void BeEquivalentTo_FailsForDifferentCount() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeEquivalentTo(1, 2));
        Assert.AreEqual("Expected values to contain 2 item(s), but found 3.", ex.Message);
    }

    [TestMethod]
    public void BeEquivalentTo_FailsForNullExpected() {
        int[] values = [1];
        Assert.ThrowsExactly<ArgumentNullException>(() => values.Should().BeEquivalentTo((IEnumerable<int>)null!));
    }

    [TestMethod]
    public void BeEquivalentTo_CustomEquality_RespectsTypeSpecificComparer() {
        var records = new List<DeliberatelyEqualItem> { new("a"), new("a"), new("b") };

        records.Should().BeEquivalentTo(new DeliberatelyEqualItem("a"), new DeliberatelyEqualItem("b"), new DeliberatelyEqualItem("a"));
    }

    [TestMethod]
    public void NotEqual_PassForDifferentValues() {
        int[] values = [1, 2, 3];
        values.Should().NotEqual([1, 2, 4]);
    }

    [TestMethod]
    public void NotEqual_FailsForEqualValues() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotEqual([1, 2, 3]));
        Assert.AreEqual("Expected values not to equal ['1', '2', '3'], but they are equal.", ex.Message);
    }

    [TestMethod]
    public void NotBeEquivalentTo_PassForDifferentValues() {
        int[] values = [1, 2, 3];
        values.Should().NotBeEquivalentTo([1, 2, 4]);
    }

    [TestMethod]
    public void NotBeEquivalentTo_FailsForEquivalentValues() {
        int[] values = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotBeEquivalentTo([3, 2, 1]));
        Assert.AreEqual("Expected values not to be equivalent to ['3', '2', '1'], but found differences.", ex.Message);
    }

    [TestMethod]
    public void AllBeEquivalentTo_PassAndFail() {
        string[] values = ["a", "a"];
        values.Should().AllBeEquivalentTo("a");
        values.Should().AllBeEquivalentTo("A", o => o.IgnoringCase());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().AllBeEquivalentTo("z"));
        Assert.AreEqual("Expected values for all elements to be equivalent to 'z', but found 'a' at index 0.", ex.Message);

        string[]? missing = null;
        var nullEx = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().AllBeEquivalentTo("a"));
        Assert.AreEqual("Expected missing for all elements to be equivalent to 'a', but collection is null.", nullEx.Message);
    }

    [TestMethod]
    public void BeEqualTo_MirrorsEqual() {
        int[] values = [1, 2, 3];
        values.Should().BeEqualTo([1, 2, 3]);
        var baseline = new[] { 1, 2, 3 };
        values.Should().BeEqualTo(baseline.AsEnumerable());

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().BeEqualTo([1, 3, 2]));
        Assert.AreEqual("Expected values to equal ['1', '3', '2'], but item at index 1 was '2' instead of '3'.", ex.Message);

        values.Should().NotBeEqualTo([1, 3, 2]);
        var notEx = Assert.ThrowsExactly<AssertionFailedException>(() => values.Should().NotBeEqualTo([1, 2, 3]));
        Assert.AreEqual("Expected values not to equal ['1', '2', '3'], but they are equal.", notEx.Message);
    }

    [TestMethod]
    public void EqualWithPredicate_ComparesElementWise() {
        var actual = new[] { new DeliberatelyEqualItem("a"), new DeliberatelyEqualItem("b") };
        var expected = new[] { "a", "b" };
        actual.Should().Equal(expected, static (item, name) => item.Name == name);
        actual.Should().BeEqualTo(expected, static (item, name) => item.Name == name);

        var renamed = new[] { "a", "c" };
        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().Equal(renamed, static (item, name) => item.Name == name));
        Assert.AreEqual("Expected actual to equal ['a', 'c'], but item at index 1 was 'Jeninnet.Assertions.Tests.CollectionAssertionsTests+DeliberatelyEqualItem' instead of 'c'.", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => actual.Should().Equal((string[])null!, static (item, name) => item.Name == name));
    }
}
