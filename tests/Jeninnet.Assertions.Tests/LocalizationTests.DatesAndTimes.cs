//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

public partial class LocalizationTests {
    [TestMethod]
    public void Arabic_DateOnly() {
        using var _ = new ArabicScope();

        var day = new DateOnly(2026, 5, 10);
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().Be(new DateOnly(2026, 5, 11)));
        Assert.AreEqual("المتوقع من day أن يكون 05/11/2026، لكن وُجد 05/10/2026.", be.Message);

        var notBe = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().NotBe(new DateOnly(2026, 5, 10)));
        Assert.AreEqual("المتوقع من day ألا يكون 05/10/2026، لكن وُجد 05/10/2026.", notBe.Message);

        var before = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().BeBefore(new DateOnly(2026, 1, 1)));
        Assert.AreEqual("المتوقع من day أن يكون قبل 01/01/2026، لكن وُجد 05/10/2026.", before.Message);

        var notBefore = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().NotBeBefore(new DateOnly(2027, 1, 1)));
        Assert.AreEqual("المتوقع من day ألا يكون قبل 01/01/2027، لكن وُجد 05/10/2026.", notBefore.Message);

        var onOrBefore = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().BeOnOrBefore(new DateOnly(2026, 1, 1)));
        Assert.AreEqual("المتوقع من day أن يكون في 01/01/2026 أو قبله، لكن وُجد 05/10/2026.", onOrBefore.Message);

        var after = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().BeAfter(new DateOnly(2027, 1, 1)));
        Assert.AreEqual("المتوقع من day أن يكون بعد 01/01/2027، لكن وُجد 05/10/2026.", after.Message);

        var notAfter = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().NotBeAfter(new DateOnly(2026, 1, 1)));
        Assert.AreEqual("المتوقع من day ألا يكون بعد 01/01/2026، لكن وُجد 05/10/2026.", notAfter.Message);

        var onOrAfter = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().BeOnOrAfter(new DateOnly(2027, 1, 1)));
        Assert.AreEqual("المتوقع من day أن يكون في 01/01/2027 أو بعده، لكن وُجد 05/10/2026.", onOrAfter.Message);

        var year = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().HaveYear(2025));
        Assert.AreEqual("المتوقع من day أن تكون سنته 2025، لكن وُجد 2026.", year.Message);

        var month = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().HaveMonth(6));
        Assert.AreEqual("المتوقع من day أن يكون شهره 6، لكن وُجد 5.", month.Message);

        var date = Assert.ThrowsExactly<AssertionFailedException>(() => day.Should().HaveDay(11));
        Assert.AreEqual("المتوقع من day أن يكون يومه 11، لكن وُجد 10.", date.Message);

        DateOnly? missing = null;
        var missingBefore = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeBefore(new DateOnly(2026, 5, 10)));
        Assert.AreEqual("المتوقع من missing أن يكون قبل 05/10/2026، لكن وُجد <null>.", missingBefore.Message);

        DateOnly? present = day;
        var beNull = Assert.ThrowsExactly<AssertionFailedException>(() => present.Should().BeNull());
        Assert.AreEqual("المتوقع من present أن يكون null، لكن وُجد 05/10/2026.", beNull.Message);

        var haveValue = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد null.", haveValue.Message);

        var missingYear = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveYear(2026));
        Assert.AreEqual("المتوقع من missing أن تكون سنته 2026، لكن وُجد <null>.", missingYear.Message);
    }

    [TestMethod]
    public void Arabic_DateTime() {
        using var _ = new ArabicScope();

        var moment = new DateTime(2026, 5, 10, 8, 30, 0, DateTimeKind.Unspecified);
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().Be(new DateTime(2026, 5, 11, 8, 30, 0, DateTimeKind.Unspecified)));
        Assert.AreEqual("المتوقع من moment أن يكون 05/11/2026 08:30:00، لكن وُجد 05/10/2026 08:30:00.", be.Message);

        var close = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().BeCloseTo(new DateTime(2026, 5, 10, 8, 30, 30, DateTimeKind.Unspecified), TimeSpan.FromSeconds(10)));
        Assert.AreEqual("المتوقع من moment أن يكون قريباً من 05/10/2026 08:30:30 (±00:00:10)، لكن وُجد 05/10/2026 08:30:00.", close.Message);

        var notClose = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().NotBeCloseTo(moment, TimeSpan.FromMinutes(1)));
        Assert.AreEqual("المتوقع من moment ألا يكون قريباً من 05/10/2026 08:30:00 (±00:01:00)، لكن وُجد 05/10/2026 08:30:00.", notClose.Message);

        var hour = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().HaveHour(9));
        Assert.AreEqual("المتوقع من moment أن تكون ساعته 9، لكن وُجد 8.", hour.Message);

        var minute = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().HaveMinute(31));
        Assert.AreEqual("المتوقع من moment أن تكون دقيقته 31، لكن وُجد 30.", minute.Message);

        var second = Assert.ThrowsExactly<AssertionFailedException>(() => moment.Should().HaveSecond(1));
        Assert.AreEqual("المتوقع من moment أن تكون ثانيته 1، لكن وُجد 0.", second.Message);

        DateTime? missing = null;
        var missingBefore = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeBefore(new DateTime(2026, 5, 10, 8, 30, 0, DateTimeKind.Unspecified)));
        Assert.AreEqual("المتوقع من missing أن يكون قبل 05/10/2026 08:30:00، لكن وُجد <null>.", missingBefore.Message);

        var haveValue = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد null.", haveValue.Message);
    }

    [TestMethod]
    public void Arabic_DateTimeOffsetAndTimeOnly() {
        using var _ = new ArabicScope();

        var offset = new DateTimeOffset(2026, 5, 10, 8, 30, 0, TimeSpan.FromHours(2));
        var before = Assert.ThrowsExactly<AssertionFailedException>(() => offset.Should().BeBefore(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)));
        Assert.AreEqual("المتوقع من offset أن يكون قبل 01/01/2026 00:00:00 +00:00، لكن وُجد 05/10/2026 08:30:00 +02:00.", before.Message);

        var morning = new TimeOnly(8, 30);
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => morning.Should().Be(new TimeOnly(9, 0)));
        Assert.AreEqual("المتوقع من morning أن يكون 09:00، لكن وُجد 08:30.", be.Message);

        var close = Assert.ThrowsExactly<AssertionFailedException>(() => morning.Should().BeCloseTo(new TimeOnly(9, 30), TimeSpan.FromMinutes(10)));
        Assert.AreEqual("المتوقع من morning أن يكون قريباً من 09:30 (±00:10:00)، لكن وُجد 08:30.", close.Message);

        var millisecond = Assert.ThrowsExactly<AssertionFailedException>(() => morning.Should().HaveMillisecond(500));
        Assert.AreEqual("المتوقع من morning أن تكون المللي ثانية 500، لكن وُجد 0.", millisecond.Message);

        TimeOnly? missing = null;
        var missingHour = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveHour(9));
        Assert.AreEqual("المتوقع من missing أن تكون ساعته 9، لكن وُجد <null>.", missingHour.Message);
    }

    [TestMethod]
    public void Arabic_TimeSpan() {
        using var _ = new ArabicScope();

        var duration = TimeSpan.FromHours(2.5);
        var be = Assert.ThrowsExactly<AssertionFailedException>(() => duration.Should().Be(TimeSpan.FromHours(3)));
        Assert.AreEqual("المتوقع من duration أن يكون 03:00:00، لكن وُجد 02:30:00.", be.Message);

        var zero = TimeSpan.Zero;
        var positive = Assert.ThrowsExactly<AssertionFailedException>(() => zero.Should().BePositive());
        Assert.AreEqual("المتوقع من zero أن يكون موجباً، لكن وُجد 00:00:00.", positive.Message);

        var negative = Assert.ThrowsExactly<AssertionFailedException>(() => duration.Should().BeNegative());
        Assert.AreEqual("المتوقع من duration أن يكون سالباً، لكن وُجد 02:30:00.", negative.Message);

        var greater = Assert.ThrowsExactly<AssertionFailedException>(() => duration.Should().BeGreaterThan(TimeSpan.FromHours(3)));
        Assert.AreEqual("المتوقع من duration أن يكون أكبر من 03:00:00، لكن وُجد 02:30:00.", greater.Message);

        var less = Assert.ThrowsExactly<AssertionFailedException>(() => duration.Should().BeLessThan(TimeSpan.FromHours(2)));
        Assert.AreEqual("المتوقع من duration أن يكون أصغر من 02:00:00، لكن وُجد 02:30:00.", less.Message);

        var span = TimeSpan.FromSeconds(100);
        var close = Assert.ThrowsExactly<AssertionFailedException>(() => span.Should().BeCloseTo(TimeSpan.FromSeconds(105), TimeSpan.FromSeconds(2)));
        Assert.AreEqual("المتوقع من span أن يكون قريباً من 00:01:45 (±00:00:02)، لكن وُجد 00:01:40.", close.Message);

        var hours = Assert.ThrowsExactly<AssertionFailedException>(() => duration.Should().HaveHours(3));
        Assert.AreEqual("المتوقع من duration أن تكون ساعاته 3، لكن وُجد 2.", hours.Message);

        var ticks = Assert.ThrowsExactly<AssertionFailedException>(() => duration.Should().HaveTicks(1));
        Assert.AreEqual("المتوقع من duration أن تكون تكاته 1، لكن وُجد 90000000000.", ticks.Message);

        TimeSpan? missing = null;
        var missingGreater = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().BeGreaterThan(TimeSpan.FromHours(2.5)));
        Assert.AreEqual("المتوقع من missing أن يكون أكبر من 02:30:00، لكن وُجد <null>.", missingGreater.Message);

        var haveValue = Assert.ThrowsExactly<AssertionFailedException>(() => missing.Should().HaveValue());
        Assert.AreEqual("المتوقع من missing ألا يكون null، لكن وُجد null.", haveValue.Message);
    }
}
