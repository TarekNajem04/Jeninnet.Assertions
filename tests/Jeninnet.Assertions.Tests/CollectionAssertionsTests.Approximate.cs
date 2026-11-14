//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class CollectionAssertionsTests {
    [TestMethod]
    public void EqualApproximately_IntPassAndFail() {
        int[] actual = [1, 2, 3];
        actual.Should().EqualApproximately([1, 2, 4], 1).And.HaveCount(3);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().EqualApproximately([1, 2, 5], 1));
        Assert.AreEqual("Expected actual to approximately equal ['1', '2', '5'], but item at index 2 was '3' instead of '5' with precision '1'.", ex.Message);
    }

    [TestMethod]
    public void EqualApproximately_DoublePassAndFail() {
        double[] actual = [1.0, 2.5];
        actual.Should().EqualApproximately([1.05, 2.45], 0.1);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().EqualApproximately([1.0, 2.8], 0.1));
        Assert.AreEqual("Expected actual to approximately equal ['1', '2.8'], but item at index 1 was '2.5' instead of '2.8' with precision '0.1'.", ex.Message);
    }

    [TestMethod]
    public void EqualApproximately_DecimalDoesNotLosePrecision() {
        decimal[] actual = [0.3m];
        actual.Should().EqualApproximately([0.3m], 0.0000000000000000000000000001m);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => actual.Should().EqualApproximately([0.3m + 0.0000000000000000000000000002m], 0.0000000000000000000000000001m));
        Assert.Contains("to approximately equal", ex.Message);
        Assert.Contains("instead of", ex.Message);
    }

    [TestMethod]
    public void EqualApproximately_LongDoesNotRoundThroughDouble() {
        long[] actual = [long.MaxValue - 1];
        actual.Should().EqualApproximately([long.MaxValue - 1], 1L);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().EqualApproximately([long.MaxValue], 0L));
        Assert.Contains("instead of", ex.Message);
        Assert.Contains("precision '0'", ex.Message);
    }

    [TestMethod]
    public void EqualApproximately_LengthMismatch_ReportsBothCounts() {
        int[] actual = [1, 2, 3];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().EqualApproximately([1, 2], 0));
        Assert.AreEqual("Expected actual to approximately equal ['1', '2'], but found 3 item(s) instead of 2.", ex.Message);
    }

    [TestMethod]
    public void EqualApproximately_NullCollection_FailsClearly() {
        int[]? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().EqualApproximately([1], 0));
        Assert.AreEqual("Expected missing to approximately equal ['1'], but collection is null.", ex.Message);
    }

    [TestMethod]
    public void EqualApproximately_EmptyCollections_Pass() {
        int[] actual = [];
        actual.Should().EqualApproximately([], 0);
    }

    [TestMethod]
    public void EqualApproximately_NaN_NeverApproximates() {
        double[] actual = [double.NaN];
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().EqualApproximately([double.NaN], 1.0));
        Assert.Contains("to approximately equal", ex.Message);

        actual.Should().NotEqualApproximately([double.NaN], 1.0);
    }

    [TestMethod]
    public void EqualApproximately_Infinity_ComparesExactly() {
        double[] actual = [double.PositiveInfinity, double.NegativeInfinity];
        actual.Should().EqualApproximately([double.PositiveInfinity, double.NegativeInfinity], 0.0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().EqualApproximately([double.PositiveInfinity, double.PositiveInfinity], 0.0));
        Assert.Contains("instead of", ex.Message);
    }

    [TestMethod]
    public void EqualApproximately_NegativePrecision_ThrowsArgumentOutOfRange() {
        int[] actual = [1];
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => actual.Should().EqualApproximately([1], -1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => actual.Should().NotEqualApproximately([1], -1));
    }

    [TestMethod]
    public void EqualApproximately_NullExpected_ThrowsArgumentNull() {
        int[] actual = [1];
        Assert.ThrowsExactly<ArgumentNullException>(() => actual.Should().EqualApproximately(null!, 0));
        Assert.ThrowsExactly<ArgumentNullException>(() => actual.Should().NotEqualApproximately(null!, 0));
    }

    [TestMethod]
    public void NotEqualApproximately_PassAndFail() {
        int[] actual = [1, 2, 3];
        actual.Should().NotEqualApproximately([1, 2, 4], 0);
        actual.Should().NotEqualApproximately([1, 2], 0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().NotEqualApproximately([1, 2, 3], 0));
        Assert.AreEqual("Expected actual not to approximately equal ['1', '2', '3'], but they are approximately equal with precision '0'.", ex.Message);
    }

    [TestMethod]
    public void NotEqualApproximately_NullCollection_FailsClearly() {
        int[]? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotEqualApproximately([1], 0));
        Assert.AreEqual("Expected missing not to approximately equal ['1'], but collection is null.", ex.Message);
    }
}
