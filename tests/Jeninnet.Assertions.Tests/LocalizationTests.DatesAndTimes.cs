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
        var be = Throws<AssertionFailedException>(() => day.Should().Be(new DateOnly(2026, 5, 11)));
        be.Message.Should().Be("المتوقع من day أن يكون 05/11/2026، لكن وُجد 05/10/2026.");

        var notBe = Throws<AssertionFailedException>(() => day.Should().NotBe(new DateOnly(2026, 5, 10)));
        notBe.Message.Should().Be("المتوقع من day ألا يكون 05/10/2026، لكن وُجد 05/10/2026.");

        var before = Throws<AssertionFailedException>(() => day.Should().BeBefore(new DateOnly(2026, 1, 1)));
        before.Message.Should().Be("المتوقع من day أن يكون قبل 01/01/2026، لكن وُجد 05/10/2026.");

        var notBefore = Throws<AssertionFailedException>(() => day.Should().NotBeBefore(new DateOnly(2027, 1, 1)));
        notBefore.Message.Should().Be("المتوقع من day ألا يكون قبل 01/01/2027، لكن وُجد 05/10/2026.");

        var onOrBefore = Throws<AssertionFailedException>(() => day.Should().BeOnOrBefore(new DateOnly(2026, 1, 1)));
        onOrBefore.Message.Should().Be("المتوقع من day أن يكون في 01/01/2026 أو قبله، لكن وُجد 05/10/2026.");

        var after = Throws<AssertionFailedException>(() => day.Should().BeAfter(new DateOnly(2027, 1, 1)));
        after.Message.Should().Be("المتوقع من day أن يكون بعد 01/01/2027، لكن وُجد 05/10/2026.");

        var notAfter = Throws<AssertionFailedException>(() => day.Should().NotBeAfter(new DateOnly(2026, 1, 1)));
        notAfter.Message.Should().Be("المتوقع من day ألا يكون بعد 01/01/2026، لكن وُجد 05/10/2026.");

        var onOrAfter = Throws<AssertionFailedException>(() => day.Should().BeOnOrAfter(new DateOnly(2027, 1, 1)));
        onOrAfter.Message.Should().Be("المتوقع من day أن يكون في 01/01/2027 أو بعده، لكن وُجد 05/10/2026.");

        var year = Throws<AssertionFailedException>(() => day.Should().HaveYear(2025));
        year.Message.Should().Be("المتوقع من day أن تكون سنته 2025، لكن وُجد 2026.");

        var month = Throws<AssertionFailedException>(() => day.Should().HaveMonth(6));
        month.Message.Should().Be("المتوقع من day أن يكون شهره 6، لكن وُجد 5.");

        var date = Throws<AssertionFailedException>(() => day.Should().HaveDay(11));
        date.Message.Should().Be("المتوقع من day أن يكون يومه 11، لكن وُجد 10.");

        DateOnly? missing = null;
        var missingBefore = Throws<AssertionFailedException>(() => missing.Should().BeBefore(new DateOnly(2026, 5, 10)));
        missingBefore.Message.Should().Be("المتوقع من missing أن يكون قبل 05/10/2026، لكن وُجد <null>.");

        DateOnly? present = day;
        var beNull = Throws<AssertionFailedException>(() => present.Should().BeNull());
        beNull.Message.Should().Be("المتوقع من present أن يكون null، لكن وُجد 05/10/2026.");

        var haveValue = Throws<AssertionFailedException>(() => missing.Should().HaveValue());
        haveValue.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد null.");

        var missingYear = Throws<AssertionFailedException>(() => missing.Should().HaveYear(2026));
        missingYear.Message.Should().Be("المتوقع من missing أن تكون سنته 2026، لكن وُجد <null>.");
    }

    [TestMethod]
    public void Arabic_DateTime() {
        using var _ = new ArabicScope();

        var moment = new DateTime(2026, 5, 10, 8, 30, 0, DateTimeKind.Unspecified);
        var be = Throws<AssertionFailedException>(() => moment.Should().Be(new DateTime(2026, 5, 11, 8, 30, 0, DateTimeKind.Unspecified)));
        be.Message.Should().Be("المتوقع من moment أن يكون 05/11/2026 08:30:00، لكن وُجد 05/10/2026 08:30:00.");

        var close = Throws<AssertionFailedException>(() => moment.Should().BeCloseTo(new DateTime(2026, 5, 10, 8, 30, 30, DateTimeKind.Unspecified), TimeSpan.FromSeconds(10)));
        close.Message.Should().Be("المتوقع من moment أن يكون قريباً من 05/10/2026 08:30:30 (±00:00:10)، لكن وُجد 05/10/2026 08:30:00.");

        var notClose = Throws<AssertionFailedException>(() => moment.Should().NotBeCloseTo(moment, TimeSpan.FromMinutes(1)));
        notClose.Message.Should().Be("المتوقع من moment ألا يكون قريباً من 05/10/2026 08:30:00 (±00:01:00)، لكن وُجد 05/10/2026 08:30:00.");

        var hour = Throws<AssertionFailedException>(() => moment.Should().HaveHour(9));
        hour.Message.Should().Be("المتوقع من moment أن تكون ساعته 9، لكن وُجد 8.");

        var minute = Throws<AssertionFailedException>(() => moment.Should().HaveMinute(31));
        minute.Message.Should().Be("المتوقع من moment أن تكون دقيقته 31، لكن وُجد 30.");

        var second = Throws<AssertionFailedException>(() => moment.Should().HaveSecond(1));
        second.Message.Should().Be("المتوقع من moment أن تكون ثانيته 1، لكن وُجد 0.");

        DateTime? missing = null;
        var missingBefore = Throws<AssertionFailedException>(() => missing.Should().BeBefore(new DateTime(2026, 5, 10, 8, 30, 0, DateTimeKind.Unspecified)));
        missingBefore.Message.Should().Be("المتوقع من missing أن يكون قبل 05/10/2026 08:30:00، لكن وُجد <null>.");

        var haveValue = Throws<AssertionFailedException>(() => missing.Should().HaveValue());
        haveValue.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد null.");
    }

    [TestMethod]
    public void Arabic_DateTimeOffsetAndTimeOnly() {
        using var _ = new ArabicScope();

        var offset = new DateTimeOffset(2026, 5, 10, 8, 30, 0, TimeSpan.FromHours(2));
        var before = Throws<AssertionFailedException>(() => offset.Should().BeBefore(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)));
        before.Message.Should().Be("المتوقع من offset أن يكون قبل 01/01/2026 00:00:00 +00:00، لكن وُجد 05/10/2026 08:30:00 +02:00.");

        var morning = new TimeOnly(8, 30);
        var be = Throws<AssertionFailedException>(() => morning.Should().Be(new TimeOnly(9, 0)));
        be.Message.Should().Be("المتوقع من morning أن يكون 09:00، لكن وُجد 08:30.");

        var close = Throws<AssertionFailedException>(() => morning.Should().BeCloseTo(new TimeOnly(9, 30), TimeSpan.FromMinutes(10)));
        close.Message.Should().Be("المتوقع من morning أن يكون قريباً من 09:30 (±00:10:00)، لكن وُجد 08:30.");

        var millisecond = Throws<AssertionFailedException>(() => morning.Should().HaveMillisecond(500));
        millisecond.Message.Should().Be("المتوقع من morning أن تكون المللي ثانية 500، لكن وُجد 0.");

        TimeOnly? missing = null;
        var missingHour = Throws<AssertionFailedException>(() => missing.Should().HaveHour(9));
        missingHour.Message.Should().Be("المتوقع من missing أن تكون ساعته 9، لكن وُجد <null>.");
    }

    [TestMethod]
    public void Arabic_TimeSpan() {
        using var _ = new ArabicScope();

        var duration = TimeSpan.FromHours(2.5);
        var be = Throws<AssertionFailedException>(() => duration.Should().Be(TimeSpan.FromHours(3)));
        be.Message.Should().Be("المتوقع من duration أن يكون 03:00:00، لكن وُجد 02:30:00.");

        var zero = TimeSpan.Zero;
        var positive = Throws<AssertionFailedException>(() => zero.Should().BePositive());
        positive.Message.Should().Be("المتوقع من zero أن يكون موجباً، لكن وُجد 00:00:00.");

        var negative = Throws<AssertionFailedException>(() => duration.Should().BeNegative());
        negative.Message.Should().Be("المتوقع من duration أن يكون سالباً، لكن وُجد 02:30:00.");

        var greater = Throws<AssertionFailedException>(() => duration.Should().BeGreaterThan(TimeSpan.FromHours(3)));
        greater.Message.Should().Be("المتوقع من duration أن يكون أكبر من 03:00:00، لكن وُجد 02:30:00.");

        var less = Throws<AssertionFailedException>(() => duration.Should().BeLessThan(TimeSpan.FromHours(2)));
        less.Message.Should().Be("المتوقع من duration أن يكون أصغر من 02:00:00، لكن وُجد 02:30:00.");

        var span = TimeSpan.FromSeconds(100);
        var close = Throws<AssertionFailedException>(() => span.Should().BeCloseTo(TimeSpan.FromSeconds(105), TimeSpan.FromSeconds(2)));
        close.Message.Should().Be("المتوقع من span أن يكون قريباً من 00:01:45 (±00:00:02)، لكن وُجد 00:01:40.");

        var hours = Throws<AssertionFailedException>(() => duration.Should().HaveHours(3));
        hours.Message.Should().Be("المتوقع من duration أن تكون ساعاته 3، لكن وُجد 2.");

        var ticks = Throws<AssertionFailedException>(() => duration.Should().HaveTicks(1));
        ticks.Message.Should().Be("المتوقع من duration أن تكون تكاته 1، لكن وُجد 90000000000.");

        TimeSpan? missing = null;
        var missingGreater = Throws<AssertionFailedException>(() => missing.Should().BeGreaterThan(TimeSpan.FromHours(2.5)));
        missingGreater.Message.Should().Be("المتوقع من missing أن يكون أكبر من 02:30:00، لكن وُجد <null>.");

        var haveValue = Throws<AssertionFailedException>(() => missing.Should().HaveValue());
        haveValue.Message.Should().Be("المتوقع من missing ألا يكون null، لكن وُجد null.");
    }
}
