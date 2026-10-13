//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class FluentSugarTests {
    [TestMethod]
    public void MonthFactories_BuildExpectedDates() {
        Assert.AreEqual(new DateTime(2010, 3, 1, 0, 0, 0, DateTimeKind.Unspecified), 1.March(2010));
        Assert.AreEqual(new DateTime(2026, 1, 31, 0, 0, 0, DateTimeKind.Unspecified), 31.January(2026));
        Assert.AreEqual(new DateTime(2024, 2, 29, 0, 0, 0, DateTimeKind.Unspecified), 29.February(2024));
        Assert.AreEqual(new DateTime(2026, 12, 25, 0, 0, 0, DateTimeKind.Unspecified), 25.December(2026));
    }

    [TestMethod]
    public void At_SetsTimePreservingKind() {
        var date = 1.March(2010).At(22, 15);
        Assert.AreEqual(new DateTime(2010, 3, 1, 22, 15, 0, DateTimeKind.Unspecified), date);

        var precise = 1.March(2010).At(22, 15, 30);
        Assert.AreEqual(new DateTime(2010, 3, 1, 22, 15, 30, DateTimeKind.Unspecified), precise);
    }

    [TestMethod]
    public void AsUtc_AsLocal_LabelKind() {
        Assert.AreEqual(DateTimeKind.Utc, 1.March(2010).AsUtc().Kind);
        Assert.AreEqual(DateTimeKind.Local, 1.March(2010).AsLocal().Kind);
    }

    [TestMethod]
    public void WithOffset_BuildsOffset() {
        var offset = 1.March(2010).At(22, 15).WithOffset(TimeSpan.FromHours(2));
        Assert.AreEqual(new DateTimeOffset(2010, 3, 1, 22, 15, 0, TimeSpan.FromHours(2)), offset);
    }

    [TestMethod]
    public void AddMicroseconds_ShiftsDate() {
        var date = 1.March(2010).AddMicroseconds(1500);
        Assert.AreEqual(new DateTime(2010, 3, 1, 0, 0, 0, DateTimeKind.Unspecified).AddTicks(15000), date);

        var offset = date.WithOffset(TimeSpan.FromHours(2)).AddMicroseconds(-500);
        Assert.AreEqual(new DateTimeOffset(2010, 3, 1, 0, 0, 0, TimeSpan.FromHours(2)).AddTicks(10000), offset);
    }

    [TestMethod]
    public void AddNanoseconds_ShiftsDate() {
        var date = 1.March(2010).AddNanoseconds(1500);
        Assert.AreEqual(new DateTime(2010, 3, 1, 0, 0, 0, DateTimeKind.Unspecified).AddTicks(15), date);

        var offset = 1.March(2010).WithOffset(TimeSpan.FromHours(2)).AddNanoseconds(-100);
        Assert.AreEqual(new DateTimeOffset(2010, 3, 1, 0, 0, 0, TimeSpan.FromHours(2)).AddTicks(-1), offset);
    }

    [TestMethod]
    public void DateComponentExtractors_ReturnSubUnitRemainders() {
        var date = new DateTime(2010, 3, 1, 0, 0, 0, DateTimeKind.Unspecified);
        Assert.AreEqual(500, date.AddTicks(12345).Nanosecond());
        Assert.AreEqual(45, date.AddTicks(10450).Microsecond());

        var offset = date.WithOffset(TimeSpan.FromHours(2));
        Assert.AreEqual(500, offset.AddTicks(12345).Nanosecond());
        Assert.AreEqual(45, offset.AddTicks(10450).Microsecond());
    }

    [TestMethod]
    public void At_WithSubSecondParts_SetsPreciseTime() {
        var precise = 1.March(2010).At(22, 15, 30, 500, 250, 100);
        var expected = new DateTime(2010, 3, 1, 22, 15, 30, 500, DateTimeKind.Unspecified).AddTicks(2501);
        Assert.AreEqual(expected, precise);

        var timeOfDay = 1.March(2010).At(TimeSpan.FromHours(22.5));
        Assert.AreEqual(new DateTime(2010, 3, 1, 22, 30, 0, DateTimeKind.Unspecified), timeOfDay);

        var offset = 1.March(2010).WithOffset(TimeSpan.FromHours(2)).At(22, 15, 30, 500);
        Assert.AreEqual(new DateTimeOffset(2010, 3, 1, 22, 15, 30, 500, TimeSpan.FromHours(2)), offset);
    }

    [TestMethod]
    public void At_SubSecondOutOfRange_ThrowsArgumentOutOfRange() {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => 1.March(2010).At(22, 15, microsecond: 1000));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => 1.March(2010).At(22, 15, nanosecond: -1));

        var offset = 1.March(2010).WithOffset(TimeSpan.FromHours(2));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => offset.At(22, 15, microsecond: -1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => offset.At(22, 15, nanosecond: 1000));
    }

    [TestMethod]
    public void TimeFactories_BuildExpectedSpans() {
        Assert.AreEqual(TimeSpan.FromSeconds(3), 3.Seconds());
        Assert.AreEqual(TimeSpan.FromHours(2), 2.Hours());
        Assert.AreEqual(TimeSpan.FromMinutes(5), 5.Minutes());
        Assert.AreEqual(TimeSpan.FromMilliseconds(100), 100.Milliseconds());
        Assert.AreEqual(TimeSpan.FromTicks(10), 1.Microseconds());
        Assert.AreEqual(TimeSpan.FromTicks(1), 100.Nanoseconds());
        Assert.AreEqual(TimeSpan.FromTicks(123456789L), 123456789L.Ticks());
    }

    [TestMethod]
    public void Totals_MeasureSpans() {
        Assert.AreEqual(1500.0, TimeSpan.FromTicks(15000).TotalMicroseconds());
        Assert.AreEqual(1500000.0, TimeSpan.FromTicks(15000).TotalNanoseconds());
    }

    [TestMethod]
    public void DaysFactory_BuildsDaySpans() {
        Assert.AreEqual(TimeSpan.FromDays(2), 2.Days());
        Assert.AreEqual(TimeSpan.FromDays(1.5), 1.5.Days());
        Assert.AreEqual(TimeSpan.FromDays(3).Add(TimeSpan.FromHours(12)), 3.Days(TimeSpan.FromHours(12)));
    }

    [TestMethod]
    public void OffsetAndDoubleOverloads_BuildPartialSpans() {
        Assert.AreEqual(TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(30)), 2.Hours(TimeSpan.FromMinutes(30)));
        Assert.AreEqual(TimeSpan.FromMinutes(5).Add(TimeSpan.FromSeconds(10)), 5.Minutes(TimeSpan.FromSeconds(10)));
        Assert.AreEqual(TimeSpan.FromSeconds(3).Add(TimeSpan.FromMilliseconds(250)), 3.Seconds(TimeSpan.FromMilliseconds(250)));
        Assert.AreEqual(TimeSpan.FromHours(1.5), 1.5.Hours());
        Assert.AreEqual(TimeSpan.FromMinutes(2.5), 2.5.Minutes());
        Assert.AreEqual(TimeSpan.FromSeconds(0.5), 0.5.Seconds());
        Assert.AreEqual(TimeSpan.FromMilliseconds(2.5), 2.5.Milliseconds());
        Assert.AreEqual(TimeSpan.FromTicks(50), 5L.Microseconds());
        Assert.AreEqual(TimeSpan.FromTicks(10), 1000L.Nanoseconds());
        Assert.AreEqual(TimeSpan.FromTicks(77), 77.Ticks());
    }

    [TestMethod]
    public void ComponentExtractors_ReturnSubUnitRemainders() {
        Assert.AreEqual(400, TimeSpan.FromTicks(1234).Nanoseconds());
        Assert.AreEqual(500, TimeSpan.FromTicks(15000).Microseconds());
        Assert.AreEqual(0, TimeSpan.FromMilliseconds(1).Nanoseconds());
        Assert.AreEqual(0, TimeSpan.FromMilliseconds(1).Microseconds());
    }

    [TestMethod]
    public void And_ChainsSpans() {
        Assert.AreEqual(TimeSpan.FromMinutes((23 * 60) + 59), 23.Hours().And(59.Minutes()));
        Assert.AreEqual(TimeSpan.Zero, TimeSpan.Zero.And(TimeSpan.Zero));
    }

    [TestMethod]
    public void Before_After_AssertOnDates() {
        var day = 1.March(2010);
        2.Hours().Before(day).Be(new DateTime(2010, 2, 28, 22, 0, 0, DateTimeKind.Unspecified));
        2.Hours().After(day).Be(new DateTime(2010, 3, 1, 2, 0, 0, DateTimeKind.Unspecified));

        var ex = Assert.ThrowsExactly<AssertionFailedException>(
            () => 2.Hours().Before(day).Be(new DateTime(2010, 2, 28, 23, 0, 0, DateTimeKind.Unspecified)));
        Assert.AreEqual("Expected day to be 02/28/2010 23:00:00, but found 02/28/2010 22:00:00.", ex.Message);

        var offset = day.WithOffset(TimeSpan.Zero);
        30.Minutes().Before(offset).Be(new DateTimeOffset(2010, 2, 28, 23, 30, 0, TimeSpan.Zero));
        30.Minutes().After(offset).Be(new DateTimeOffset(2010, 3, 1, 0, 30, 0, TimeSpan.Zero));
    }
}
