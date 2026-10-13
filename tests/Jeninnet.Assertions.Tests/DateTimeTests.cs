//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class DateTimeTests {
    private static readonly DateTime _day = new(2026, 5, 10, 8, 30, 0, DateTimeKind.Unspecified);

    [TestMethod]
    public void Be_PassAndFail() {
        _day.Should().Be(new DateTime(2026, 5, 10, 8, 30, 0, DateTimeKind.Unspecified));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().Be(new DateTime(2026, 5, 11, 8, 30, 0, DateTimeKind.Unspecified)));
        Assert.AreEqual("Expected _day to be 05/11/2026 08:30:00, but found 05/10/2026 08:30:00.", ex.Message);
    }

    [TestMethod]
    public void NotBe_FailsForEqual() {
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().NotBe(_day));
        Assert.AreEqual("Expected _day not to be 05/10/2026 08:30:00, but found 05/10/2026 08:30:00.", ex.Message);
    }

    [TestMethod]
    public void Ordering_PassAndFail() {
        _day.Should().BeBefore(new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Unspecified)).And.BeAfter(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified))
            .And.BeOnOrBefore(_day).And.BeOnOrAfter(_day).And.NotBeBefore(_day).And.NotBeAfter(_day);

        var before = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().BeBefore(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)));
        Assert.AreEqual("Expected _day to be before 01/01/2026 00:00:00, but found 05/10/2026 08:30:00.", before.Message);

        var after = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().BeAfter(new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Unspecified)));
        Assert.AreEqual("Expected _day to be after 12/31/2026 00:00:00, but found 05/10/2026 08:30:00.", after.Message);
    }

    [TestMethod]
    public void BeCloseTo_PassAndFail() {
        _day.Should().BeCloseTo(_day.AddSeconds(30), TimeSpan.FromMinutes(1));
        _day.Should().NotBeCloseTo(_day.AddHours(2), TimeSpan.FromMinutes(1));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().BeCloseTo(_day.AddSeconds(30), TimeSpan.FromSeconds(10)));
        Assert.AreEqual("Expected _day to be close to 05/10/2026 08:30:30 (±00:00:10), but found 05/10/2026 08:30:00.", ex.Message);
    }

    [TestMethod]
    public void BeCloseTo_NegativePrecision_ThrowsArgumentOutOfRange() => Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _day.Should().BeCloseTo(_day, TimeSpan.FromSeconds(-1)));

    [TestMethod]
    public void Components_PassAndFail() {
        _day.Should().HaveYear(2026).And.HaveMonth(5).And.HaveDay(10).And.HaveHour(8).And.HaveMinute(30).And.HaveSecond(0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().HaveYear(2025));
        Assert.AreEqual("Expected _day to have year 2025, but found 2026.", ex.Message);
    }

    [TestMethod]
    public void Nullable_Behaviors() {
        DateTime? present = _day;
        present.Should().Be(_day).And.BeBefore(new DateTime(2027, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)).And.HaveValue();

        DateTime? missing = null;
        missing.Should().Be(null).And.BeNull();
        missing.Should().NotBe(_day).And.NotBeBefore(_day).And.NotBeAfter(_day);

        var before = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeBefore(_day));
        Assert.AreEqual("Expected missing to be before 05/10/2026 08:30:00, but found <null>.", before.Message);

        var notNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("Expected missing to not be null, but found null.", notNull.Message);
    }

    [TestMethod]
    public void NullableHaveValue_PassAndFail() {
        DateTime? present = _day;
        present.Should().HaveValue();

        DateTime? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("Expected missing to not be null, but found null.", ex.Message);
    }

    [TestMethod]
    public void NullableNotHaveValue_PassAndFail() {
        DateTime? missing = null;
        missing.Should().NotHaveValue();

        DateTime? present = _day;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => present.Should().NotHaveValue());
        Assert.AreEqual("Expected present to be null, but found 05/10/2026 08:30:00.", ex.Message);
    }

    [TestMethod]
    public void NullableOffsetHaveValue_PassAndFail() {
        DateTimeOffset? present = new DateTimeOffset(2026, 5, 10, 8, 30, 0, TimeSpan.FromHours(2));
        present.Should().HaveValue();

        DateTimeOffset? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("Expected missing to not be null, but found null.", ex.Message);

        missing.Should().NotHaveValue();
    }

    [TestMethod]
    public void DateTimeOffset_BeAndOrdering() {
        var offset = new DateTimeOffset(2026, 5, 10, 8, 30, 0, TimeSpan.FromHours(2));
        offset.Should().Be(offset);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => offset.Should().BeBefore(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)));
        Assert.AreEqual("Expected offset to be before 01/01/2026 00:00:00 +00:00, but found 05/10/2026 08:30:00 +02:00.", ex.Message);
    }

    [TestMethod]
    public void BeSameDateAs_IgnoresTimeParts() {
        _day.Should().BeSameDateAs(_day.AddHours(5));
        _day.Should().NotBeSameDateAs(_day.AddDays(1));

        var same = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().BeSameDateAs(new DateTime(2026, 5, 11, 8, 30, 0, DateTimeKind.Unspecified)));
        Assert.AreEqual("Expected _day to be on the same date as 05/11/2026 08:30:00, but found 05/10/2026 08:30:00.", same.Message);

        var different = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().NotBeSameDateAs(_day.AddHours(5)));
        Assert.AreEqual("Expected _day not to be on the same date as 05/10/2026 13:30:00, but found 05/10/2026 08:30:00.", different.Message);
    }

    [TestMethod]
    public void BeSameDateAs_NullableNullEqualsNull() {
        DateTime? present = _day;
        present.Should().BeSameDateAs(_day.AddHours(5));

        DateTime? missing = null;
        missing.Should().BeSameDateAs(null);
        missing.Should().NotBeSameDateAs(_day);

        var presentNull = Assert.ThrowsExactly<AssertionFailedException>(() => present.Should().BeSameDateAs(null));
        Assert.AreEqual("Expected present to be on the same date as <null>, but found 05/10/2026 08:30:00.", presentNull.Message);

        var nullNull = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeSameDateAs(null));
        Assert.AreEqual("Expected missing not to be on the same date as <null>, but found <null>.", nullNull.Message);
    }

    [TestMethod]
    public void BeSameDateAs_DateTimeOffset() {
        var offset = new DateTimeOffset(2026, 5, 10, 8, 30, 0, TimeSpan.FromHours(2));
        offset.Should().BeSameDateAs(offset.AddHours(3));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => offset.Should().BeSameDateAs(offset.AddDays(1)));
        Assert.AreEqual("Expected offset to be on the same date as 05/11/2026 08:30:00 +02:00, but found 05/10/2026 08:30:00 +02:00.", ex.Message);

        DateTimeOffset? missing = null;
        missing.Should().BeSameDateAs(null);
    }

    [TestMethod]
    public void Range_BeWithin_AnchoredAndBare() {
        var moment = DateTime.UtcNow;
        moment.Should().BeWithin(TimeSpan.FromHours(1)).After(moment.AddMinutes(-30));
        moment.Should().BeWithin(TimeSpan.FromHours(1)).Before(moment.AddMinutes(30));
        DateTime.UtcNow.Should().BeWithin(TimeSpan.FromDays(1));

        var ancient = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => ancient.Should().BeWithin(TimeSpan.FromDays(1)));
        Assert.AreEqual("Expected ancient to be within 1.00:00:00 of the current moment, but found 01/01/2020 00:00:00.", ex.Message);

        var after = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().BeWithin(TimeSpan.FromSeconds(30)).After(moment.AddMinutes(-30)));
        Assert.Contains("to be within 00:00:30 after", after.Message);
        Assert.Contains("but found", after.Message);

        var before = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().BeWithin(TimeSpan.FromHours(1)).Before(moment.AddMinutes(-90)));
        Assert.Contains("to be within 01:00:00 before", before.Message);
        Assert.Contains("but found", before.Message);
    }

    [TestMethod]
    public void Range_BeLessThan_PassAndFail() {
        var moment = DateTime.UtcNow;
        moment.Should().BeLessThan(TimeSpan.FromHours(1)).After(moment.AddMinutes(-30));
        moment.Should().BeLessThan(TimeSpan.FromHours(1)).Before(moment.AddMinutes(30));
        DateTime.UtcNow.Should().BeLessThan(TimeSpan.FromDays(1));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().BeLessThan(TimeSpan.FromMinutes(30)).After(moment.AddMinutes(-30)));
        Assert.Contains("to be less than 00:30:00 after", ex.Message);
        Assert.Contains("but found", ex.Message);
    }

    [TestMethod]
    public void Range_BeMoreThan_PassAndFail() {
        _day.Should().BeMoreThan(TimeSpan.FromMinutes(10)).After(new DateTime(2026, 5, 10, 8, 0, 0, DateTimeKind.Unspecified));
        var ancient = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        ancient.Should().BeMoreThan(TimeSpan.FromDays(1));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().BeMoreThan(TimeSpan.FromHours(1)).After(new DateTime(2026, 5, 10, 8, 0, 0, DateTimeKind.Unspecified)));
        Assert.AreEqual("Expected _day to be more than 01:00:00 after 05/10/2026 08:00:00, but found 05/10/2026 08:30:00.", ex.Message);
    }

    [TestMethod]
    public void Range_BeAtLeast_PassAndFail() {
        _day.Should().BeAtLeast(TimeSpan.FromMinutes(30)).After(new DateTime(2026, 5, 10, 8, 0, 0, DateTimeKind.Unspecified));
        _day.Should().BeAtLeast(TimeSpan.FromMinutes(30)).Before(new DateTime(2026, 5, 10, 9, 0, 0, DateTimeKind.Unspecified));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().BeAtLeast(TimeSpan.FromHours(1)).Before(new DateTime(2026, 5, 10, 9, 0, 0, DateTimeKind.Unspecified)));
        Assert.AreEqual("Expected _day to be at least 01:00:00 before 05/10/2026 09:00:00, but found 05/10/2026 08:30:00.", ex.Message);
    }

    [TestMethod]
    public void Range_BeExactly_AnchoredPassAndFail() {
        _day.Should().BeExactly(TimeSpan.FromMinutes(30)).After(new DateTime(2026, 5, 10, 8, 0, 0, DateTimeKind.Unspecified));
        _day.Should().BeExactly(TimeSpan.FromMinutes(30)).Before(new DateTime(2026, 5, 10, 9, 0, 0, DateTimeKind.Unspecified));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().BeExactly(TimeSpan.FromMinutes(30)).After(new DateTime(2026, 5, 10, 8, 1, 0, DateTimeKind.Unspecified)));
        Assert.AreEqual("Expected _day to be exactly 00:30:00 after 05/10/2026 08:01:00, but found 05/10/2026 08:30:00.", ex.Message);

        DateTime? missing = null;
        var nullExact = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeExactly(TimeSpan.FromDays(1)));
        Assert.AreEqual("Expected missing to be exactly 1.00:00:00 from the current moment, but found <null>.", nullExact.Message);
    }

    [TestMethod]
    public void Range_NegativeWindow_ThrowsArgumentOutOfRange() {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _day.Should().BeWithin(TimeSpan.FromSeconds(-1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _day.Should().BeLessThan(TimeSpan.FromSeconds(-1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _day.Should().BeMoreThan(TimeSpan.FromSeconds(-1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _day.Should().BeAtLeast(TimeSpan.FromSeconds(-1)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _day.Should().BeExactly(TimeSpan.FromSeconds(-1)));
    }

    [TestMethod]
    public void Range_ExclusiveBoundsFailAtAnchor() {
        var moment = DateTime.UtcNow;
        var withinAfter = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().BeWithin(TimeSpan.FromHours(1)).After(moment));
        Assert.Contains("to be within 01:00:00 after", withinAfter.Message);

        var withinBefore = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().BeWithin(TimeSpan.FromHours(1)).Before(moment));
        Assert.Contains("to be within 01:00:00 before", withinBefore.Message);

        var ancient = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var atLeast = Assert.ThrowsExactly<AssertionFailedException>(() => ancient.Should().BeAtLeast(TimeSpan.FromDays(1)).After(ancient));
        Assert.AreEqual("Expected ancient to be at least 1.00:00:00 after 01/01/2020 00:00:00, but found 01/01/2020 00:00:00.", atLeast.Message);
    }

    [TestMethod]
    public void Range_NullableNullSubjectFails() {
        DateTime? missing = null;
        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeWithin(TimeSpan.FromDays(1)));
        Assert.AreEqual("Expected missing to be within 1.00:00:00 of the current moment, but found <null>.", ex.Message);
    }

    [TestMethod]
    public void BeIn_KindChecks() {
        _day.Should().BeIn(DateTimeKind.Unspecified);
        _day.Should().NotBeIn(DateTimeKind.Utc);

        var utc = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().BeIn(DateTimeKind.Utc));
        Assert.AreEqual("Expected _day to have kind Utc, but found Unspecified.", utc.Message);

        var not = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().NotBeIn(DateTimeKind.Unspecified));
        Assert.AreEqual("Expected _day not to have kind Unspecified, but found Unspecified.", not.Message);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _day.Should().BeIn((DateTimeKind)42));

        DateTime? missing = null;
        var nullKind = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeIn(DateTimeKind.Utc));
        Assert.AreEqual("Expected missing to have kind Utc, but found <null>.", nullKind.Message);
    }

    [TestMethod]
    public void BeOneOf_PassAndFail() {
        _day.Should().BeOneOf(_day.AddDays(-1), _day);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().BeOneOf(_day.AddDays(1)));
        Assert.AreEqual("Expected _day to be one of 05/11/2026 08:30:00, but found 05/10/2026 08:30:00.", ex.Message);

        Assert.ThrowsExactly<ArgumentNullException>(() => _day.Should().BeOneOf((DateTime[])null!));

        DateTime? missing = null;
        missing.Should().BeOneOf(null, _day);

        var nullMissing = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeOneOf(_day));
        Assert.AreEqual("Expected missing to be one of 05/10/2026 08:30:00, but found <null>.", nullMissing.Message);
    }

    [TestMethod]
    public void NotBeOnOr_Boundaries() {
        _day.Should().NotBeOnOrAfter(_day.AddDays(1));
        _day.Should().NotBeOnOrBefore(_day.AddDays(-1));

        var after = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().NotBeOnOrAfter(_day));
        Assert.AreEqual("Expected _day not to be on or after 05/10/2026 08:30:00, but found 05/10/2026 08:30:00.", after.Message);

        var before = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().NotBeOnOrBefore(_day));
        Assert.AreEqual("Expected _day not to be on or before 05/10/2026 08:30:00, but found 05/10/2026 08:30:00.", before.Message);

        DateTime? missing = null;
        missing.Should().NotBeOnOrAfter(_day);
        missing.Should().NotBeOnOrBefore(_day);
    }

    [TestMethod]
    public void NotHaveComponents_PassAndFail() {
        _day.Should().NotHaveYear(2025).And.NotHaveMonth(6).And.NotHaveDay(11).And.NotHaveHour(9).And.NotHaveMinute(31).And.NotHaveSecond(1).And.NotHaveMillisecond(1);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().NotHaveYear(2026));
        Assert.AreEqual("Expected _day not to have year 2026, but found 2026.", ex.Message);

        DateTime? missing = null;
        var nullYear = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotHaveYear(2026));
        Assert.AreEqual("Expected missing not to have year 2026, but found <null>.", nullYear.Message);
    }

    [TestMethod]
    public void HaveMillisecond_PassAndFail() {
        _day.Should().HaveMillisecond(0);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => _day.Should().HaveMillisecond(1));
        Assert.AreEqual("Expected _day to have millisecond 1, but found 0.", ex.Message);

        DateTime? missing = null;
        var nullMs = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveMillisecond(0));
        Assert.AreEqual("Expected missing to have millisecond 0, but found <null>.", nullMs.Message);
    }

    [TestMethod]
    public void DateTimeOffset_Ranges() {
        var offset = new DateTimeOffset(2026, 5, 10, 8, 30, 0, TimeSpan.FromHours(2));
        var moment = DateTimeOffset.Now;
        moment.Should().BeWithin(TimeSpan.FromHours(1)).After(moment.AddMinutes(-30));
        offset.Should().BeExactly(TimeSpan.FromMinutes(30)).After(new DateTimeOffset(2026, 5, 10, 8, 0, 0, TimeSpan.FromHours(2)));
        DateTimeOffset.Now.Should().BeWithin(TimeSpan.FromDays(1));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().BeWithin(TimeSpan.FromSeconds(10)).After(moment.AddMinutes(-30)));
        Assert.Contains("to be within 00:00:10 after", ex.Message);
        Assert.Contains("but found", ex.Message);

        var less = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().BeLessThan(TimeSpan.FromMinutes(30)).After(moment.AddMinutes(-30)));
        Assert.Contains("to be less than 00:30:00 after", less.Message);

        var more = Assert.ThrowsExactly<AssertionFailedException>(() => offset.Should().BeMoreThan(TimeSpan.FromHours(1)).Before(new DateTimeOffset(2026, 5, 10, 9, 0, 0, TimeSpan.FromHours(2))));
        Assert.AreEqual("Expected offset to be more than 01:00:00 before 05/10/2026 09:00:00 +02:00, but found 05/10/2026 08:30:00 +02:00.", more.Message);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => offset.Should().BeWithin(TimeSpan.FromSeconds(-1)));
    }

    [TestMethod]
    public void DateTimeOffset_HaveOffset_PassAndFail() {
        var offset = new DateTimeOffset(2026, 5, 10, 8, 30, 0, TimeSpan.FromHours(2));
        offset.Should().HaveOffset(TimeSpan.FromHours(2));
        offset.Should().NotHaveOffset(TimeSpan.FromHours(3));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => offset.Should().HaveOffset(TimeSpan.FromHours(3)));
        Assert.AreEqual("Expected offset to have offset 03:00:00, but found 02:00:00.", ex.Message);

        var not = Assert.ThrowsExactly<AssertionFailedException>(() => offset.Should().NotHaveOffset(TimeSpan.FromHours(2)));
        Assert.AreEqual("Expected offset not to have offset 02:00:00, but found 02:00:00.", not.Message);

        DateTimeOffset? missing = null;
        var nullOffset = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveOffset(TimeSpan.FromHours(2)));
        Assert.AreEqual("Expected missing to have offset 02:00:00, but found <null>.", nullOffset.Message);
    }

    [TestMethod]
    public void DateTimeOffset_BeExactly_DistinguishesOffset() {
        var offset = new DateTimeOffset(2026, 5, 10, 8, 30, 0, TimeSpan.FromHours(2));
        var sameInstant = new DateTimeOffset(2026, 5, 10, 9, 30, 0, TimeSpan.FromHours(3));
        offset.Should().Be(sameInstant);
        offset.Should().NotBeExactly(sameInstant);

        var ex = Assert.ThrowsExactly<AssertionFailedException>(() => offset.Should().BeExactly(sameInstant));
        Assert.AreEqual("Expected offset to be exactly 05/10/2026 09:30:00 +03:00, but found 05/10/2026 08:30:00 +02:00.", ex.Message);

        var not = Assert.ThrowsExactly<AssertionFailedException>(() => offset.Should().NotBeExactly(offset));
        Assert.AreEqual("Expected offset not to be exactly 05/10/2026 08:30:00 +02:00, but found 05/10/2026 08:30:00 +02:00.", not.Message);

        DateTimeOffset? missing = null;
        missing.Should().BeExactly(null);
        var nullNot = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotBeExactly(null));
        Assert.AreEqual("Expected missing not to be exactly <null>, but found <null>.", nullNot.Message);
    }

    [TestMethod]
    public void DateTimeOffset_BeOneOf_AndNegatives() {
        var offset = new DateTimeOffset(2026, 5, 10, 8, 30, 0, TimeSpan.FromHours(2));
        offset.Should().BeOneOf(offset.AddDays(-1), offset);

        var oneOf = Assert.ThrowsExactly<AssertionFailedException>(() => offset.Should().BeOneOf(offset.AddDays(1)));
        Assert.AreEqual("Expected offset to be one of 05/11/2026 08:30:00 +02:00, but found 05/10/2026 08:30:00 +02:00.", oneOf.Message);

        offset.Should().NotBeOnOrAfter(offset.AddDays(1));
        var onOrAfter = Assert.ThrowsExactly<AssertionFailedException>(() => offset.Should().NotBeOnOrAfter(offset));
        Assert.AreEqual("Expected offset not to be on or after 05/10/2026 08:30:00 +02:00, but found 05/10/2026 08:30:00 +02:00.", onOrAfter.Message);

        offset.Should().NotHaveYear(2025);
        var year = Assert.ThrowsExactly<AssertionFailedException>(() => offset.Should().NotHaveYear(2026));
        Assert.AreEqual("Expected offset not to have year 2026, but found 2026.", year.Message);

        offset.Should().HaveMillisecond(0);
        offset.Should().NotHaveMillisecond(1);

        DateTimeOffset? missing = null;
        missing.Should().NotBeOnOrAfter(offset);
        var nullMs = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().NotHaveMillisecond(0));
        Assert.AreEqual("Expected missing not to have millisecond 0, but found <null>.", nullMs.Message);
    }
}
