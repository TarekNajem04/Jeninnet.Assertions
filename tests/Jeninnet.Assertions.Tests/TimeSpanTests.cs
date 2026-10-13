//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class TimeSpanTests {
    private static readonly TimeSpan _duration = TimeSpan.FromHours(2.5);

    [TestMethod]
    public void Be_PassAndFail() {
        _duration.Should().Be(TimeSpan.FromMinutes(150));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _duration.Should().Be(TimeSpan.FromHours(3)));
        Assert.AreEqual("Expected _duration to be 03:00:00, but found 02:30:00.", ex.Message);
    }

    [TestMethod]
    public void Sign_PassAndFail() {
        _duration.Should().BePositive();
        TimeSpan.FromHours(-1).Should().BeNegative();

        var positive = Assert.ThrowsExactly<AssertionFailedException>(() => TimeSpan.Zero.Should().BePositive());
        Assert.AreEqual("Expected TimeSpan.Zero to be positive, but found 00:00:00.", positive.Message);
    }

    [TestMethod]
    public void Comparisons_PassAndFail() {
        _duration.Should().BeGreaterThan(TimeSpan.FromHours(2)).And.BeLessThan(TimeSpan.FromHours(3))
            .And.BeGreaterThanOrEqualTo(_duration).And.BeLessThanOrEqualTo(_duration);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _duration.Should().BeLessThan(TimeSpan.FromHours(2)));
        Assert.AreEqual("Expected _duration to be less than 02:00:00, but found 02:30:00.", ex.Message);
    }

    [TestMethod]
    public void BeCloseTo_PassAndFail() {
        TimeSpan.FromSeconds(100).Should().BeCloseTo(TimeSpan.FromSeconds(105), TimeSpan.FromSeconds(10));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => TimeSpan.FromSeconds(100).Should().BeCloseTo(TimeSpan.FromSeconds(105), TimeSpan.FromSeconds(2)));
        Assert.AreEqual("Expected TimeSpan.FromSeconds(100) to be close to 00:01:45 (±00:00:02), but found 00:01:40.", ex.Message);
    }

    [TestMethod]
    public void Components_PassAndFail() {
        _duration.Should().HaveDays(0).And.HaveHours(2).And.HaveMinutes(30).And.HaveSeconds(0).And.HaveMilliseconds(0)
            .And.HaveTicks(TimeSpan.FromMinutes(150).Ticks);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _duration.Should().HaveHours(3));
        Assert.AreEqual("Expected _duration to have hours 3, but found 2.", ex.Message);
    }

    [TestMethod]
    public void Nullable_Behaviors() {
        TimeSpan? present = _duration;
        present.Should().Be(_duration).And.BePositive().And.HaveValue();

        TimeSpan? missing = null;
        missing.Should().Be(null).And.BeNull();
        missing.Should().NotBe(_duration).And.NotBeCloseTo(_duration, TimeSpan.FromMinutes(1));

        var greater = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeGreaterThan(_duration));
        Assert.AreEqual("Expected missing to be greater than 02:30:00, but found <null>.", greater.Message);
    }

    [TestMethod]
    public void NullableHaveValue_PassAndFail() {
        TimeSpan? present = _duration;
        present.Should().HaveValue();

        TimeSpan? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("Expected missing to not be null, but found null.", ex.Message);
    }

    [TestMethod]
    public void NullableNotHaveValue_PassAndFail() {
        TimeSpan? missing = null;
        missing.Should().NotHaveValue();

        TimeSpan? present = _duration;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => present.Should().NotHaveValue());
        Assert.AreEqual("Expected present to be null, but found 02:30:00.", ex.Message);
    }
}
