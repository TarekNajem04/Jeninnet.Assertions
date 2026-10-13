//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class NullableNumericTests {
    [TestMethod]
    public void Be_PassForEqualAndNull() {
        int? count = 3;
        count.Should().Be(3);
        count.Should().HaveValue();
        int? missing = null;
        missing.Should().Be(null);
        missing.Should().BeNull();
    }

    [TestMethod]
    public void Be_FailsForNull() {
        int? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().Be(5));
        Assert.AreEqual("Expected missing to be 5, but found <null>.", ex.Message);
    }

    [TestMethod]
    public void Comparisons_FailForNull() {
        int? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeGreaterThan(0));
        Assert.AreEqual("Expected missing to be greater than 0, but found <null>.", ex.Message);
    }

    [TestMethod]
    public void NotBe_PassForNull() {
        int? missing = null;
        missing.Should().NotBe(5);
        missing.Should().NotBeInRange(1, 10);
        missing.Should().NotBeNaN();
    }

    [TestMethod]
    public void NotHaveValue_PassAndFail() {
        int? missing = null;
        missing.Should().NotHaveValue();

        int? count = 3;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => count.Should().NotHaveValue());
        Assert.AreEqual("Expected count to be null, but found 3.", ex.Message);
    }

    [TestMethod]
    public void BePositive_BeNegative_FailForNull() {
        int? missing = null;
        var positive = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BePositive());
        Assert.AreEqual("Expected missing to be positive, but found <null>.", positive.Message);

        int? debt = -3;
        debt.Should().BeNegative();
    }

    [TestMethod]
    public void BeApproximately_FailsForNull() {
        double? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeApproximately(1.0, 0.1));
        Assert.AreEqual("Expected missing to approximate 1 (±0.1), but found <null>.", ex.Message);
    }

    [TestMethod]
    public void BeNaN_PassForNaN() {
        double? nan = double.NaN;
        nan.Should().BeNaN();
    }
}
