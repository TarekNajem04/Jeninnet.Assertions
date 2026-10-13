//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class DateComponentsCoverageTests {
    [TestMethod]
    public void NullableDateTime_Components_FailForNullAndMismatch() {
        DateTime? missing = null;
        DateTime? present = new DateTime(2026, 1, 2, 3, 4, 5, 6, DateTimeKind.Utc);

        present.Should().HaveYear(2026).And.HaveMonth(1).And.HaveDay(2).And.HaveHour(3).And.HaveMinute(4).And.HaveSecond(5).And.HaveMillisecond(6);
        present.Should().NotHaveYear(2025).And.NotHaveMonth(2).And.NotHaveDay(3).And.NotHaveHour(4).And.NotHaveMinute(5).And.NotHaveSecond(6).And.NotHaveMillisecond(7);

        using var scope = new AssertionScope();

        missing.Should().HaveYear(2026);
        present.Should().HaveYear(2025);
        missing.Should().HaveMonth(1);
        present.Should().HaveMonth(2);
        missing.Should().HaveDay(2);
        present.Should().HaveDay(3);
        missing.Should().HaveHour(3);
        present.Should().HaveHour(4);
        missing.Should().HaveMinute(4);
        present.Should().HaveMinute(5);
        missing.Should().HaveSecond(5);
        present.Should().HaveSecond(6);
        missing.Should().HaveMillisecond(6);
        present.Should().HaveMillisecond(7);
        missing.Should().NotHaveYear(2026);
        present.Should().NotHaveYear(2026);
        missing.Should().NotHaveMonth(1);
        present.Should().NotHaveMonth(1);
        missing.Should().NotHaveDay(2);
        present.Should().NotHaveDay(2);
        missing.Should().NotHaveHour(3);
        present.Should().NotHaveHour(3);
        missing.Should().NotHaveMinute(4);
        present.Should().NotHaveMinute(4);
        missing.Should().NotHaveSecond(5);
        present.Should().NotHaveSecond(5);
        missing.Should().NotHaveMillisecond(6);
        present.Should().NotHaveMillisecond(6);

        var failures = scope.Discard();

        Assert.HasCount(28, failures);
        Assert.AreEqual("Expected missing to have year 2026, but found <null>.", failures[0]);
        Assert.AreEqual("Expected present to have year 2025, but found 2026.", failures[1]);
        Assert.AreEqual("Expected present to have millisecond 7, but found 6.", failures[13]);
        Assert.AreEqual("Expected missing not to have year 2026, but found <null>.", failures[14]);
        Assert.AreEqual("Expected present not to have millisecond 6, but found 6.", failures[27]);
    }

    [TestMethod]
    public void NullableDateTimeOffset_Components_FailForNullAndMismatch() {
        DateTimeOffset? missing = null;
        DateTimeOffset? present = new DateTimeOffset(2026, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(2));

        present.Should().HaveYear(2026).And.HaveMonth(1).And.HaveDay(2).And.HaveHour(3).And.HaveMinute(4).And.HaveSecond(5).And.HaveMillisecond(6);
        present.Should().NotHaveYear(2025).And.NotHaveMonth(2).And.NotHaveDay(3).And.NotHaveHour(4).And.NotHaveMinute(5).And.NotHaveSecond(6).And.NotHaveMillisecond(7);

        using var scope = new AssertionScope();

        missing.Should().HaveYear(2026);
        present.Should().HaveYear(2025);
        missing.Should().HaveMonth(1);
        present.Should().HaveMonth(2);
        missing.Should().HaveDay(2);
        present.Should().HaveDay(3);
        missing.Should().HaveHour(3);
        present.Should().HaveHour(4);
        missing.Should().HaveMinute(4);
        present.Should().HaveMinute(5);
        missing.Should().HaveSecond(5);
        present.Should().HaveSecond(6);
        missing.Should().HaveMillisecond(6);
        present.Should().HaveMillisecond(7);
        missing.Should().NotHaveYear(2026);
        present.Should().NotHaveYear(2026);
        missing.Should().NotHaveMonth(1);
        present.Should().NotHaveMonth(1);
        missing.Should().NotHaveDay(2);
        present.Should().NotHaveDay(2);
        missing.Should().NotHaveHour(3);
        present.Should().NotHaveHour(3);
        missing.Should().NotHaveMinute(4);
        present.Should().NotHaveMinute(4);
        missing.Should().NotHaveSecond(5);
        present.Should().NotHaveSecond(5);
        missing.Should().NotHaveMillisecond(6);
        present.Should().NotHaveMillisecond(6);

        var failures = scope.Discard();

        Assert.HasCount(28, failures);
        Assert.AreEqual("Expected missing to have year 2026, but found <null>.", failures[0]);
        Assert.AreEqual("Expected present to have month 2, but found 1.", failures[3]);
        Assert.AreEqual("Expected missing not to have hour 3, but found <null>.", failures[20]);
        Assert.AreEqual("Expected present not to have millisecond 6, but found 6.", failures[27]);
    }

    [TestMethod]
    public void DateTimeOffset_ComponentsAndOffset_FailForMismatch() {
        var value = new DateTimeOffset(2026, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(2));

        value.Should().HaveYear(2026).And.HaveMonth(1).And.HaveDay(2).And.HaveHour(3).And.HaveMinute(4).And.HaveSecond(5).And.HaveMillisecond(6);
        value.Should().NotHaveYear(2025).And.NotHaveMonth(2).And.NotHaveDay(3).And.NotHaveHour(4).And.NotHaveMinute(5).And.NotHaveSecond(6).And.NotHaveMillisecond(7);
        value.Should().HaveOffset(TimeSpan.FromHours(2)).And.NotHaveOffset(TimeSpan.Zero);

        using var scope = new AssertionScope();

        value.Should().HaveYear(2025);
        value.Should().HaveMonth(2);
        value.Should().HaveDay(3);
        value.Should().HaveHour(4);
        value.Should().HaveMinute(5);
        value.Should().HaveSecond(6);
        value.Should().HaveMillisecond(7);
        value.Should().NotHaveYear(2026);
        value.Should().NotHaveMonth(1);
        value.Should().NotHaveDay(2);
        value.Should().NotHaveHour(3);
        value.Should().NotHaveMinute(4);
        value.Should().NotHaveSecond(5);
        value.Should().NotHaveMillisecond(6);
        value.Should().HaveOffset(TimeSpan.Zero);
        value.Should().NotHaveOffset(TimeSpan.FromHours(2));
        value.Should().BeExactly(value.AddTicks(1));

        var failures = scope.Discard();

        Assert.HasCount(17, failures);
        Assert.AreEqual("Expected value to have year 2025, but found 2026.", failures[0]);
        Assert.AreEqual("Expected value not to have year 2026, but found 2026.", failures[7]);
        Assert.AreEqual("Expected value to have offset 00:00:00, but found 02:00:00.", failures[14]);
        Assert.AreEqual("Expected value not to have offset 02:00:00, but found 02:00:00.", failures[15]);
    }

    [TestMethod]
    public void NullableDateOnly_Verbs_FailForNullAndMismatch() {
        DateOnly today = new(2026, 1, 2);
        DateOnly? missing = null;
        DateOnly? present = today;

        present.Should().Be(present).And.NotBe(today.AddDays(1));
        present.Should().BeBefore(today.AddDays(1)).And.BeOnOrBefore(today).And.BeAfter(today.AddDays(-1)).And.BeOnOrAfter(today);
        present.Should().NotBeBefore(today).And.NotBeAfter(today).And.NotBeOnOrAfter(today.AddDays(1)).And.NotBeOnOrBefore(today.AddDays(-1));
        present.Should().HaveYear(2026).And.HaveMonth(1).And.HaveDay(2);
        present.Should().NotHaveYear(2025).And.NotHaveMonth(2).And.NotHaveDay(3);
        missing.Should().BeNull().And.NotHaveValue().And.NotBe(present);
        missing.Should().BeOneOf(null, present);

        using var scope = new AssertionScope();

        missing.Should().Be(present);
        present.Should().NotBe(today);
        missing.Should().BeBefore(today);
        present.Should().BeBefore(today);
        present.Should().NotBeBefore(today.AddDays(1));
        missing.Should().BeOnOrBefore(today);
        present.Should().BeOnOrBefore(today.AddDays(-1));
        missing.Should().BeAfter(today);
        present.Should().BeAfter(today.AddDays(1));
        present.Should().NotBeAfter(today.AddDays(-1));
        missing.Should().BeOnOrAfter(today);
        present.Should().BeOnOrAfter(today.AddDays(1));
        present.Should().NotBeOnOrAfter(today);
        present.Should().NotBeOnOrBefore(today);
        missing.Should().HaveYear(2026);
        present.Should().HaveYear(2025);
        missing.Should().HaveMonth(1);
        present.Should().HaveMonth(2);
        missing.Should().HaveDay(2);
        present.Should().HaveDay(3);
        missing.Should().NotHaveYear(2026);
        present.Should().NotHaveYear(2026);
        missing.Should().NotHaveMonth(1);
        present.Should().NotHaveMonth(1);
        missing.Should().NotHaveDay(2);
        present.Should().NotHaveDay(2);
        present.Should().BeNull();
        missing.Should().HaveValue();
        present.Should().BeOneOf(today.AddDays(1));

        var failures = scope.Discard();

        Assert.HasCount(29, failures);
        Assert.Contains("Expected missing to be", failures[0]);
        Assert.Contains("but found <null>.", failures[0]);
        Assert.AreEqual("Expected missing to have year 2026, but found <null>.", failures[14]);
        Assert.AreEqual("Expected present to have year 2025, but found 2026.", failures[15]);
        Assert.AreEqual("Expected missing not to have year 2026, but found <null>.", failures[20]);
        Assert.AreEqual("Expected present not to have day 2, but found 2.", failures[25]);
        Assert.Contains("to be null, but found", failures[26]);
        Assert.Contains("to be one of", failures[28]);
    }

    [TestMethod]
    public void NullableDateOnly_Guards_ThrowForNullCandidates() {
        DateOnly? present = new DateOnly(2026, 1, 2);
        Assert.ThrowsExactly<ArgumentNullException>(() => present.Should().BeOneOf((DateOnly?[])null!));
    }
}
