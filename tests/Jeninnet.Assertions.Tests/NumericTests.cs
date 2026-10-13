//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class NumericTests {
    [TestMethod]
    public void Be_PassForEqualValue() {
        const int count = 5;
        count.Should().Be(5);
    }

    [TestMethod]
    public void Be_FailsForDifferentValue() {
        const int count = 5;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().Be(6));
        Assert.AreEqual("Expected count to be 6, but found 5.", ex.Message);
    }

    [TestMethod]
    public void NotBe_FailsForEqualValue() {
        const int count = 5;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().NotBe(5));
        Assert.AreEqual("Expected count not to be 5, but found 5.", ex.Message);
    }

    [TestMethod]
    public void Be_ChainsIntoNumericAssertions() {
        const int count = 5;
        count.Should().Be(5).And.BePositive().And.NotBeNaN();
    }

    [TestMethod]
    public void NotBe_ChainsIntoNumericAssertions() {
        const int count = 5;
        count.Should().NotBe(6).And.BePositive();
    }

    [TestMethod]
    public void Comparisons_PassAndFail() {
        const int count = 5;
        count.Should().BeGreaterThan(4).And.BeLessThan(6);

        var greater = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().BeGreaterThan(10));
        Assert.AreEqual("Expected count to be greater than 10, but found 5.", greater.Message);

        var greaterOrEqual = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().BeGreaterThanOrEqualTo(6));
        Assert.AreEqual("Expected count to be greater than or equal to 6, but found 5.", greaterOrEqual.Message);

        var less = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().BeLessThan(5));
        Assert.AreEqual("Expected count to be less than 5, but found 5.", less.Message);

        var lessOrEqual = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().BeLessThanOrEqualTo(4));
        Assert.AreEqual("Expected count to be less than or equal to 4, but found 5.", lessOrEqual.Message);
    }

    [TestMethod]
    public void BePositive_BeNegative_ExcludeZero() {
        const int positive = 5;
        positive.Should().BePositive();
        const int negative = -8;
        negative.Should().BeNegative();

        const int zero = 0;
        var positiveEx = Assert.ThrowsExactly<AssertionFailedException>(() => zero.Should().BePositive());
        Assert.AreEqual("Expected zero to be positive, but found 0.", positiveEx.Message);
        var negativeEx = Assert.ThrowsExactly<AssertionFailedException>(() => zero.Should().BeNegative());
        Assert.AreEqual("Expected zero to be negative, but found 0.", negativeEx.Message);
    }

    [TestMethod]
    public void BeInRange_PassInsideBounds() {
        const int count = 5;
        count.Should().BeInRange(1, 10);
        count.Should().BeInRange(5, 5);
    }

    [TestMethod]
    public void BeInRange_FailsOutsideBounds() {
        const int count = 5;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().BeInRange(6, 10));
        Assert.AreEqual("Expected count to be between 6 and 10, but found 5.", ex.Message);
    }

    [TestMethod]
    public void BeInRange_InvertedBounds_ThrowsArgumentException() {
        const int count = 5;
        Assert.ThrowsExactly<ArgumentException>(() => count.Should().BeInRange(10, 1));
        Assert.ThrowsExactly<ArgumentException>(() => count.Should().NotBeInRange(10, 1));
    }

    [TestMethod]
    public void NotBeInRange_FailsInsideBounds() {
        const int count = 5;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().NotBeInRange(1, 10));
        Assert.AreEqual("Expected count not to be between 1 and 10, but found 5.", ex.Message);
    }

    [TestMethod]
    public void BeApproximately_PassWithinPrecision() {
        const float value = 3.14F;
        value.Should().BeApproximately(3.14F, 0.01F);
        const double ratio = 5.1;
        ratio.Should().BeApproximately(5.0, 0.2);
    }

    [TestMethod]
    public void BeApproximately_FailsOutsidePrecision() {
        const double ratio = 5.1;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => ratio.Should().BeApproximately(5.0, 0.05));
        Assert.AreEqual("Expected ratio to approximate 5 (±0.05), but found 5.1.", ex.Message);
    }

    [TestMethod]
    public void BeApproximately_NegativePrecision_ThrowsArgumentOutOfRange() {
        const double ratio = 5.1;
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ratio.Should().BeApproximately(5.0, -0.1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ratio.Should().NotBeApproximately(5.0, -0.1));
    }

    [TestMethod]
    public void NotBeApproximately_FailsWithinPrecision() {
        const float value = 3.5F;
        value.Should().NotBeApproximately(2.5F, 0.5F);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().NotBeApproximately(3.4F, 0.5F));
        Assert.AreEqual("Expected value not to approximate 3.4 (±0.5), but found 3.5.", ex.Message);
    }

    [TestMethod]
    public void BeNaN_PassForNaN() {
        const float nan = float.NaN;
        nan.Should().BeNaN();
    }

    [TestMethod]
    public void BeNaN_FailsForNumber() {
        const float value = 1.0F;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().BeNaN());
        Assert.AreEqual("Expected value to be NaN, but found 1.", ex.Message);
    }

    [TestMethod]
    public void NotBeNaN_FailsForNaN() {
        const float nan = float.NaN;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => nan.Should().NotBeNaN());
        Assert.AreEqual("Expected nan not to be NaN, but found NaN.", ex.Message);
    }

    [TestMethod]
    public void BeOneOf_PassForCandidate() {
        const int count = 5;
        count.Should().BeOneOf(3, 6, 5);
    }

    [TestMethod]
    public void BeOneOf_FailsListingCandidates() {
        const int count = 5;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().BeOneOf(3, 6));
        Assert.AreEqual("Expected count to be one of 3, 6, but found 5.", ex.Message);
    }

    [TestMethod]
    public void Match_PassForSatisfiedPredicate() {
        const int count = 5;
        count.Should().Match(static x => x % 2 == 1);
    }

    [TestMethod]
    public void Match_FailsForUnsatisfiedPredicate() {
        const int count = 4;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().Match(static x => x % 2 == 1));
        Assert.Contains("Expected count to match", ex.Message);
        Assert.Contains("but it did not.", ex.Message);
    }

    [TestMethod]
    public void WorksForOtherNumericTypes() {
        const long big = 100L;
        big.Should().BeGreaterThan(99L);
        const decimal price = 9.99M;
        price.Should().BeInRange(9M, 10M);
        const byte small = 2;
        small.Should().Be(2);
    }

    [TestMethod]
    public void BeRankedEquallyTo_PassAndFail() {
        const int rank = 5;
        rank.Should().BeRankedEquallyTo(5);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => rank.Should().BeRankedEquallyTo(6));
        Assert.AreEqual("Expected rank to be ranked equal to 6, but found 5.", ex.Message);
    }

    [TestMethod]
    public void NotBeRankedEquallyTo_PassAndFail() {
        const int rank = 5;
        rank.Should().NotBeRankedEquallyTo(6);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => rank.Should().NotBeRankedEquallyTo(5));
        Assert.AreEqual("Expected rank not to be ranked equal to 5, but found 5.", ex.Message);
    }

    [TestMethod]
    public void BeEquivalentTo_UsesObjectGraphEquivalency() {
        const int rank = 5;
        rank.Should().BeEquivalentTo(5);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => rank.Should().BeEquivalentTo(6));
        Assert.Contains("to be equivalent", ex.Message);
    }
}
