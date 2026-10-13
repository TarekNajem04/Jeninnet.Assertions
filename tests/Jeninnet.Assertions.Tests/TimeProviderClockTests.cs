//
// Copyright (c) Jeninnet.
// Part of the Jeninnet Platform.
// Repository: https://github.com/TarekNajem04/Jeninnet.Assertions/
// Licensed under the MIT License.
//
namespace Jeninnet.Assertions.Tests;

[TestClass]
public class TimeProviderClockTests {
    private static readonly DateTimeOffset _epoch = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [TestCleanup]
    public void Reset() => AssertionClock.Current = SystemClock.Instance;

    [TestMethod]
    public void UtcAndLocal_ReadFromTheProvider() {
        var zone = TimeZoneInfo.CreateCustomTimeZone("fixed+02", TimeSpan.FromHours(2), "fixed+02", "fixed+02");
        var clock = new TimeProviderClock(new StubTimeProvider(_epoch, zone));

        Assert.AreEqual(new DateTimeOffset(2026, 1, 1, 2, 0, 0, TimeSpan.FromHours(2)), clock.Now);
        Assert.AreEqual(_epoch, clock.UtcNow);
    }

    [TestMethod]
    public void NullProvider_ThrowsArgumentNull() {
        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => new TimeProviderClock(null!));
        Assert.AreEqual("timeProvider", ex.ParamName);
    }

    [TestMethod]
    public void InjectedAdapter_DrivesBoundaryAssertions() {
        var zone = TimeZoneInfo.CreateCustomTimeZone("fixed+02", TimeSpan.FromHours(2), "fixed+02", "fixed+02");
        AssertionClock.Current = new TimeProviderClock(new StubTimeProvider(_epoch, zone));
        var value = new DateTime(2026, 1, 1, 0, 5, 0, DateTimeKind.Utc);

        value.Should().BeWithin(TimeSpan.FromMinutes(5));

        Assert.ThrowsExactly<AssertionFailedException>(() => value.Should().BeWithin(TimeSpan.FromMinutes(3)));
    }

    private sealed class StubTimeProvider(DateTimeOffset utcNow, TimeZoneInfo localTimeZone) : TimeProvider {
        public override DateTimeOffset GetUtcNow() => utcNow;

        public override TimeZoneInfo LocalTimeZone => localTimeZone;
    }
}
