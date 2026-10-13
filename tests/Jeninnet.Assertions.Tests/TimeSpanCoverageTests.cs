//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class TimeSpanCoverageTests {
    [TestMethod]
    public void Nullable_FailingVerbs_ReportEveryMessage() {
        TimeSpan? missing = null;
        TimeSpan? present = TimeSpan.FromHours(2.5);
        using var scope = new AssertionScope();

        missing.Should().Be(TimeSpan.Zero);
        present.Should().NotBe(TimeSpan.FromHours(2.5));
        missing.Should().BePositive();
        TimeSpan.Zero.Should().BePositive();
        missing.Should().BeNegative();
        TimeSpan.Zero.Should().BeNegative();
        missing.Should().BeGreaterThan(TimeSpan.Zero);
        present.Should().BeGreaterThan(TimeSpan.FromHours(3));
        missing.Should().BeGreaterThanOrEqualTo(TimeSpan.Zero);
        present.Should().BeGreaterThanOrEqualTo(TimeSpan.FromHours(3));
        missing.Should().BeLessThan(TimeSpan.Zero);
        present.Should().BeLessThan(TimeSpan.FromHours(1));
        missing.Should().BeLessThanOrEqualTo(TimeSpan.Zero);
        present.Should().BeLessThanOrEqualTo(TimeSpan.FromHours(1));
        missing.Should().BeCloseTo(TimeSpan.Zero, TimeSpan.FromMinutes(1));
        present.Should().BeCloseTo(TimeSpan.FromHours(5), TimeSpan.FromMinutes(1));
        present.Should().NotBeCloseTo(TimeSpan.FromHours(2.5), TimeSpan.FromMinutes(1));
        missing.Should().HaveDays(1);
        present.Should().HaveDays(1);
        missing.Should().HaveHours(1);
        present.Should().HaveHours(1);
        missing.Should().HaveMinutes(1);
        present.Should().HaveMinutes(1);
        missing.Should().HaveSeconds(1);
        present.Should().HaveSeconds(1);
        missing.Should().HaveMilliseconds(1);
        present.Should().HaveMilliseconds(1);
        missing.Should().HaveTicks(1);
        present.Should().HaveTicks(1);

        var failures = scope.Discard();

        Assert.HasCount(29, failures);
        Assert.AreEqual("Expected missing to be 00:00:00, but found <null>.", failures[0]);
        Assert.AreEqual("Expected present not to be 02:30:00, but found 02:30:00.", failures[1]);
        Assert.AreEqual("Expected missing to be positive, but found <null>.", failures[2]);
        Assert.AreEqual("Expected TimeSpan.Zero to be positive, but found 00:00:00.", failures[3]);
        Assert.AreEqual("Expected missing to be negative, but found <null>.", failures[4]);
        Assert.AreEqual("Expected missing to be greater than 00:00:00, but found <null>.", failures[6]);
        Assert.AreEqual("Expected present to be greater than 03:00:00, but found 02:30:00.", failures[7]);
        Assert.AreEqual("Expected missing to be greater than or equal to 00:00:00, but found <null>.", failures[8]);
        Assert.AreEqual("Expected missing to be less than 00:00:00, but found <null>.", failures[10]);
        Assert.AreEqual("Expected missing to be less than or equal to 00:00:00, but found <null>.", failures[12]);
        Assert.AreEqual("Expected missing to be close to 00:00:00 (±00:01:00), but found <null>.", failures[14]);
        Assert.AreEqual("Expected present to be close to 05:00:00 (±00:01:00), but found 02:30:00.", failures[15]);
        Assert.AreEqual("Expected present not to be close to 02:30:00 (±00:01:00), but found 02:30:00.", failures[16]);
        Assert.AreEqual("Expected missing to have days 1, but found <null>.", failures[17]);
        Assert.AreEqual("Expected present to have days 1, but found 0.", failures[18]);
        Assert.AreEqual("Expected missing to have hours 1, but found <null>.", failures[19]);
        Assert.AreEqual("Expected present to have hours 1, but found 2.", failures[20]);
        Assert.AreEqual("Expected missing to have minutes 1, but found <null>.", failures[21]);
        Assert.AreEqual("Expected present to have minutes 1, but found 30.", failures[22]);
        Assert.AreEqual("Expected missing to have seconds 1, but found <null>.", failures[23]);
        Assert.AreEqual("Expected present to have seconds 1, but found 0.", failures[24]);
        Assert.AreEqual("Expected missing to have milliseconds 1, but found <null>.", failures[25]);
        Assert.AreEqual("Expected present to have milliseconds 1, but found 0.", failures[26]);
        Assert.AreEqual("Expected missing to have ticks 1, but found <null>.", failures[27]);
        Assert.Contains("to have ticks 1, but found", failures[28]);
    }

    [TestMethod]
    public void Nullable_CloseTo_Guards_ThrowArgumentOutOfRange() {
        TimeSpan? value = TimeSpan.FromSeconds(1);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => value.Should().BeCloseTo(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => value.Should().NotBeCloseTo(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1)));
    }
}
