//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
using System.Numerics;

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

    [TestMethod]
    public void BeApproximately_DecimalDoesNotLosePrecision() {
        const decimal actual = 0.1m + 0.2m;
        actual.Should().BeApproximately(0.3m, 0.0000000000000000000000000001m);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeApproximately(0.3m + 0.0000000000000000000000000002m, 0.0000000000000000000000000001m));
        Assert.Contains("to approximate", ex.Message);
    }

    [TestMethod]
    public void BeApproximately_LongDoesNotRoundThroughDouble() {
        const long actual = long.MaxValue;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeApproximately(long.MaxValue - 1, 0L));
        Assert.Contains("but found 9223372036854775807", ex.Message);

        actual.Should().BeApproximately(long.MaxValue - 1, 1L);
        actual.Should().NotBeApproximately(long.MaxValue - 1, 0L);
    }

    [TestMethod]
    public void BeApproximately_ULongDoesNotRoundThroughDouble() {
        const ulong actual = ulong.MaxValue;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeApproximately(ulong.MaxValue - 1, 0UL));
        Assert.Contains("but found 18446744073709551615", ex.Message);

        actual.Should().BeApproximately(ulong.MaxValue - 1, 1UL);
    }

    [TestMethod]
    public void BeApproximately_DecimalMaxValueDoesNotRoundThroughDouble() {
        const decimal actual = decimal.MaxValue;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => actual.Should().BeApproximately(decimal.MaxValue - 1m, 0m));
        Assert.Contains("to approximate", ex.Message);

        actual.Should().BeApproximately(decimal.MaxValue - 1m, 1m);
    }

    [TestMethod]
    public void BeApproximately_DecimalExtremesDoNotOverflow() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => decimal.MaxValue.Should().BeApproximately(decimal.MinValue, decimal.MaxValue));
        Assert.Contains("to approximate", ex.Message);

        decimal.MaxValue.Should().NotBeApproximately(decimal.MinValue, decimal.MaxValue);
    }

    [TestMethod]
    public void BeApproximately_NaN_NeverApproximates() {
        const double nan = double.NaN;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => nan.Should().BeApproximately(double.NaN, 1.0));
        Assert.Contains("NaN", ex.Message);

        nan.Should().NotBeApproximately(1.0, 1.0);
        nan.Should().NotBeApproximately(double.NaN, 1.0);
    }

    [TestMethod]
    public void BeApproximately_Infinity_RequiresEqualInfinity() {
        double.PositiveInfinity.Should().BeApproximately(double.PositiveInfinity, 0.0);
        double.NegativeInfinity.Should().BeApproximately(double.NegativeInfinity, 0.0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => double.PositiveInfinity.Should().BeApproximately(double.NegativeInfinity, 1.0));
        Assert.Contains("to approximate", ex.Message);

        double.PositiveInfinity.Should().NotBeApproximately(double.NegativeInfinity, 1.0);
    }

    [TestMethod]
    public void BeZero_And_NotBeZero() {
        const int zero = 0;
        zero.Should().BeZero();
        const decimal zeroDecimal = 0m;
        zeroDecimal.Should().BeZero();

        const int five = 5;
        five.Should().NotBeZero();

        var beZero = Assert.ThrowsExactly<AssertionFailedException>(() => five.Should().BeZero());
        Assert.AreEqual("Expected five to be 0, but found 5.", beZero.Message);

        var notBeZero = Assert.ThrowsExactly<AssertionFailedException>(() => zero.Should().NotBeZero());
        Assert.AreEqual("Expected zero not to be 0, but found 0.", notBeZero.Message);
    }

    [TestMethod]
    public void Should_SupportsBigInteger() {
        BigInteger value = BigInteger.Parse("123456789012345678901234567890", CultureInfo.InvariantCulture);
        value.Should().BeGreaterThan(BigInteger.One);
        value.Should().BeApproximately(value + 1, BigInteger.One);
        value.Should().NotBeZero();

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().BeApproximately(value + 2, BigInteger.One));
        Assert.Contains("to approximate", ex.Message);
    }

    [TestMethod]
    public void Should_SupportsHalf() {
        Half value = (Half)1.5;
        value.Should().BeApproximately((Half)1.6, (Half)0.11);
        Half.NaN.Should().BeNaN();
        ((Half)0).Should().BeZero();
    }

    [TestMethod]
    public void Should_SupportsInt128AndUInt128() {
        Int128 signed = Int128.MaxValue;
        signed.Should().BeApproximately(Int128.MaxValue - 1, 1);
        Int128.MinValue.Should().BeApproximately(Int128.MinValue + 1, 1);
        Assert.ThrowsExactly<AssertionFailedException>(() => signed.Should().BeApproximately(Int128.MaxValue - 1, 0));

        UInt128 unsigned = UInt128.MaxValue;
        unsigned.Should().BeApproximately(UInt128.MaxValue - 1, 1);
        Assert.ThrowsExactly<AssertionFailedException>(() => unsigned.Should().BeApproximately(UInt128.MaxValue - 1, 0));
    }

    [TestMethod]
    public void Should_SupportsNativeSizedIntegers() {
        nint signed = 5;
        signed.Should().Be(5L);
        signed.Should().BePositive();

        nuint unsigned = 5;
        unsigned.Should().Be(5UL);
        unsigned.Should().BePositive();
    }
}
